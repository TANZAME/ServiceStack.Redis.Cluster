using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack.Redis.Cluster
{
    /// <summary>
    /// 集群节点信息
    /// </summary>
    internal class InternalClusterNode
    {
        /// <summary>
        /// 主节点
        /// </summary>
        public const string MASTER = "master";

        /// <summary>
        /// 从节点
        /// </summary>
        public const string SLAVE = "slave";

        /// <summary>
        /// 正在关系的节点
        /// </summary>
        public const string MYSELF = "myself";

        /// <summary>
        /// 已连接
        /// </summary>
        public const string CONNECTED = "connected";

        private string _nodeDescription = null;

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
        /// 节点ID
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// 主设备ID
        /// </summary>
        public string MasterNodeId { get; set; }

        /// <summary>
        /// 从节点列表
        /// </summary>
        public IEnumerable<InternalClusterNode> SlaveNodes { get; set; }

        /// <summary>
        /// 是否主设备
        /// </summary>
        public bool IsMater { get; set; }

        /// <summary>
        /// 是否从设备
        /// </summary>
        public bool IsSlave { get; set; }

        /// <summary>
        /// 节点标志位
        /// </summary>
        public string NodeFlag { get; set; }

        /// <summary>
        /// 以毫秒为单位的当前激活的ping发送的unix时间，如果没有挂起的ping，则为零
        /// </summary>
        public long PingSent { get; set; }

        /// <summary>
        /// 毫秒 unix 时间收到最后ping响应
        /// </summary>
        public long PongRecv { get; set; }

        /// <summary>
        /// 当前节点（或当前主节点，如果该节点是从节点）的配置时期（或版本）。
        /// 每次发生故障切换时，都会创建一个新的，唯一的，单调递增的配置时期。
        /// 如果多个节点声称服务于相同的哈希槽，则具有较高配置时期的节点将获胜。
        /// </summary>
        public int ConfigEpoch { get; set; }

        /// <summary>
        /// 用于节点到节点集群总线的链路状态。我们使用此链接与节点进行通信。可以是connected或disconnected
        /// </summary>
        public string LinkState { get; set; }

        /// <summary>
        /// 节点插槽
        /// </summary>
        public HashSlot Slot { get; set; }

        /// <summary>
        /// 余下的节点插槽。比如 0-1364 5461-6826 10923-12287，那么 5461-6826 10923-12287 这些就归到余下的插槽里
        /// </summary>
        public IList<HashSlot> RestSlots { get; set; }

        /// <summary>
        /// 节点  HOST + PORT 的字符串表示形式，做为节点的唯一键
        /// </summary>
        public string HostString
        {
            get
            {
                string result = string.Format("{0}:{1}", this.Host, this.Port);
                if (!string.IsNullOrEmpty(this.Password)) result = string.Format("{0}@{1}", this.Password, result);
                return result;
            }
        }

        /// <summary>
        /// 实例化 <see cref="ClusterNode"/> 类的新实例
        /// </summary>
        public InternalClusterNode()
        {
            this.Slot = new HashSlot(0, null);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(this._nodeDescription) ? this._nodeDescription : this.HostString;
        }

        /// <summary>
        /// 从 cluster nodes 的每一行命令里读取出集群节点的相关信息
        /// </summary>
        /// <param name="line">集群命令</param>
        /// <returns></returns>
        public static InternalClusterNode Parse(string line)
        {
            if (string.IsNullOrEmpty(line))
                throw new ArgumentException("line");

            InternalClusterNode node = new InternalClusterNode();
            node._nodeDescription = line;
            string[] segs = line.Split(' ');

            node.NodeId = segs[0];
            node.Host = segs[1].Split(':')[0];
            node.Port = int.Parse(segs[1].Split(':')[1]);
            node.MasterNodeId = segs[3] == "-" ? null : segs[3];
            node.PingSent = long.Parse(segs[4]);
            node.PongRecv = long.Parse(segs[5]);
            node.ConfigEpoch = int.Parse(segs[6]);
            node.LinkState = segs[7];

            string[] flags = segs[2].Split(',');
            node.IsMater = flags[0] == MYSELF ? flags[1] == MASTER : flags[0] == MASTER;
            node.IsSlave = !node.IsMater;
            int start = 0;
            if (flags[start] == MYSELF)
                start = 1;
            if (flags[start] == SLAVE || flags[start] == MASTER)
                start += 1;
            node.NodeFlag = string.Join(",", flags.Skip(start));

            if (segs.Length > 8)
            {
                string[] slots = segs[8].Split('-');
                node.Slot.Start = int.Parse(slots[0]);
                if (slots.Length > 1) node.Slot.End = int.Parse(slots[1]);

                for (int index = 9; index < segs.Length; index++)
                {
                    if (node.RestSlots == null)
                        node.RestSlots = new List<HashSlot>();

                    slots = segs[index].Split('-');

                    int s1 = 0;
                    int s2 = 0;
                    bool b1 = int.TryParse(slots[0], out s1);
                    bool b2 = int.TryParse(slots[1], out s2);
                    if (!b1 || !b2)
                        continue;
                    else
                        node.RestSlots.Add(new HashSlot(s1, slots.Length > 1 ? new Nullable<int>(s2) : null));
                }
            }

            return node;



            //序列化格式
            //命令的输出只是一个空格分隔的 CSV 字符串，其中每行代表集群中的一个节点。以下是输出示例：

            //07c37dfeb235213a872192d90877d0cd55635b91 127.0.0.1:30004@通讯端口 slave e7d1eecce10fd6bb5eb35b9f99a514335d9ba9ca 0 1426238317239 4 connected
            //67ed2db8d677e59ec4a4cefb06858cf2a1a89fa1 127.0.0.1:30002@通讯端口 master - 0 1426238316232 2 connected 5461-10922
            //292f8b365bb7edb5e285caf0b7e6ddc7265d2f4f 127.0.0.1:30003@通讯端口 master - 0 1426238318243 3 connected 10923-16383
            //6ec23923021cf3ffec47632106199cb7f496ce01 127.0.0.1:30005@通讯端口 slave 67ed2db8d677e59ec4a4cefb06858cf2a1a89fa1 0 1426238316232 5 connected
            //824fe116063bc5fcf9f4ffd895bc17aee7731ac3 127.0.0.1:30006@Password slave 292f8b365bb7edb5e285caf0b7e6ddc7265d2f4f 0 1426238317741 6 connected
            //e7d1eecce10fd6bb5eb35b9f99a514335d9ba9ca 127.0.0.1:30001@通讯端口 myself,master - 0 0 1 connected 0-5460
            //每行由以下字段组成：

            //<id> <ip:port> <flags> <master> <ping-sent> <pong-recv> <config-epoch> <link-state> <slot> <slot> ... <slot>
            //每个字段的含义如下：
            //1. id：节点 ID，一个40个字符的随机字符串，当一个节点被创建时不会再发生变化（除非CLUSTER RESET HARD被使用）。
            //2. ip:port：客户端应该联系节点以运行查询的节点地址。
            //3. flags：逗号列表分隔的标志：myself，master，slave，fail?，fail，handshake，noaddr，noflags
            //4. master：如果节点是从属节点，并且主节点已知，则节点ID为主节点，否则为“ - ”字符。
            //5. ping-sent：以毫秒为单位的当前激活的ping发送的unix时间，如果没有挂起的ping，则为零。
            //6. pong-recv：毫秒 unix 时间收到最后一个乒乓球。
            //7. config-epoch：当前节点（或当前主节点，如果该节点是从节点）的配置时期（或版本）。每次发生故障切换时，都会创建一个新的，唯一的，单调递增的配置时期。如果多个节点声称服务于相同的哈希槽，则具有较高配置时期的节点将获胜。
            //8. link-state：用于节点到节点集群总线的链路状态。我们使用此链接与节点进行通信。可以是connected或disconnected。
            //9. slot：散列槽号或范围。从参数9开始，但总共可能有16384个条目（限制从未达到）。这是此节点提供的散列槽列表。如果条目仅仅是一个数字，则被解析为这样。如果它是一个范围，它是在形式start-end，并且意味着节点负责所有散列时隙从start到end包括起始和结束值。
            //标志的含义（字段编号3）：
            //myself：您正在联系的节点。
            //master：节点是主人。
            //slave：节点是从属的。
            //fail?：节点处于PFAIL状态。对于正在联系的节点无法访问，但仍然可以在逻辑上访问（不处于FAIL状态）。
            //fail：节点处于FAIL状态。对于将PFAIL状态提升为FAIL的多个节点而言，这是无法访问的。
            //handshake：不受信任的节点，我们握手。
            //noaddr：此节点没有已知的地址。
            //noflags：根本没有标志。
        }

        /// <summary>
        /// 节点插槽
        /// </summary>
        public class HashSlot
        {
            /// <summary>
            /// 起始值
            /// </summary>
            public int Start { get; internal set; }

            /// <summary>
            /// 结束值
            /// </summary>
            public int? End { get; internal set; }

            /// <summary>
            /// 实例化 <see cref="HashSlot"/> 类的新实例
            /// </summary>
            /// <param name="start">起始值</param>
            /// <param name="end">结束值</param>
            public HashSlot(int start, int? end)
            {
                this.Start = start;
                this.End = end;
            }
        }
    }
}