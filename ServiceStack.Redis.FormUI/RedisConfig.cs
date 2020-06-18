using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.Redis.FormUI
{
    /// <summary>
    /// Redis 配置类
    /// </summary>
    public sealed class RedisConfig : ConfigurationSection
    {
        /// <summary>
        /// 主机地址{master},格式：password@host:port，若无密码，则为 host:port
        /// </summary>
        [ConfigurationProperty("writeServerConnString", IsRequired = false)]
        public string WriteServerConnString
        {
            get
            {
                return (string)base["writeServerConnString"];
            }
            set
            {
                base["writeServerConnString"] = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 从机地址{slave},格式：password@host:port，若无密码，则为 host:port
        /// </summary>
        [ConfigurationProperty("readServerConnString", IsRequired = false)]
        public string ReadServerConnString
        {
            get
            {
                return (string)base["readServerConnString"];
            }
            set
            {
                base["readServerConnString"] = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 最大写链接数
        /// </summary>
        [ConfigurationProperty("maxWritePoolSize", IsRequired = false, DefaultValue = 5)]
        public int MaxWritePoolSize
        {
            get
            {
                int _maxWritePoolSize = (int)base["maxWritePoolSize"];
                return _maxWritePoolSize > 0 ? _maxWritePoolSize : 5;
            }
            set
            {
                base["maxWritePoolSize"] = value;
            }
        }

        /// <summary>
        /// 最大读链接数
        /// </summary>
        [ConfigurationProperty("maxReadPoolSize", IsRequired = false, DefaultValue = 5)]
        public int MaxReadPoolSize
        {
            get
            {
                int _maxReadPoolSize = (int)base["maxReadPoolSize"];
                return _maxReadPoolSize > 0 ? _maxReadPoolSize : 5;
            }
            set
            {
                base["maxReadPoolSize"] = value;
            }
        }

        /// <summary>
        /// 默认数据库
        /// </summary>
        [ConfigurationProperty("defaultDb", IsRequired = false)]
        public int? DefaultDb
        {
            get
            {
                int? _defaultDb = (int?)base["defaultDb"];
                return _defaultDb;
            }
            set
            {
                base["defaultDb"] = value;
            }
        }

        /// <summary>
        /// 自动重启
        /// </summary>
        [ConfigurationProperty("autoStart", IsRequired = false, DefaultValue = true)]
        public bool AutoStart
        {
            get
            {
                return (bool)base["autoStart"];
            }
            set
            {
                base["autoStart"] = value;
            }
        }

        /// <summary>
        /// 本地缓存到期时间，单位:秒
        /// </summary>
        [ConfigurationProperty("localCacheTime", IsRequired = false, DefaultValue = 36000)]
        public int LocalCacheTime
        {
            get
            {
                return (int)base["localCacheTime"];
            }
            set
            {
                base["localCacheTime"] = value;
            }
        }

        /// <summary>
        /// 是否记录日志,该设置仅用于排查redis运行时出现的问题,如redis工作正常,请关闭该项
        /// </summary>
        [ConfigurationProperty("recordeLog", IsRequired = false, DefaultValue = false)]
        public bool RecordeLog
        {
            get
            {
                return (bool)base["recordeLog"];
            }
            set
            {
                base["recordeLog"] = value;
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static RedisConfig GetConfig(string sectionName = "redisConfig")
        {
            RedisConfig section = (RedisConfig)ConfigurationManager.GetSection(sectionName);
            if (section == null) throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            return section;
        }
    }
}

// 参考：https://www.cnblogs.com/aspnethot/articles/1408689.html
