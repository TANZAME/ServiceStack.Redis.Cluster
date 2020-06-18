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
        // 其它操作封装

        #region Server

        /// <summary>
        /// 清空所有节点数据(删除所有数据库的所有 key )
        /// </summary>
        /// <returns></returns>
        public void FlushAll()
        {
            foreach (var node in _masters)
                this.DoExecute(node.Slot.Start, c => c.FlushAll());
        }

        /// <summary>
        /// 清空指定节点数据(删除所有数据库的所有 key )
        /// </summary>
        /// <param name="src">主设备节点</param>
        /// <returns></returns>
        public void FlushAll(ClusterNode src)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            this.DoExecute(node.Slot.Start, c => c.FlushAll());
        }

        /// <summary>
        /// 在后台异步(Asynchronously)保存所有节点的数据到磁盘。
        /// BGSAVE 命令执行之后立即返回 OK ，然后 Redis fork 出一个新子进程，原来的 Redis 进程(父进程)继续处理客户端请求，而子进程则负责将数据保存到磁盘，然后退出
        /// </summary>
        /// <returns></returns>
        public void BgSave()
        {
            foreach (var node in _masters)
                this.DoExecute(node.Slot.Start, c => c.FlushAll());
        }

        /// <summary>
        /// 在后台异步(Asynchronously)保存当前节点的数据到磁盘。
        /// BGSAVE 命令执行之后立即返回 OK ，然后 Redis fork 出一个新子进程，原来的 Redis 进程(父进程)继续处理客户端请求，而子进程则负责将数据保存到磁盘，然后退出
        /// </summary>
        /// <param name="src">主设备节点</param>
        /// <returns></returns>
        public void BgSave(ClusterNode src)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            this.DoExecute(node.Slot.Start, c => c.BgSave());
        }

        /// <summary>
        /// 调整整个集群 Redis 服务器的配置(configuration)而无须重启
        /// </summary>
        /// <param name="parameter">配置名称</param>
        /// <param name="value">配置内容</param>
        /// <returns></returns>
        public void ConfigSet(string parameter, byte[] value)
        {
            foreach (var node in _masters)
                this.DoExecute(node.Slot.Start, c => c.ConfigSet(parameter, value));
        }

        /// <summary>
        /// 调整指定 Redis 服务器的配置(configuration)而无须重启
        /// </summary>
        /// <param name="src">主设备节点</param>
        /// <param name="parameter">配置名称</param>
        /// <param name="value">配置内容</param>
        /// <returns></returns>
        public void ConfigSet(ClusterNode src, string parameter, byte[] value)
        {
            InternalClusterNode node = this.CheckMasterNode(src);
            this.DoExecute(node.Slot.Start, c => c.ConfigSet(parameter, value));
        }

        #endregion
    }
}
