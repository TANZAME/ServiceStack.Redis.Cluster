using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// REDIS 哈希槽找不到
    /// </summary>
    public class SlotNotFoundException : RedisException
    {
        /// <summary>
        /// REDIS 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 哈希槽
        /// </summary>
        public int Slot { get; private set; }

        /// <summary>
        /// 实例化 <see cref="SlotNotFoundException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="slot">哈希槽</param>
        /// <param name="key">REDIS 键</param>
        public SlotNotFoundException(string message, int slot, string key)
               : base(message)
        {
            this.Slot = slot;
            this.Key = key;
        }

        /// <summary>
        /// 实例化 <see cref="RedisClusterException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="slot">哈希槽</param>
        /// <param name="key">REDIS 键</param>
        /// <param name="innerException">内部异常</param>
        public SlotNotFoundException(string message, int slot, string key, Exception innerException)
            : base(message, innerException)
        {
            this.Slot = slot;
            this.Key = key;
        }
    }
}
