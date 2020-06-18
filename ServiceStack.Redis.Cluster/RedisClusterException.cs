using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// REDIS 集群操作异常
    /// </summary>
    public class RedisClusterException : RedisException
    {
        /// <summary>
        /// 产生异常的节点
        /// </summary>
        public string HostString { get; private set; }

        /// <summary>
        /// 实例化 <see cref="RedisClusterException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="hostString">产生异常的节点</param>
        public RedisClusterException(string message,string hostString)
               : base(message)
        {
            this.HostString = hostString;
        }

        /// <summary>
        /// 实例化 <see cref="RedisClusterException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="hostString">产生异常的节点</param>
        /// <param name="innerException">内部异常</param>
        public RedisClusterException(string message, string hostString, Exception innerException)
            : base(message, innerException)
        {
            this.HostString = hostString;
        }
    }
}
