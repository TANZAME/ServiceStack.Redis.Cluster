using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// REDIS 集群工厂
    /// </summary>
    public class RedisClusterFactory
    {
        static RedisClusterFactory _factory = new RedisClusterFactory();
        static RedisCluster _cluster = null;

        /// <summary>
        /// Redis 集群
        /// </summary>
        public static RedisCluster Cluster
        {
            get
            {
                if (_cluster == null)
                    throw new Exception("You should call RedisClusterFactory.Configure to config cluster first.");
                else
                    return _cluster;
            }
        }

        /// <summary>
        /// 初始化 <see cref="RedisClusterFactory"/> 类的新实例
        /// </summary>
        private RedisClusterFactory()
        {
        }

        /// <summary>
        /// 配置 REDIS 集群
        /// <para>若群集中有指定 password 的节点，必须使用  IEnumerable&lt;ClusterNode&gt; 重载列举出这些节点</para>
        /// </summary>
        /// <param name="node">集群节点</param>
        /// <returns></returns>
        public static RedisCluster Configure(ClusterNode node)
        {
            return RedisClusterFactory.Configure(node, null);
        }

        /// <summary>
        /// 配置 REDIS 集群
        /// <para>若群集中有指定 password 的节点，必须使用  IEnumerable&lt;ClusterNode&gt; 重载列举出这些节点</para>
        /// </summary>
        /// <param name="node">集群节点</param>
        /// <param name="config"><see cref="RedisClientManagerConfig"/> 客户端缓冲池配置</param>
        /// <returns></returns>
        public static RedisCluster Configure(ClusterNode node, RedisClientManagerConfig config)
        {
            return RedisClusterFactory.Configure(new List<ClusterNode> { node }, config);
        }

        /// <summary>
        /// 配置 REDIS 集群
        /// </summary>
        /// <param name="nodes">集群节点</param>
        /// <param name="config"><see cref="RedisClientManagerConfig"/> 客户端缓冲池配置</param>
        /// <returns></returns>
        public static RedisCluster Configure(IEnumerable<ClusterNode> nodes, RedisClientManagerConfig config)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            if (nodes == null || nodes.Count() == 0)
                throw new ArgumentException("There is no nodes to configure cluster.");

            if (_cluster == null)
            {
                lock (_factory)
                {
                    if (_cluster == null)
                    {
                        RedisCluster c = new RedisCluster(nodes, config);
                        _cluster = c;
                    }
                }
            }

            return _cluster;
        }
    }
}
