using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// Reids 集群操作客户端
    /// </summary>
    public partial class RedisCluster
    {
        // key 操作封装

        /// <summary>
        /// 检查给定 key 是否存在。若 key 存在，返回 1 ，否则返回 0 
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long Exists(string key) => this.DoExecute(key, c => c.Exists(key));

        /// <summary>
        /// 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
        /// <para> 当生存时间移除成功时，返回 1 。如果 key 不存在或 key 没有设置生存时间，返回 0 。</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Persist(string key) => this.DoExecute(key, c => c.Persist(key));

        /// <summary>
        /// 返回 key 所储存的值的类型
        /// <para>返回：none (key不存在)，string (字符串)，list(列表)，set(集合)，zset(有序集)，hash(哈希表)</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string Type(string key) => this.DoExecute(key, c => c.Type(key));

        /// <summary>
        /// 从内部察看给定 key 的 Redis 对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string Object(string key) => throw new NotImplementedException("RedisCluster.Object not implemented.");

        /// <summary>
        /// 序列化给定 key ，并返回被序列化的值，使用 RESTORE 命令可以将这个值反序列化为 Redis 键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public byte[] Dump(string key) => this.DoExecute(key, c => c.Dump(key));

        /// <summary>
        /// 反序列化给定的序列化值，并将它和给定的 key 关联。
        /// 如果反序列化成功那么返回 OK ，否则返回一个错误。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expireMs">过期毫秒数</param>
        /// <param name="dumpValue">dump 命令序列化后的值</param>
        /// <returns></returns>
        public byte[] Restore(string key, long expireMs, byte[] dumpValue) => this.DoExecute(key, c => c.Restore(key, expireMs, dumpValue));

        /// <summary>
        /// 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="seconds">过期时间</param>
        /// <returns></returns>
        public bool Expire(string key, int seconds) => this.DoExecute(key, c => c.Expire(key, seconds));

        /// <summary>
        /// 为给定 key 设置生存时间（毫秒）做单位，当 key 过期时(生存时间为 0 )，它会被自动删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="ttlMs">过期时间</param>
        /// <returns></returns>
        public bool PExpire(string key, long ttlMs) => this.DoExecute(key, c => c.PExpire(key, ttlMs));

        /// <summary>
        /// 为给定 key 设置生存时间（秒）做单位，当 key 过期时(生存时间为 0 )，它会被自动删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="unixTime">UNIX 时间戳(unix timestamp)</param>
        /// <returns></returns>
        public bool ExpireAt(string key, long unixTime) => this.DoExecute(key, c => c.ExpireAt(key, unixTime));

        /// <summary>
        /// 为给定 key 设置生存时间（毫秒）做单位，当 key 过期时(生存时间为 0 )，它会被自动删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="unixTime">UNIX 时间戳(unix timestamp)</param>
        /// <returns></returns>
        public bool PExpireAt(string key, long unixTime) => this.DoExecute(key, c => c.PExpireAt(key, unixTime));

        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long Ttl(string key) => this.DoExecute(key, c => c.Ttl(key));

        /// <summary>
        /// 以毫秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long PTtl(string key) => this.DoExecute(key, c => c.PTtl(key));

        /// <summary>
        /// 删除给定的一个 key，返回被删除的 key 的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long Del(string key) => this.DoExecute(key, c => c.Del(key));

        /// <summary>
        /// 查找所有符合给定模式 pattern 的 key
        /// 返回集群所有节点的 keys 之和
        /// </summary>
        /// <param name="pattern">匹配模式</param>
        /// <returns></returns>
        public IList<string> Keys(string pattern)
        {
            IList<string> result = null;
            foreach (var n in _masters)
            {
                var bytes = this.DoExecute(n.Slot.Start, c => c.Keys(pattern));
                if (bytes != null)
                {
                    if (result == null) result = new List<string>();
                    for (var index = 0; index < bytes.Length; index++)
                        result.Add(bytes[index].FromUtf8Bytes());
                }
            }

            return result;
        }

        /// <summary>
        /// 查找指定节点所有符合给定模式 pattern 的 key
        /// </summary>
        /// <param name="src">主设备节点</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns></returns>
        public IList<string> Keys(ClusterNode src, string pattern)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            IList<string> result = null;
            var bytes = this.DoExecute(node.Slot.Start, c => c.Keys(pattern));
            if (bytes != null)
            {
                if (result == null) result = new List<string>();
                for (var index = 0; index < bytes.Length; index++)
                    result.Add(bytes[index].FromUtf8Bytes());
            }

            return result;
        }

        /// <summary>
        /// 删除给定的一个或多个 key，不存在的 key 会被忽略，返回被删除的 key 的数量
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        public long Del(params string[] keys) => this.DoExecute(keys, c => c.Del(keys));

        /// <summary>
        /// 将 key 原子性地从当前实例传送到目标实例的指定数据库上，一旦传送成功， key 保证会出现在目标实例上，而当前实例上的 key 会被删除
        /// </summary>
        /// <param name="host">目标主机</param>
        /// <param name="port">目标主机端口</param>
        /// <param name="key">键</param>
        /// <param name="destinationDb">目标数据库</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns></returns>
        public void Migrate(string host, int port, string key, int destinationDb, long timeoutMs)
        {
            this.DoExecute(key, c => c.Migrate(host, port, key, destinationDb, timeoutMs));
        }

        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">目标数据库</param>
        /// <returns></returns>
        public bool Move(string key, int db) => this.DoExecute(key, c => c.Move(key, db));

        /// <summary>
        /// 随机取一个随机返回(不删除)一个 key 
        /// </summary>
        /// <returns></returns>
        public string RandomKey()
        {
            int index = _rd.Next(0, _masters.Count - 1);
            return this.DoExecute(_masters[index].Slot.Start, c => c.RandomKey());
        }

        /// <summary>
        /// 从指定节点随机返回(不删除)一个 key 
        /// </summary>
        /// <returns></returns>
        public string RandomKey(ClusterNode src)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            return this.DoExecute(node.Slot.Start, c => c.RandomKey());
        }

        /// <summary>
        /// 将 key 改名为 newkey
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">新键</param>
        /// <returns></returns>
        public void Rename(string key, string newKey) => this.DoExecute(key, c => c.Rename(key, newKey));

        /// <summary>
        /// 当且仅当 newkey 不存在时，将 key 改名为 newkey
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">新键</param>
        /// <returns></returns>
        public bool RenameNx(string key, string newKey) => this.DoExecute(key, c => c.RenameNx(key, newKey));

        /// <summary>
        /// SCAN 命令是一个基于游标的迭代器（cursor based iterator）： SCAN 命令每次被调用之后， 都会向用户返回一个新的游标。
        /// 用户在下次迭代时需要使用这个新游标作为 SCAN 命令的游标参数， 以此来延续之前的迭代过程。
        /// 当 SCAN 命令的游标参数被设置为 0 时， 服务器将开始一次新的迭代， 而当服务器向用户返回值为 0 的游标时， 表示迭代已结束
        /// </summary>
        /// <param name="src">主设备节点</param>
        /// <param name="cursor">游标</param>
        /// <param name="count">每次返回的键数量</param>
        /// <param name="match">匹配模式</param>
        /// <returns></returns>
        public ScanResult Scan(ClusterNode src, ulong cursor, int count = 10, string match = null)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            return this.DoExecute(node.Slot.Start, c => c.Scan(cursor, count, match));
        }

        // 检查给定节点是否主节点
        private InternalClusterNode CheckMasterNode(ClusterNode src)
        {
            if (src == null)
                throw new ArgumentNullException("node");

            string hostString = string.Format("{0}:{1}", src.Host, src.Port);
            if (!string.IsNullOrEmpty(src.Password)) hostString = string.Format("{0}@{1}", src.Password, hostString);
            var node = _clusterNodes.FirstOrDefault(x => x.HostString == hostString);
            if (node == null)
                throw new KeyNotFoundException(string.Format("node not found in cluster => {0}", node.HostString));

            if (!node.IsMater)
                throw new ArgumentException(string.Format("it is not a master node => {0}", node.HostString));

            return node;
        }
    }
}
