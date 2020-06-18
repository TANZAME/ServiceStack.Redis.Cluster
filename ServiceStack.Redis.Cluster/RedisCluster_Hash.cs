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

        // 哈希表操作封装

        /// <summary>
        /// 将哈希表 key 中的域 key 的值设为 value
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long HSet(string hashId, byte[] key, byte[] value) => this.DoExecute(hashId, c => c.HSet(hashId, key, value));

        /// <summary>
        /// 将哈希表 key 中的域 key 的值设置为 value ，当且仅当域 key 不存在
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long HSetNX(string hashId, byte[] key, byte[] value) => this.DoExecute(hashId, c => c.HSetNX(hashId, key, value));

        /// <summary>
        /// 同时将多个 key-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="keys">字段域</param>
        /// <param name="values">值</param>
        public void HMSet(string hashId, byte[][] keys, byte[][] values) => this.DoExecute(hashId, c => c.HMSet(hashId, keys, values));

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public long HIncrby(string hashId, string key, int incrementBy) => this.DoExecute(hashId, c => c.HIncrby(hashId, key.ToUtf8Bytes(), incrementBy));

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public long HIncrby(string hashId, byte[] key, int incrementBy) => this.DoExecute(hashId, c => c.HIncrby(hashId, key, incrementBy));

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public long HIncrby(string hashId, string key, long incrementBy) => this.DoExecute(hashId, c => c.HIncrby(hashId, key.ToUtf8Bytes(), incrementBy));

        /// <summary>
        /// 为哈希表 key 中的域 field 的值加上增量 increment 
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public long HIncrby(string hashId, byte[] key, long incrementBy) => this.DoExecute(hashId, c => c.HIncrby(hashId, key, incrementBy));

        /// <summary>
        /// 为哈希表 key 中的域 field 加上浮点数增量 increment
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public double HIncrbyFloat(string hashId, string key, double incrementBy) => this.DoExecute(hashId, c => c.HIncrbyFloat(hashId, key.ToUtf8Bytes(), incrementBy));

        /// <summary>
        /// 为哈希表 key 中的域 field 加上浮点数增量 increment
        /// </summary>
        /// <param name="hashId"></param>
        /// <param name="key">字段域</param>
        /// <param name="incrementBy">增量</param>
        /// <returns></returns>
        public double HIncrbyFloat(string hashId, byte[] key, double incrementBy) => this.DoExecute(hashId, c => c.HIncrbyFloat(hashId, key, incrementBy));

        /// <summary>
        /// 返回哈希表 key 中给定域 key 的值
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public byte[] HGet(string hashId, string key) => this.DoExecute(hashId, c => c.HGet(hashId, key.ToUtf8Bytes()));

        /// <summary>
        /// 返回哈希表 key 中给定域 key 的值
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public byte[] HGet(string hashId, byte[] key) => this.DoExecute(hashId, c => c.HGet(hashId, key));

        /// <summary>
        /// 返回哈希表 key 中，所有的域和值
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <returns></returns>
        public byte[][] HGetAll(string hashId) => this.DoExecute(hashId, c => c.HGetAll(hashId));

        /// <summary>
        /// 返回哈希表 key 中，一个或多个给定域的值
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="keys">字段域</param>
        /// <returns></returns>
        public byte[][] HMGet(string hashId, params byte[][] keys) => this.DoExecute(hashId, c => c.HMGet(hashId, keys));


        /// <summary>
        /// 删除哈希表 key 中的一个指定域
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public long HDel(string hashId, string key) => this.DoExecute(hashId, c => c.HDel(hashId, key.ToUtf8Bytes()));

        /// <summary>
        /// 删除哈希表 key 中的一个指定域
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public long HDel(string hashId, byte[] key) => this.DoExecute(hashId, c => c.HDel(hashId, key));

        /// <summary>
        /// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="keys">一个或多个指定域</param>
        /// <returns></returns>
        public long HDel(string hashId, byte[][] keys) => this.DoExecute(hashId, c => c.HDel(hashId, keys));

        /// <summary>
        /// 查看哈希表 key 中，给定域 key 是否存在
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public long HExists(string hashId, string key) => this.DoExecute(hashId, c => c.HExists(hashId, key.ToUtf8Bytes()));

        /// <summary>
        /// 查看哈希表 key 中，给定域 key 是否存在
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <param name="key">字段域</param>
        /// <returns></returns>
        public long HExists(string hashId, byte[] key) => this.DoExecute(hashId, c => c.HExists(hashId, key));

        /// <summary>
        /// 返回哈希表 key 中域的数量
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <returns></returns>
        public long HLen(string hashId) => this.DoExecute(hashId, c => c.HLen(hashId));

        /// <summary>
        /// 返回哈希表 key 中的所有域
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <returns></returns>
        public byte[][] HKeys(string hashId) => this.DoExecute(hashId, c => c.HKeys(hashId));

        /// <summary>
        /// 返回哈希表 key 中所有域的值
        /// </summary>
        /// <param name="hashId">哈希表</param>
        /// <returns></returns>
        public byte[][] HVals(string hashId) => this.DoExecute(hashId, c => c.HVals(hashId));

        // 消息队列

        //public long Publish(string toChannel, byte[] message)
        //{
        //    return SendExpectLong(Commands.Publish, toChannel.ToUtf8Bytes(), message);
        //}

        //public byte[][] ReceiveMessages()
        //{
        //    return ReadMultiData();
        //}

        //public virtual IRedisSubscription CreateSubscription()
        //{
        //    return new RedisSubscription(this);
        //}

        //public byte[][] Subscribe(params string[] toChannels)
        //{
        //    if (toChannels.Length == 0)
        //        throw new ArgumentNullException(nameof(toChannels));

        //    var cmdWithArgs = MergeCommandWithArgs(Commands.Subscribe, toChannels);
        //    return SendExpectMultiData(cmdWithArgs);
        //}

        //public byte[][] UnSubscribe(params string[] fromChannels)
        //{
        //    var cmdWithArgs = MergeCommandWithArgs(Commands.UnSubscribe, fromChannels);
        //    return SendExpectMultiData(cmdWithArgs);
        //}

        //public byte[][] PSubscribe(params string[] toChannelsMatchingPatterns)
        //{
        //    if (toChannelsMatchingPatterns.Length == 0)
        //        throw new ArgumentNullException(nameof(toChannelsMatchingPatterns));

        //    var cmdWithArgs = MergeCommandWithArgs(Commands.PSubscribe, toChannelsMatchingPatterns);
        //    return SendExpectMultiData(cmdWithArgs);
        //}

        //public byte[][] PUnSubscribe(params string[] fromChannelsMatchingPatterns)
        //{
        //    var cmdWithArgs = MergeCommandWithArgs(Commands.PUnSubscribe, fromChannelsMatchingPatterns);
        //    return SendExpectMultiData(cmdWithArgs);
        //}

        //public RedisPipelineCommand CreatePipelineCommand()
        //{
        //    AssertConnectedSocket();
        //    return new RedisPipelineCommand(this);
        //}

    }
}
