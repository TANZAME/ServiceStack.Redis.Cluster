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

        // 有序集合操作封装

        //合法的 min 和 max 参数必须包含(或者[ ， 其中(表示开区间（指定的值不会被包含在范围之内）， 而[则表示闭区间（指定的值会被包含在范围之内）。
        //特殊值 + 和 - 在 min 参数以及 max 参数中具有特殊的意义， 其中 + 表示正无限， 而 - 表示负无限。 
        //因此， 向一个所有成员的分值都相同的有序集合发送命令 ZRANGEBYLEX < zset > -+ ， 命令将返回有序集合中的所有元素。

        /// <summary>
        /// 将一个 member 元素及其 score 值加入到有序集 key 当中
        /// <para>如果 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上</para>
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="score">分数值</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long ZAdd(string setId, long score, byte[] value) => this.DoExecute(setId, c => c.ZAdd(setId, score, value));

        /// <summary>
        /// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中
        /// <para>如果 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上</para>
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="pairs">member 元素和对应分数值集合</param>
        /// <returns></returns>
        public long ZAdd(string setId, List<KeyValuePair<byte[], double>> pairs) => this.DoExecute(setId, c => c.ZAdd(setId, pairs));

        /// <summary>
        /// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中
        /// <para>如果 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上</para>
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="pairs">member 元素和对应分数值集合</param>
        /// <returns></returns>
        public long ZAdd(string setId, List<KeyValuePair<byte[], long>> pairs) => this.DoExecute(setId, c => c.ZAdd(setId, pairs));

        /// <summary>
        /// 移除有序集 key 中的一个成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long ZRem(string setId, byte[] value) => this.DoExecute(setId, c => c.ZRem(setId, value));

        /// <summary>
        /// 移除有序集 key 中的一个或多个成员，不存在的成员将被忽略
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="values">一个或多个 member 元素</param>
        /// <returns></returns>
        public long ZRem(string setId, byte[][] values) => this.DoExecute(setId, c => c.ZRem(setId, values));

        /// <summary>
        /// 为有序集 key 的成员 member 的 score 值加上增量 increment 
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="increment">增量</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public double ZIncrBy(string setId, double increment, byte[] value) => this.DoExecute(setId, c => c.ZIncrBy(setId, increment, value));

        /// <summary>
        /// 为有序集 key 的成员 member 的 score 值加上增量 increment 
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="increment">增量</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public double ZIncrBy(string setId, long increment, byte[] value) => this.DoExecute(setId, c => c.ZIncrBy(setId, increment, value));

        /// <summary>
        /// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递增(从小到大)顺序排
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long ZRank(string setId, byte[] value) => this.DoExecute(setId, c => c.ZRank(setId, value));

        /// <summary>
        /// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递减(从大到小)排序
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public long ZRevRank(string setId, byte[] value) => this.DoExecute(setId, c => c.ZRevRank(setId, value));

        /// <summary>
        /// 返回有序集 key 中指定区间内（索引）的成员，其中成员的位置按 score 值递增(从小到大)次序排列。
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public byte[][] ZRange(string setId, int start, int end) => this.DoExecute(setId, c => c.ZRange(setId, start, end));

        /// <summary>
        /// 返回有序集 key 中指定区间内（索引）的成员，其中成员的位置按 score 值递增(从小到大)次序排列。
        /// 当有序集合的所有成员都具有相同的分值时， 有序集合的元素会根据成员的字典序（lexicographical ordering）来进行排序， 
        /// 而这个命令则可以返回给定的有序集合键 key 中， 值介于 start 和 end 之间的成员。
        /// 如果有序集合里面的成员带有不同的分值， 那么命令返回的结果是未指定的（unspecified）
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRangeByLex(string setId, string start, string end, int? skip = null, int? take = null)
            => this.DoExecute(setId, c => c.ZRangeByLex(setId, start, end, skip, take));

        /// <summary>
        /// 返回有序集 key 中指定区间内（索引）的成员，其中成员的位置按 score 值递增(从小到大)次序排列。
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public byte[][] ZRangeWithScores(string setId, int start, int end) => this.DoExecute(setId, c => c.ZRangeWithScores(setId, start, end));

        /// <summary>
        /// 返回有序集 key 中指定区间内（索引）的成员，其中成员的位置按 score 值递减(从大到小)来排列
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public byte[][] ZRevRange(string setId, int start, int end) => this.DoExecute(setId, c => c.ZRevRange(setId, start, end));

        /// <summary>
        /// 返回有序集 key 中指定区间内的成员，其中成员的位置按 score 值递减(从大到小)来排列。
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public byte[][] ZRevRangeWithScores(string setId, int start, int end) => this.DoExecute(setId, c => c.ZRevRangeWithScores(setId, start, end));

        /// <summary>
        /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRangeByScore(string setId, double min, double max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRangeByScore(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRangeByScore(string setId, long min, long max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRangeByScore(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRangeByScoreWithScores(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRangeByScoreWithScores(string setId, long min, long max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRangeByScoreWithScores(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中， score 值介于 max 和 min 之间(默认包括等于 max 或 min )的所有的成员。有序集成员按 score 值递减(从大到小)的次序排列
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRevRangeByScore(string setId, double min, double max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRevRangeByScore(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中， score 值介于 max 和 min 之间(默认包括等于 max 或 min )的所有的成员。有序集成员按 score 值递减(从大到小)的次序排列
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRevRangeByScore(string setId, long min, long max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRevRangeByScore(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中， score 值介于 max 和 min 之间(默认包括等于 max 或 min )的所有的成员。有序集成员按 score 值递减(从大到小)的次序排列
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRevRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take)
            => this.DoExecute(setId, c => c.ZRevRangeByScoreWithScores(setId, min, max, skip, take));

        /// <summary>
        /// 返回有序集 key 中， score 值介于 max 和 min 之间(默认包括等于 max 或 min )的所有的成员。有序集成员按 score 值递减(从大到小)的次序排列
        /// 成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <param name="skip">返回结果起始位置</param>
        /// <param name="take">返回结果数量</param>
        /// <returns></returns>
        public byte[][] ZRevRangeByScoreWithScores(string setId, long min, long max, int? skip, int? take) =>
            this.DoExecute(setId, c => c.ZRevRangeByScoreWithScores(setId, min, max, skip, take));

        /// <summary>
        /// 移除有序集 key 中，指定排名(rank)区间内的所有成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public long ZRemRangeByRank(string setId, int start, int end) => this.DoExecute(setId, c => c.ZRemRangeByRank(setId, start, end));

        /// <summary>
        /// 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <returns></returns>
        public long ZRemRangeByScore(string setId, double min, double max) => this.DoExecute(setId, c => c.ZRemRangeByScore(setId, min, max));

        /// <summary>
        /// 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <returns></returns>
        public long ZRemRangeByScore(string setId, long min, long max) => this.DoExecute(setId, c => c.ZRemRangeByScore(setId, min, max));

        /// <summary>
        /// 对于一个所有成员的分值都相同的有序集合键 key 来说， 这个命令会移除该集合中， 成员介于 start 和 end 范围内的所有元素
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public long ZRemRangeByLex(string setId, string start, string end) => this.DoExecute(setId, c => c.ZRemRangeByLex(setId, start, end));

        /// <summary>
        /// 返回有序集 key 的基数
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <returns></returns>
        public long ZCard(string setId) => this.DoExecute(setId, c => c.ZCard(setId));

        /// <summary>
        /// 返回有序集 key 中， score 值在 min 和 max 之间(默认包括 score 值等于 min 或 max )的成员的数量
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <returns></returns>
        public long ZCount(string setId, double min, double max) => this.DoExecute(setId, c => c.ZCount(setId, min, max));

        /// <summary>
        /// 对于一个所有成员的分值都相同的有序集合键 key 来说， 这个命令会返回该集合中， 成员介于 start 和 end 范围内的元素数量
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="start">起始索引</param>
        /// <param name="end">结束索引</param>
        /// <returns></returns>
        public long ZLexCount(string setId, string start, string end) => this.DoExecute(setId, c => c.ZLexCount(setId, start, end));

        /// <summary>
        /// 返回有序集 key 中， score 值在 min 和 max 之间(默认包括 score 值等于 min 或 max )的成员的数量
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="min">起始区间</param>
        /// <param name="max">结束区间</param>
        /// <returns></returns>
        public long ZCount(string setId, long min, long max) => this.DoExecute(setId, c => c.ZCount(setId, min, max));

        /// <summary>
        /// 返回有序集 key 中，成员 member 的 score 值
        /// </summary>
        /// <param name="setId">集合键</param>
        /// <param name="value">member 元素</param>
        /// <returns></returns>
        public double ZScore(string setId, byte[] value) => this.DoExecute(setId, c => c.ZScore(setId, value));

        /// <summary>
        /// 计算给定的一个或多个有序集的并集，其中给定 key 的数量必须以 numkeys 参数指定，并将该并集(结果集)储存到 destination。
        /// 默认情况下，结果集中某个成员的 score 值是所有给定集下该成员 score 值之 和 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setIds"></param>
        /// <returns></returns>
        public long ZUnionStore(string destination, params string[] setIds)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            return this.DoExecute(keys.ToArray(), c => c.ZUnionStore(destination, setIds));
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的并集，其中给定 key 的数量必须以 numkeys 参数指定，并将该并集(结果集)储存到 destination。
        /// 默认情况下，结果集中某个成员的 score 值是所有给定集下该成员 score 值之 和 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setIds"></param>
        /// <param name="args">
        /// 
        /// WEIGHTS
        /// 使用 WEIGHTS 选项，你可以为 每个 给定有序集 分别 指定一个乘法因子(multiplication factor)，
        /// 每个给定有序集的所有成员的 score 值在传递给聚合函数(aggregation function)之前都要先乘以该有序集的因子。
        /// 如果没有指定 WEIGHTS 选项，乘法因子默认设置为 1 。
        /// 
        /// AGGREGATE
        /// 使用 AGGREGATE 选项，你可以指定并集的结果集的聚合方式。
        /// 默认使用的参数 SUM ，可以将所有集合中某个成员的 score 值之 和 作为结果集中该成员的 score 值；
        /// 使用参数 MIN ，可以将所有集合中某个成员的 最小 score 值作为结果集中该成员的 score 值；而参数 MAX 则是将所有集合中某个成员的 最大 score 值作为结果集中该成员的 score 值。
        /// </param>
        /// <returns></returns>
        public long ZUnionStore(string destination, string[] setIds, string[] args)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            return this.DoExecute(keys.ToArray(), c => c.ZUnionStore(destination, setIds, args));
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的交集，其中给定 key 的数量必须以 numkeys 参数指定，并将该交集(结果集)储存到 destination 。
        /// 默认情况下，结果集中某个成员的 score 值是所有给定集下该成员 score 值之和
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setIds"></param>
        /// <returns></returns>
        public long ZInterStore(string destination, params string[] setIds)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            return this.DoExecute(keys.ToArray(), c => c.ZInterStore(destination, setIds));
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的交集，其中给定 key 的数量必须以 numkeys 参数指定，并将该交集(结果集)储存到 destination 。
        /// 默认情况下，结果集中某个成员的 score 值是所有给定集下该成员 score 值之和
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="setIds"></param>
        /// <param name="args">
        /// 
        /// WEIGHTS
        /// 使用 WEIGHTS 选项，你可以为 每个 给定有序集 分别 指定一个乘法因子(multiplication factor)，
        /// 每个给定有序集的所有成员的 score 值在传递给聚合函数(aggregation function)之前都要先乘以该有序集的因子。
        /// 如果没有指定 WEIGHTS 选项，乘法因子默认设置为 1 。
        /// 
        /// AGGREGATE
        /// 使用 AGGREGATE 选项，你可以指定并集的结果集的聚合方式。
        /// 默认使用的参数 SUM ，可以将所有集合中某个成员的 score 值之 和 作为结果集中该成员的 score 值；
        /// 使用参数 MIN ，可以将所有集合中某个成员的 最小 score 值作为结果集中该成员的 score 值；而参数 MAX 则是将所有集合中某个成员的 最大 score 值作为结果集中该成员的 score 值。
        /// </param>
        /// <returns></returns>
        public long ZInterStore(string destination, string[] setIds, string[] args)
        {
            var keys = new List<string> { destination };
            if (setIds != null) keys.AddRange(setIds);
            return this.DoExecute(keys.ToArray(), c => c.ZInterStore(destination, setIds, args));
        }
    }
}
