
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceStack.Redis;

namespace ServiceStack.Redis.FormUI
{
    /// <summary>
    /// Redis 客户端管理器
    /// </summary>
    public class RedisClientManager
    {
        // 注：RedisClient的单例模式在集群的场景下并不适用
        // Hash 结构主要用来存储实体类型
        // List 链表，做消息队列
        // Set  去重后的链表

        // redis配置文件信息 password@ip:port
        static RedisConfig _config = RedisConfig.GetConfig();
        // 客户端连接池
        static PooledRedisClientManager _pool;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisClientManager()
        {
            string[] writeHosts = _config.WriteServerConnString.Split(',');
            string[] readHosts = _config.ReadServerConnString.Split(',');

            RedisClientManagerConfig config = new RedisClientManagerConfig();
            config.MaxWritePoolSize = _config.MaxWritePoolSize;
            config.MaxReadPoolSize = _config.MaxReadPoolSize;
            config.AutoStart = _config.AutoStart;
            config.DefaultDb = _config.DefaultDb;

            _pool = new PooledRedisClientManager(writeHosts, readHosts, config);
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static RedisClient GetClient()
        {
            return (RedisClient)_pool.GetClient();
        }
    }
}
