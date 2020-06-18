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

        // 集合操作封装

        /// <summary>
        /// 返回集合 key 中的所有成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <returns></returns>
        public byte[][] SMembers(string setId) => this.DoExecute(setId, c => c.SMembers(setId));

        /// <summary>
        /// 将一个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long SAdd(string setId, byte[] value) => this.DoExecute(setId, c => c.SAdd(setId, value));

        /// <summary>
        /// 将一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="values">一个或多个集合元素</param>
        /// <returns></returns>
        public long SAdd(string setId, byte[][] values) => this.DoExecute(setId, c => c.SAdd(setId, values));

        /// <summary>
        /// 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long SRem(string setId, byte[] value) => this.DoExecute(setId, c => c.SRem(setId, value));

        /// <summary>
        /// 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="values">多个 member 元素</param>
        /// <returns></returns>
        public long SRem(string setId, byte[][] values) => this.DoExecute(setId, c => c.SRem(setId, values));

        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <returns></returns>
        public byte[] SPop(string setId) => this.DoExecute(setId, c => c.SPop(setId));

        /// <summary>
        /// 移除并返回集合中的 N 个随机元素
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="count">移除元素数量</param>
        /// <returns></returns>
        public byte[][] SPop(string setId, int count) => this.DoExecute(setId, c => c.SPop(setId, count));

        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <param name="source">源集合</param>
        /// <param name="destination">目标集合</param>
        /// <param name="value">member 元素</param>
        public void SMove(string source, string destination, byte[] value) => this.DoExecute(new[] { source, destination }, c => c.SMove(source, destination, value));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <returns></returns>
        public long SCard(string setId) => this.DoExecute(setId, c => c.SCard(setId));

        /// <summary>
        /// 判断 member 元素是否集合 key 的成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long SIsMember(string setId, byte[] value) => this.DoExecute(setId, c => c.SIsMember(setId, value));

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合的交集
        /// </summary>
        /// <param name="setIds">多个集合ID</param>
        /// <returns></returns>
        public byte[][] SInter(params string[] setIds) => this.DoExecute(setIds, c => c.SInter(setIds));

        /// <summary>
        /// 将给定集合的交集结果保存到 destination 集合，而不是简单地返回结果集
        /// </summary>
        /// <param name="destination">目标集合</param>
        /// <param name="setIds">多个集合ID</param>
        public void SInterStore(string destination, params string[] setIds)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            this.DoExecute(keys.ToArray(), c => c.SInterStore(destination, setIds));
        }

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合的并集。
        /// </summary>
        /// <param name="setIds">多个集合ID</param>
        /// <returns></returns>
        public byte[][] SUnion(params string[] setIds) => this.DoExecute(setIds, c => c.SUnion(setIds));

        /// <summary>
        /// 将给定集合的并集结果保存到 destination 集合，而不是简单地返回结果集
        /// </summary>
        /// <param name="destination">目标集合</param>
        /// <param name="setIds">多个集合ID</param>
        public void SUnionStore(string destination, params string[] setIds)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            this.DoExecute(keys.ToArray(), c => c.SUnionStore(destination, setIds));
        }

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合之间的差集
        /// </summary>
        /// <param name="source">源集合</param>
        /// <param name="setIds">多个集合ID</param>
        /// <returns></returns>
        public byte[][] SDiff(string source, params string[] setIds)
        {
            var keys = new List<string> { source };
            if (setIds != null) keys.AddRange(setIds);
            return this.DoExecute(keys.ToArray(), c => c.SDiff(source, setIds));
        }

        /// <summary>
        /// 将给定集合的差集结果保存到 destination 集合，而不是简单地返回结果集
        /// </summary>
        /// <param name="destination">目标集合</param>
        /// <param name="source">源集合</param>
        /// <param name="setIds">多个集合ID</param>
        public void SDiffStore(string destination, string source, params string[] setIds)
        {
            var keys = new List<string> { destination, source };
            if (setIds != null) keys.AddRange(setIds);
            this.DoExecute(keys.ToArray(), c => c.SDiffStore(destination, source, setIds));
        }

        /// <summary>
        /// 返回集合中的一个随机元素
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <returns></returns>
        public byte[] SRandMember(string setId) => this.DoExecute(setId, c => c.SRandMember(setId));

        /// <summary>
        /// 返回集合中的 n 个随机元素
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="count">
        /// 如果 count 为正数，且小于集合基数，那么命令返回一个包含 count 个元素的数组，数组中的元素各不相同。如果 count 大于等于集合基数，那么返回整个集合。
        /// 如果 count 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 count 的绝对值。
        /// </param>
        /// <returns></returns>
        public byte[][] SRandMember(string setId, int count) => this.DoExecute(setId, c => c.SRandMember(setId, count));
    }
}
