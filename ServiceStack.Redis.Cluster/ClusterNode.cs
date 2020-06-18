
namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// 集群节点信息
    /// </summary>
    public class ClusterNode
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 实例化 <see cref="ClusterNode"/> 类的新实例
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="password">密码</param>
        public ClusterNode(string host, int port, string password = null)
        {
            this.Host = host;
            this.Port = port;
            this.Password = password;
        }
    }
}