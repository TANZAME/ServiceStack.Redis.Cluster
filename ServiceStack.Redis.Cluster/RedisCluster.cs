
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceStack.Logging;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// 线程安全的 Reids 集群操作客户端
    /// </summary>
    public partial class RedisCluster /*: IRedisNativeClient,IDisposable*/
    {
        #region 私有字段

        /// <summary>
        /// 同步最小间隔时间（毫秒），默认 3 秒
        /// </summary>
        public const int MONITORINTERVAL = 3 * 1000;

        // 同步锁对象
        object _objLock = new object();
        // 同步标志位
        bool _isDiscoverying = false;
        // 最后一次同步时间
        DateTime _lastDiscoveryTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        int _disposeAttempts = 0;
        IEnumerable<ClusterNode> _source = null;
        RedisClientManagerConfig _config = null;
        IList<InternalClusterNode> _masters = null;
        IList<InternalClusterNode> _clusterNodes = null;
        IDictionary<int, PooledRedisClientManager> _redisClientManagers = null;

        static Random _rd = new Random();

        #endregion

        #region 构造函数

        /// <summary>
        /// 实例化 <see cref="RedisCluster"/> 类的新实例
        /// <para>若群集中有指定 password 的节点，必须使用  IEnumerable&lt;ClusterNode&gt; 重载列举出这些节点</para>
        /// </summary>
        /// <param name="node">随机节点</param>
        internal RedisCluster(ClusterNode node)
            : this(new List<ClusterNode> { node }, null)
        {
        }

        /// <summary>
        /// 实例化 <see cref="RedisCluster"/> 类的新实例
        /// </summary>
        internal RedisCluster(IEnumerable<ClusterNode> source)
            : this(source, null)
        {
        }

        /// <summary>
        /// 实例化 <see cref="RedisCluster"/> 类的新实例
        /// </summary>
        internal RedisCluster(IEnumerable<ClusterNode> source, RedisClientManagerConfig config)
        {
            _source = source;
            _config = config;
            this.Initialize(null);
        }

        #endregion

        #region 初始化的

        // 初始化集群管理
        void Initialize(IList<InternalClusterNode> clusterNodes = null)
        {
            // 从 redis 读取集群信息
            IList<InternalClusterNode> nodes = clusterNodes == null ? RedisCluster.ReadClusterNodes(_source) : clusterNodes;

            // 生成主节点，每个主节点的 slot 对应一个REDIS客户端缓冲池管理器
            IList<InternalClusterNode> masters = null;
            IDictionary<int, PooledRedisClientManager> managers = null;
            foreach (var n in nodes)
            {
                // 节点无效或者
                if (!(n.IsMater &&
                    !string.IsNullOrEmpty(n.Host) &&
                    string.IsNullOrEmpty(n.NodeFlag) &&
                    (string.IsNullOrEmpty(n.LinkState) || n.LinkState == InternalClusterNode.CONNECTED))) continue;

                n.SlaveNodes = nodes.Where(x => x.MasterNodeId == n.NodeId);
                if (masters == null)
                    masters = new List<InternalClusterNode>();
                masters.Add(n);

                // 用每一个主节点的哈希槽做键，导入REDIS客户端缓冲池管理器
                // 然后，方法表指针（又名类型对象指针）上场，占据 4 个字节。 4 * 16384 / 1024 = 64KB
                if (managers == null)
                    managers = new Dictionary<int, PooledRedisClientManager>();

                string[] writeHosts = new[] { n.HostString };
                string[] readHosts = n.SlaveNodes.Where(n => false).Select(n => n.HostString).ToArray();
                var pool = new PooledRedisClientManager(writeHosts, readHosts, _config);
                managers.Add(n.Slot.Start, pool);
                if (n.Slot.End != null)
                {
                    // 这个范围内的哈希槽都用同一个缓冲池
                    for (int s = n.Slot.Start + 1; s <= n.Slot.End.Value; s++)
                        managers.Add(s, pool);
                }
                if (n.RestSlots != null)
                {
                    foreach (var slot in n.RestSlots)
                    {
                        managers.Add(slot.Start, pool);
                        if (slot.End != null)
                        {
                            // 这个范围内的哈希槽都用同一个缓冲池
                            for (int s = slot.Start + 1; s <= slot.End.Value; s++)
                                managers.Add(s, pool);
                        }
                    }
                }
            }

            _masters = masters;
            _redisClientManagers = managers;
            _clusterNodes = nodes != null ? nodes : null;

            if (_masters == null) _masters = new List<InternalClusterNode>(0);
            if (_clusterNodes == null) _clusterNodes = new List<InternalClusterNode>(0);
            if (_redisClientManagers == null) _redisClientManagers = new Dictionary<int, PooledRedisClientManager>(0);

            if (_masters.Count > 0)
                _source = _masters.Select(n => new ClusterNode(n.Host, n.Port, n.Password)).ToList();
        }

        // 重新刷新集群信息
        private bool DiscoveryNodes(IEnumerable<ClusterNode> source, RedisClientManagerConfig config)
        {
            bool lockTaken = false;
            try
            {
                // noop
                if (_isDiscoverying) { }

                Monitor.Enter(_objLock, ref lockTaken);

                _source = source;
                _config = config;
                _isDiscoverying = true;

                // 跟上次同步时间相隔 {MONITORINTERVAL} 秒钟以上才需要同步
                if ((DateTime.Now - _lastDiscoveryTime).TotalMilliseconds >= MONITORINTERVAL)
                {
                    bool isRefresh = false;
                    IList<InternalClusterNode> newNodes = RedisCluster.ReadClusterNodes(_source);
                    foreach (var node in newNodes)
                    {
                        var n = _clusterNodes.FirstOrDefault(x => x.HostString == node.HostString);
                        isRefresh =
                            n == null ||                        // 新节点                                                                
                            n.Password != node.Password ||      // 密码变了                                                                
                            n.IsMater != node.IsMater ||        // 主变从或者从变主                                                                
                            n.IsSlave != node.IsSlave ||        // 主变从或者从变主                                                                
                            n.NodeFlag != node.NodeFlag ||      // 节点标记位变了                                                                
                            n.LinkState != node.LinkState ||    // 节点状态位变了                                                                
                            n.Slot.Start != node.Slot.Start ||  // 哈希槽变了                                                                
                            n.Slot.End != node.Slot.End ||      // 哈希槽变了
                            (n.RestSlots == null && node.RestSlots != null) ||
                            (n.RestSlots != null && node.RestSlots == null);
                        if (!isRefresh && n.RestSlots != null && node.RestSlots != null)
                        {
                            var slots1 = n.RestSlots.OrderBy(x => x.Start).ToList();
                            var slots2 = node.RestSlots.OrderBy(x => x.Start).ToList();
                            for (int index = 0; index < slots1.Count; index++)
                            {
                                isRefresh =
                                    slots1[index].Start != slots2[index].Start ||   // 哈希槽变了                                                                
                                    slots1[index].End != slots2[index].End;         // 哈希槽变了
                                if (isRefresh) break;
                            }
                        }

                        if (isRefresh) break;
                    }

                    if (isRefresh)
                    {
                        // 重新初始化集群
                        this.Dispose();
                        this.Initialize(newNodes);
                        this._lastDiscoveryTime = DateTime.Now;
                    }
                }

                // 最后刷新时间在 {MONITORINTERVAL} 内，表示是最新群集信息 newest
                return (DateTime.Now - _lastDiscoveryTime).TotalMilliseconds < MONITORINTERVAL;
            }
            finally
            {
                if (lockTaken)
                {
                    _isDiscoverying = false;
                    Monitor.Exit(_objLock);
                }
            }
        }

        // 读取集群上的节点信息
        static IList<InternalClusterNode> ReadClusterNodes(IEnumerable<ClusterNode> source)
        {
            RedisClient c = null;
            StringReader reader = null;
            IList<InternalClusterNode> result = null;

            int index = 0;
            int rowCount = source.Count();

            foreach (var node in source)
            {
                try
                {
                    // 从当前节点读取REDIS集群节点信息
                    index += 1;
                    c = new RedisClient(node.Host, node.Port, node.Password);
                    RedisData data = c.RawCommand("CLUSTER".ToUtf8Bytes(), "NODES".ToUtf8Bytes());
                    string info = Encoding.UTF8.GetString(data.Data);

                    // 将读回的字符文本转成强类型节点实体
                    reader = new StringReader(info);
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (result == null) result = new List<InternalClusterNode>();
                        InternalClusterNode n = InternalClusterNode.Parse(line);
                        n.Password = node.Password;
                        result.Add(n);

                        line = reader.ReadLine();
                    }

                    // 只要任意一个节点拿到集群信息，直接退出
                    if (result != null && result.Count > 0) break;
                }
                catch (Exception ex)
                {
                    // 出现异常，如果还没到最后一个节点，则继续使用下一下节点读取集群信息
                    // 否则抛出异常
                    if (index < rowCount)
                        Thread.Sleep(100);
                    else
                        throw new RedisClusterException(ex.Message, c != null ? c.GetHostString() : string.Empty, ex);
                }
                finally
                {
                    if (reader != null) reader.Dispose();
                    if (c != null) c.Dispose();
                }
            }


            if (result == null)
                result = new List<InternalClusterNode>(0);
            return result;
        }

        #endregion

        #region 代理操作

        // 执行指定动作不返回值
        private void DoExecute(string key, Action<RedisClient> action) => this.DoExecute(() => this.GetRedisClient(key), action);

        // 执行指定动作并返回值
        private T DoExecute<T>(string key, Func<RedisClient, T> action) => this.DoExecute(() => this.GetRedisClient(key), action);

        // 执行指定动作不返回值
        private void DoExecute(int slot, Action<RedisClient> action) => this.DoExecute(() => this.GetRedisClient(slot), action);

        // 执行指定动作并返回值
        private T DoExecute<T>(int slot, Func<RedisClient, T> action) => this.DoExecute(() => this.GetRedisClient(slot), action);

        // 执行指定动作不返回值
        private void DoExecute(string[] keys, Action<RedisClient> action) => this.DoExecute(() => this.GetRedisClient(keys), action);

        // 执行指定动作并返回值
        private T DoExecute<T>(string[] keys, Func<RedisClient, T> action) => this.DoExecute(() => this.GetRedisClient(keys), action);

        // 执行指定动作不返回值
        private void DoExecute(Func<RedisClient> slot, Action<RedisClient> action, int tryTimes = 1)
        {
            RedisClient c = null;
            try
            {
                c = slot();
                action(c);
            }
            catch (Exception ex)
            {
                if (!(ex is RedisException) || tryTimes == 0) throw new RedisClusterException(ex.Message, c != null ? c.GetHostString() : string.Empty, ex);
                else
                {
                    tryTimes -= 1;
                    // 尝试重新刷新集群信息
                    bool isRefresh = DiscoveryNodes(_source, _config);
                    if (isRefresh)
                        // 集群节点有更新过，重新执行
                        this.DoExecute(slot, action, tryTimes);
                    else
                        // 集群节点未更新过，直接抛出异常
                        throw new RedisClusterException(ex.Message, c != null ? c.GetHostString() : string.Empty, ex);
                }
            }
            finally
            {
                if (c != null)
                    c.Dispose();
            }
        }

        // 执行指定动作并返回值
        private T DoExecute<T>(Func<RedisClient> slot, Func<RedisClient, T> action, int tryTimes = 1)
        {
            RedisClient c = null;
            try
            {
                c = slot();
                return action(c);
            }
            catch (Exception ex)
            {
                if (!(ex is RedisException) || tryTimes == 0) throw new RedisClusterException(ex.Message, c != null ? c.GetHostString() : string.Empty, ex);
                else
                {
                    tryTimes -= 1;
                    // 尝试重新刷新集群信息
                    bool isRefresh = DiscoveryNodes(_source, _config);
                    if (isRefresh)
                        // 集群节点有更新过，重新执行
                        return this.DoExecute(slot, action, tryTimes);
                    else
                        // 集群节点未更新过，直接抛出异常
                        throw new RedisClusterException(ex.Message, c != null ? c.GetHostString() : string.Empty, ex);
                }
            }
            finally
            {
                if (c != null)
                    c.Dispose();
            }
        }

        // 获取指定key对应的主设备节点
        private RedisClient GetRedisClient(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            int slot = CRC16.GetSlot(key);
            if (!_redisClientManagers.ContainsKey(slot))
                throw new SlotNotFoundException(string.Format("No reachable node in cluster for slot {{{0}}}", slot), slot, key);

            var pool = _redisClientManagers[slot];
            return (RedisClient)pool.GetClient();
        }

        // 获取指定key对应的主设备节点
        private RedisClient GetRedisClient(string[] keys)
        {
            //for multiple keys, only execute if they all share the same connection slot.
            if (keys == null || keys.Length == 0)
                throw new ArgumentNullException("keys");

            int slot = CRC16.GetSlot(keys[0]);
            if (keys.Length > 1)
            {
                for (int i = 1; i < keys.Length; i++)
                {
                    int nextSlot = CRC16.GetSlot(keys[i]);
                    if (slot != nextSlot)
                        throw new Exception("No way to dispatch this command to Redis Cluster because keys have different slots.");
                }
            }

            if (!_redisClientManagers.ContainsKey(slot))
                throw new SlotNotFoundException(string.Format("No reachable node in cluster for slot {{{0}}}", slot), slot, keys[0]);

            var pool = _redisClientManagers[slot];
            return (RedisClient)pool.GetClient();
        }

        // 获取指定哈希槽对应的主设备节点
        private RedisClient GetRedisClient(int slot)
        {
            if (!_redisClientManagers.ContainsKey(slot))
                throw new SlotNotFoundException(string.Format("No reachable node in cluster for slot {{{0}}}", slot), slot, string.Empty);

            var pool = _redisClientManagers[slot];
            return (RedisClient)pool.GetClient();
        }

        #endregion

        #region 释放资源

        // 释放资源
        private void Dispose()
        {
            if (Interlocked.Increment(ref _disposeAttempts) > 1)
                return;

            if (_redisClientManagers != null)
            {
                foreach (var m in _redisClientManagers)
                    m.Value.Dispose();
            }

            _masters.Clear();
            _redisClientManagers.Clear();
            _clusterNodes.Clear();
        }

        #endregion

        // JedisClusterInfoCache.renewClusterSlots line 118 bug?
        // JedisCluster.keys line 1418 bug?
    }
}
