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
        // 字符串类型操作封装

        /// <summary>
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾
        /// <para>返回 追加 value 之后， key 中字符串的长度</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">将要追加的值</param>
        /// <returns></returns>
        public long Append(string key, byte[] value) => this.DoExecute(key, c => c.Append(key, value));

        /// <summary>
        /// 计算给定字符串中，被设置为 1 的比特位的数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long BitCount(string key) => this.DoExecute(key, c => c.BitCount(key));

        /// <summary>
        /// 对一个或多个保存二进制位的字符串 key 进行位元操作，并将结果保存到 destkey 上
        /// </summary>
        /// <param name="operation">AND 、 OR 、 NOT 、 XOR 这四种操作中的任意一种</param>
        /// <param name="destkey">目标键</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long BitTop(string operation, string destkey, string key) => throw new NotImplementedException("RedisCluster.BitTop not implemented.");

        /// <summary>
        /// 将 key 中储存的数字值减一
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long Decr(string key) => this.DoExecute(key, c => c.Decr(key));

        /// <summary>
        /// 将 key 所储存的值减去减量 decrement 
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="decrement">减量</param>
        /// <returns></returns>
        public long DecrBy(string key, int decrement) => this.DoExecute(key, c => c.DecrBy(key, decrement));

        /// <summary>
        /// 返回 key 所关联的字符串值，如果 key 不存在那么返回特殊值 null
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public byte[] Get(string key) => this.DoExecute<byte[]>(key, c => c.Get(key));

        /// <summary>
        /// 返回 key 所关联的字符串值，如果 key 不存在那么返回特殊值 null
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key) => this.DoExecute<T>(key, c => c.Get<T>(key));

        /// <summary>
        /// 对 key 所储存的字符串值，获取指定偏移量上的位(bit)
        /// <para>当 offset 比字符串值的长度大，或者 key 不存在时，返回 0</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public long GetBit(string key, int offset) => this.DoExecute(key, c => c.GetBit(key, offset));

        /// <summary>
        /// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)
        /// <para>负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="start ">起始偏移量</param>
        /// <param name="end ">结束偏移量</param>
        /// <returns></returns>
        public byte[] GetRange(string key, int start, int end) => this.DoExecute(key, c => c.GetRange(key, start, end));

        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">新值</param>
        /// <returns></returns>
        public byte[] GetSet(string key, byte[] value) => this.DoExecute<byte[]>(key, c => c.GetSet(key, value));

        /// <summary>
        /// 将 key 中储存的数字值增一
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long DIncrecr(string key) => this.DoExecute(key, c => c.Incr(key));

        /// <summary>
        /// 将 key 所储存的值加上增量 increment 
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="decrement">减量</param>
        /// <returns></returns>
        public long IncrBy(string key, int decrement) => this.DoExecute(key, c => c.IncrBy(key, decrement));

        /// <summary>
        /// 为 key 中所储存的值加上浮点数增量 increment
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="decrement">减量</param>
        /// <returns></returns>
        public double IncrByFloat(string key, double decrement) => this.DoExecute(key, c => c.IncrByFloat(key, decrement));

        /// <summary>
        /// 返回所有(一个或多个)给定 key 的值
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        public byte[][] MGet(params string[] keys) => this.DoExecute(keys, c => c.MGet(keys));

        /// <summary>
        /// 同时设置一个或多个 key-value 对（原子性(atomic)操作）
        /// </summary>
        /// <param name="keys">键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public void MSet(string[] keys, byte[][] values) => this.DoExecute(keys, c => c.MSet(keys, values));

        /// <summary>
        /// 同时设置一个或多个 key-value 对，当且仅当所有给定 key 都不存在
        /// </summary>
        /// <param name="keys">键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public void MSetNx(string[] keys, byte[][] values) => this.DoExecute(keys, c => c.MSetNx(keys, values));

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value) => this.DoExecute<bool>(key, c => c.Set<T>(key, value));

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresIn">设置键的过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn) => this.DoExecute<bool>(key, c => c.Set<T>(key, value, expiresIn));

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresAt">设置键的过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTime expiresAt) => this.DoExecute<bool>(key, c => c.Set<T>(key, value, expiresAt));

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void Set(string key, byte[] value) => this.DoExecute<bool>(key, c => c.Set(key, value));

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expirySeconds">过期秒数</param>
        /// <param name="expiryMs">过期毫秒数。expirySeconds 和 expiryMs 两者只设置其中一个即可</param>
        /// <returns></returns>
        public void Set(string key, byte[] value, int expirySeconds, long expiryMs = 0)
        {
            this.DoExecute(key, c => c.Set(key, value, expirySeconds, expiryMs));
        }

        /// <summary>
        /// 将字符串值 value 关联到 key
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="exists">true表示 XX（键存在才设置），false 表示 NX（键不存在才设置）</param>
        /// <param name="expirySeconds">过期秒数</param>
        /// <param name="expiryMs">过期毫秒数。expirySeconds 和 expiryMs 两者只设置其中一个即可</param>
        /// <returns></returns>
        public void Set(string key, byte[] value, bool exists, int expirySeconds = 0, long expiryMs = 0)
        {
            this.DoExecute<bool>(key, c => c.Set(key, value, exists, expirySeconds, expiryMs));
        }

        /// <summary>
        /// 对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)
        /// <para>位的设置或清除取决于 value 参数，可以是 0 也可以是 1</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <param name="bit">位，0或1</param>
        /// <returns></returns>
        public long SetBit(string key, int offset, int bit) => this.DoExecute(key, c => c.SetBit(key, offset, bit));

        /// <summary>
        /// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="expireInSeconds">过期时间</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public void SetEx(string key, int expireInSeconds, byte[] value) => this.DoExecute(key, c => c.SetEx(key, expireInSeconds, value));

        /// <summary>
        /// 将 key 的值设为 value ，当且仅当 key 不存在
        /// <para>成功返回 1，错误返回 0</para>
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long SetNX(string key, byte[] value) => this.DoExecute(key, c => c.SetNX(key, value));

        /// <summary>
        /// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long SetRange(string key, int offset, byte[] value) => this.DoExecute(key, c => c.SetRange(key, offset, value));

        /// <summary>
        /// 返回 key 所储存的字符串值的长度
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public long StrLen(string key) => this.DoExecute(key, c => c.StrLen(key));
    }
}
