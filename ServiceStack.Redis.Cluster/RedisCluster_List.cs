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

        // 列表表操作封装

        /// <summary>
        /// 返回或保存给定列表、集合、有序集合 key 中经过排序的元素
        /// <para>排序默认以数字作为对象，值被解释为双精度浮点数，然后进行比较</para>
        /// </summary>
        /// <param name="listOrSetId">列表或集合的键</param>
        /// <param name="sortOptions">排序参数</param>
        /// <returns></returns>
        public byte[][] Sort(string listOrSetId, SortOptions sortOptions) => this.DoExecute(listOrSetId, c => c.Sort(listOrSetId, sortOptions));

        /// <summary>
        /// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 end 指定
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="start">起始偏移量</param>
        /// <param name="end">结束偏移量</param>
        /// <returns></returns>
        public byte[][] LRange(string listId, int start, int end) => this.DoExecute(listId, c => c.LRange(listId, start, end));

        /// <summary>
        /// 将一个值 value 插入到列表 key 的表头
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long RPush(string listId, byte[] value) => this.DoExecute(listId, c => c.RPush(listId, value));

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表头
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public long RPush(string listId, byte[][] values) => this.DoExecute(listId, c => c.RPush(listId, values));

        /// <summary>
        /// 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表。
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long RPushX(string listId, byte[] value) => this.DoExecute(listId, c => c.RPushX(listId, value));

        /// <summary>
        /// 将一个值 value 插入到列表 key 的表头
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long LPush(string listId, byte[] value) => this.DoExecute(listId, c => c.LPush(listId, value));

        /// <summary>
        /// 将一个或多个值 value 插入到列表 key 的表头
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public long LPush(string listId, byte[][] values) => this.DoExecute(listId, c => c.LPush(listId, values));

        /// <summary>
        /// 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public long LPushX(string listId, byte[] value) => this.DoExecute(listId, c => c.LPushX(listId, value));

        /// <summary>
        /// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="keepStartingFrom">起始区间</param>
        /// <param name="keepEndingAt">结束区间</param>
        public void LTrim(string listId, int keepStartingFrom, int keepEndingAt) => this.DoExecute(listId, c => c.LTrim(listId, keepStartingFrom, keepEndingAt));

        /// <summary>
        /// 根据参数 count 的值，移除列表中与参数 value 相等的元素
        /// <para>
        /// count 的值可以是以下几种：
        /// count &gt; 0 : 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count 。
        /// count &lt; 0 : 从表尾开始向表头搜索，移除与 value 相等的元素，数量为 count 的绝对值。
        /// count = 0 : 移除表中所有与 value 相等的值。
        /// </para>
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="count">移除元素数量</param>
        /// <param name="value">相比较的值</param>
        /// <returns></returns>
        public long LRem(string listId, int count, byte[] value) => this.DoExecute(listId, c => c.LRem(listId, count, value));

        /// <summary>
        /// 返回列表 key 的长度
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <returns></returns>
        public long LLen(string listId) => this.DoExecute(listId, c => c.LLen(listId));

        /// <summary>
        /// 返回列表 key 中，下标为 index 的元素
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="index">下标</param>
        /// <returns></returns>
        public byte[] LIndex(string listId, int index) => this.DoExecute(listId, c => c.LIndex(listId, index));

        /// <summary>
        /// 将值 value 插入到列表 key 当中，位于值 pivot 之前或之后
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="insertBefore">true=BEFORE，false=ALFTER</param>
        /// <param name="pivot">参照物</param>
        /// <param name="value">值</param>
        public void LInsert(string listId, bool insertBefore, byte[] pivot, byte[] value) => this.DoExecute(listId, c => c.LInsert(listId, insertBefore, pivot, value));

        /// <summary>
        /// 将列表 key 下标为 index 的元素的值设置为 value 
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="index">下标</param>
        /// <param name="value">值</param>
        public void LSet(string listId, int index, byte[] value) => this.DoExecute(listId, c => c.LSet(listId, index, value));

        /// <summary>
        /// 移除并返回列表 key 的头元素
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <returns></returns>
        public byte[] LPop(string listId) => this.DoExecute(listId, c => c.LPop(listId));

        /// <summary>
        /// 移除并返回列表 key 的尾元素
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        public byte[] RPop(string listId) => this.DoExecute(listId, c => c.RPop(listId));

        /// <summary>
        /// 弹出表头元素。
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="timeout">超时时间（秒），0表示无限延长</param>
        /// <returns></returns>
        public byte[][] BLPop(string listId, int timeout) => this.DoExecute(listId, c => c.BLPop(listId, timeout));

        /// <summary>
        /// 按参数 key 的先后顺序依次检查各个列表，弹出第一个非空列表的头元素
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="listIds">列表键</param>
        /// <param name="timeout">超时时间（秒），0表示无限延长</param>
        /// <returns></returns>
        public byte[][] BLPop(string[] listIds, int timeout) => this.DoExecute(listIds, c => c.BLPop(listIds, timeout));

        /// <summary>
        /// 弹出表头元素。
        /// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPop 命令阻塞，直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="listId">列表键</param>
        /// <param name="timeout">超时时间（秒），0表示无限延长</param>
        /// <returns></returns>
        public byte[][] BRPop(string listId, int timeout) => this.DoExecute(listId, c => c.BRPop(listId, timeout));

        /// <summary>
        /// 按参数 key 的先后顺序依次检查各个列表，弹出第一个非空列表的尾部元素
        /// 它是 RPop 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止。
        /// </summary>
        /// <param name="listIds">列表键</param>
        /// <param name="timeout">超时时间（秒），0表示无限延长</param>
        /// <returns></returns>
        public byte[][] BRPop(string[] listIds, int timeout) => this.DoExecute(listIds, c => c.BRPop(listIds, timeout));

        /// <summary>
        /// 将列表 source 中的最后一个元素(尾元素)弹出，并返回给客户端，将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <returns></returns>
        public byte[] RPopLPush(string source, string destination) => this.DoExecute(new[] { source, destination }, c => c.RPopLPush(source, destination));

        /// <summary>
        /// RPopLPush 命令的阻塞版本
        /// </summary>
        /// <param name="source">源列表</param>
        /// <param name="destination">目标列表</param>
        /// <param name="timeout">超时时间（秒），0表示无限延长</param>
        /// <returns></returns>
        public byte[] BRPopLPush(string source, string destination, int timeout)
            => this.DoExecute(new[] { source, destination }, c => c.BRPopLPush(source, destination, timeout));
    }
}
