
using ServiceStack.Redis.Cluster;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceStack.Redis.FormUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 缓存键

        // 将字符串值 value 关联到 key 。
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果 key 已经持有其他值， SET 就覆写旧值，无视类型。
            //对于某个原本带有生存时间（TTL）的键来说， 当 SET 命令成功在这个键上执行时， 这个键原有的 TTL 将被清除。

            //从 Redis 2.6.12 版本开始， SET 命令的行为可以通过一系列参数来修改：
            //    EX second ：设置键的过期时间为 second 秒。 SET key value EX second 效果等同于 SETEX key second value 。
            //    PX millisecond ：设置键的过期时间为 millisecond 毫秒。 SET key value PX millisecond 效果等同于 PSETEX key millisecond value 。
            //    NX ：只在键不存在时，才对键进行设置操作。 SET key value NX 效果等同于 SETNX key value 。
            //    XX ：只在键已经存在时，才对键进行设置操作。

            using (var channel = RedisClientManager.GetClient())
            {
                //channel.Set<string>("LabelTemplate", "LabelTemplateLLL");
                channel.Set<int>("LabelTemplate_int", 10);
                channel.Set<string>("LabelTemplate_string", "设置键的过期时间为 second 秒。");
            }
        }

        // 返回 key 所关联的字符串值。
        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果 key 不存在那么返回特殊值 nil 。
            //假如 key 储存的值不是字符串类型，返回一个错误，因为 GET 只能用于处理字符串值。
            //可用版本：

            var now = DateTime.Now;
            AppendTextAsync(string.Format("{0} 开始", now.ToString("yyyy-MM-dd HH:mm:ss")));

            using (var channel = RedisClientManager.GetClient())
            {
                for (int i = 0; i <= 100000; i++)
                {
                    var result = channel.Get<long>("Template_lock");
                    //this.AppendTextAsync("第 {0} 次，=> {1}", i, result);
                }
            }

            AppendTextAsync(string.Format("{0} 结束，10万次耗时 {1} 秒", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (DateTime.Now - now).TotalSeconds));
        }

        // 删除给定的一个或多个 key 。
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // remove 底层也是调用 del 命令去删除，并且判断返回值是否为 1
            // 只要有一个命令删除成功就返回1
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Del("LabelTemplate", "LabelTemplate_1", "LabelTemplate_cost");
            }
        }

        // 序列化给定 key对应的值，并返回被序列化的值
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                //序列化生成的值有以下几个特点：
                //它带有 64 位的校验和，用于检测错误， RESTORE 在进行反序列化之前会先检查校验和。
                //值的编码格式和 RDB 文件保持一致。
                //RDB 版本会被编码在序列化值当中，如果因为 Redis 的版本不同造成 RDB 格式不兼容，那么 Redis 会拒绝对这个值进行反序列化操作。
                //序列化的值不包括任何生存时间信息。
                var bytes = channel.Dump("LabelTemplate_string");


                //反序列化给定的序列化值，并将它和给定的 key 关联。
                //参数 ttl 以毫秒为单位为 key 设置生存时间；如果 ttl 为 0 ，那么不设置生存时间。
                //如果指定的 key 已存在 ，则会抛出异常
                channel.Restore("LabelTemplate_1", 0, bytes);

                string result = channel.Get<string>("LabelTemplate_1");
                richTextBox1.Text = result;
            }
        }

        // 检查给定 key 是否存在。
        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Exists("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        // 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除。
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Expire("LabelTemplate", 10);
                richTextBox1.Text = result.ToString();
            }
        }

        // 查找所有符合给定模式 pattern 的 key 。
        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //KEYS 匹配符
            //1. * 任意长度的任意字符
            //2. ? 任意单一字符
            //3. [xxx] 匹配方括号中的一个字符
            //如：
            //h?llo matches hello, hallo and hxllo
            //h*llo matches hllo and heeeello
            //h[ae]llo matches hello and hallo, but not hillo
            //h[^e]llo matches hallo, hbllo, ... but not hello
            //h[a-b]llo matches hallo and hbllo

            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Keys("*");
                if (result.Length > 0)
                {
                    string line = string.Empty;
                    foreach (var bytes in result) line += Encoding.UTF8.GetString(bytes) + Environment.NewLine;
                    richTextBox1.Text = line;
                }
            }
        }

        // 将 key 原子性地从当前实例传送到目标实例的指定数据库上，一旦传送成功， key 保证会出现在目标实例上，而当前实例上的 key 会被删除。
        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //注意：这里在操作migrate的时候，若各节点有认证，执行的时候会出现：
            //(error) ERR Target instance replied with error: NOAUTH Authentication required.
            //若确定执行的迁移，本文中是把所有节点的masterauth和requirepass注释掉之后进行的，等进行完之后再开启认证。

            using (var channel = RedisClientManager.GetClient())
            {
                channel.Migrate("192.168.34.170", 6379, "LabelTemplate", 0, 5 * 1000);
            }
        }

        // 将当前数据库的 key 移动到给定的数据库 db 当中。
        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
            //因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。

            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Move("LabelTemplate", 1);
                richTextBox1.Text = result.ToString();

                // ChnageDb 对应 SELECT 命令
                channel.ChangeDb(1);
                AppendTextAsync(channel.Get<string>("LabelTemplate"));
            }
        }

        // 从内部察看给定 key 的 Redis 对象。
        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //它通常用在除错(debugging)或者了解为了节省空间而对 key 使用特殊编码的情况。
            //当将Redis用作缓存程序时，你也可以通过 OBJECT 命令中的信息，决定 key 的驱逐策略(eviction policies)。
            //OBJECT 命令有多个子命令：
            //    OBJECT REFCOUNT <key> 返回给定 key 引用所储存的值的次数。此命令主要用于除错。
            //    OBJECT ENCODING <key> 返回给定 key 锁储存的值所使用的内部表示(representation)。
            //    OBJECT IDLETIME <key> 返回给定 key 自储存以来的空转时间(idle， 没有被读取也没有被写入)，以秒为单位。

            //对象可以以多种方式编码：
            //    字符串可以被编码为 raw (一般字符串)或 int (用字符串表示64位数字是为了节约空间)。
            //    列表可以被编码为 ziplist 或 linkedlist 。 ziplist 是为节约大小较小的列表空间而作的特殊表示。
            //    集合可以被编码为 intset 或者 hashtable 。 intset 是只储存数字的小集合的特殊表示。
            //    哈希表可以编码为 zipmap 或者 hashtable 。 zipmap 是小哈希表的特殊表示。
            //    有序集合可以被编码为 ziplist 或者 skiplist 格式。 ziplist 用于表示小的有序集合，而 skiplist 则用于表示任何大小的有序集合。

            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.ObjectIdleTime("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        // 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Persist("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        // 从当前数据库中随机返回(不删除)一个 key 。
        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.RandomKey();
                richTextBox1.Text = result.ToString();
            }
        }

        // 将 key 改名为 newkey 。
        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                channel.Rename("LabelTemplate", "LabelTemplate");
            }
        }

        // 返回或保存给定列表、集合、有序集合 key 中经过排序的元素。
        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                channel.Del("LabelTemplate_cost", "LabelTemplate_uid");

                //channel.AddItemToList("LabelTemplate_cost", "12");
                //channel.AddItemToList("LabelTemplate_cost", "12.98");
                //channel.AddItemToList("LabelTemplate_cost", "12.89");

                //var result = channel.Sort("LabelTemplate_cost", new SortOptions { SortDesc = true });
                //string d0 = System.Text.Encoding.UTF8.GetString(result[0]);
                //string d1 = System.Text.Encoding.UTF8.GetString(result[1]);
                //string d2 = System.Text.Encoding.UTF8.GetString(result[2]);

                //channel.LPush("LabelTemplate_cost", new[] { (12d).ToUtf8Bytes(), (12.86d).ToUtf8Bytes(), (12.68d).ToUtf8Bytes() });
                //var result = channel.Sort("LabelTemplate_cost", new SortOptions { SortDesc = true });
                //double d0 = Convert.ToDouble(System.Text.Encoding.UTF8.GetString(result[0]));
                //double d1 = Convert.ToDouble(System.Text.Encoding.UTF8.GetString(result[1]));
                //double d2 = Convert.ToDouble(System.Text.Encoding.UTF8.GetString(result[2]));

                //// 为什么数据自动变成了字符串~
                //channel.LPush("LabelTemplate_cost", new[] { BitConverter.GetBytes(12d), BitConverter.GetBytes(12.68d), BitConverter.GetBytes(12.86d) });
                //var result = channel.Sort("LabelTemplate_cost", new SortOptions { SortDesc = true, SortAlpha = true });
                //double d0 = BitConverter.ToDouble(result[0], 0);

                channel.LPush("LabelTemplate_uid", new[] { 5.ToUtf8Bytes(), 7.ToUtf8Bytes(), 9.ToUtf8Bytes() });

                channel.Set("user_name_5", "admin");
                channel.Set("user_level_5", 8);

                channel.Set("user_name_7", "jack");
                channel.Set("user_level_7", 7);

                channel.Set("user_name_9", "luci");
                channel.Set("user_level_9", 6);

                // 通过使用 BY 选项，可以让 uid 按其他键的元素来排序。
                // user_level_* 是一个占位符， 它先取出 uid 中的值， 然后再用这个值来查找相应的键。
                // 比如在对 uid 列表进行排序时， 程序就会先取出 uid 的值 1 、 2 、 3 、 4 ， 然后使用 user_level_1 、 user_level_2 、 user_level_3 和 user_level_4 的值作为排序 uid 的权重。

                // 使用 GET 选项， 可以根据排序的结果来取出相应的键值。

                // 通过组合使用 BY 和 GET ， 可以让排序结果以更直观的方式显示出来。可以同时使用多个 GET 选项， 获取多个外部键的值。

                var result = channel.Sort("LabelTemplate_uid", new SortOptions { SortPattern = "user_level_*", GetPattern = "user_name_*" });
                string d0 = System.Text.Encoding.UTF8.GetString(result[0]);
                string d1 = System.Text.Encoding.UTF8.GetString(result[1]);
                string d2 = System.Text.Encoding.UTF8.GetString(result[2]);
            }
        }

        // 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Ttl("ICS-SRP-dig-pendant diamond necklace");
                richTextBox1.Text = result.ToString();
            }
        }

        // 返回 key 所储存的值的类型。
        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Type("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        // 命令用于迭代当前数据库中的数据库键。
        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //SCAN 命令用于迭代当前数据库中的数据库键。
            //SSCAN 命令用于迭代集合键中的元素。
            //HSCAN 命令用于迭代哈希键中的键值对。
            //ZSCAN 命令用于迭代有序集合中的元素（包括元素成员和元素分值）。

            HashSet<string> keys = new HashSet<string>();

            while (true)
            {
                try
                {
                    int sumQty = 0;
                    AppendTextAsync(string.Format("{0}： 开始扫描命令", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    AppendTextAsync(Environment.NewLine);
                    using (var channel = RedisClientManager.GetClient())
                    {
                        var result = channel.Scan(0, 1000);
                        if (result.Results != null)
                        {
                            sumQty += result.Results.Count;
                            foreach (var bytes in result.Results)
                            {
                                string key = Encoding.UTF8.GetString(bytes);

                            }
                        }

                        while (result.Cursor != 0)
                        {
                            result = channel.Scan(result.Cursor, 1000);
                            if (result.Results != null) sumQty += result.Results.Count;

                            Application.DoEvents();
                        }

                        //richTextBox1.Text = result.ToString();

                        //var d = channel.GetAllEntriesFromHash("LabelTemplate_Hash");
                    }
                    AppendTextAsync(string.Format("{0}： 扫描命令结束，共 {1} 个关键词。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sumQty));
                    AppendTextAsync(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    AppendTextAsync(string.Format("{0}： {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message));
                    AppendTextAsync(Environment.NewLine);
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(5 * 60 * 1000);
            }
        }

        #endregion

        #region 字符串

        // 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
        private void linkLabel18_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
            //如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样

            using (var channel = RedisClientManager.GetClient())
            {
                //var mLong = channel.AppendToValue("LabelTemplate","OK 以上");

                // Get<string> 无法得到正确的 Append 后的值，原因是多了一层序列化
                // 方法1 使用原始 Get
                var bytes = channel.Get("LabelTemplate");
                var result = Encoding.UTF8.GetString(bytes);

                // 方法2 使用 GetValue
                result = channel.GetValue("LabelTemplate");

                //// 持久化
                //channel.RewriteAppendOnlyFileAsync();

                richTextBox1.Text = result.ToString();
            }
        }

        // 计算给定字符串中，被设置为 1 的比特位的数量。
        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 参考： https://www.zhihu.com/question/27672245
            // 在redis中，存储的字符串都是以二级制的进行存在的，此命令用来统计给定 key 的值中标记为 1 的个数
            // 配合 SETBIT 和 GETBIT 使用
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.BitCount("LabelTemplate3");
                richTextBox1.Text = result.ToString();

                //for (int i = 0; i <= 2999; i++)
                //{
                //    channel.SetBit("LabelTemplate3", i, 1);
                //    var b = channel.GetBit("LabelTemplate3", i);
                //    richTextBox1.Text = b.ToString();
                //}

                //var t = channel.GetValue("LabelTemplate3");
                //richTextBox1.Text = t.ToString();                
            }
        }

        // 将 key 中储存的数字值减一。
        private void linkLabel20_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
            //如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
            //本操作的值限制在 64 位(bit)有符号数字表示之内。

            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.Decr("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        // 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)
        private void linkLabel21_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推。
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.GetRange("LabelTemplate", -1, -2);
                richTextBox1.Text = result.ToString();
            }
        }

        // 这个命令和 SETEX 命令相似，但它以毫秒为单位设置 key 的生存时间，而不是像 SETEX 命令那样，以秒为单位。
        private void linkLabel22_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                //var result = channel.PSetEx("LabelTemplate", -1, -2);
                //richTextBox1.Text = result.ToString();
            }
        }

        // 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。
        private void linkLabel23_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 如果 key 已经存在， SETEX 命令将覆写旧值。
            using (var channel = RedisClientManager.GetClient())
            {
                channel.SetEx("LabelTemplate", 10, Encoding.UTF8.GetBytes(" 生存时间设为 seconds (以秒为单位)。"));
                var result = channel.GetValue("LabelTemplate");
                richTextBox1.Text = result.ToString();
            }
        }

        #endregion

        #region 哈希表

        // 将哈希表 key 中的域 field 的值设为 value 。
        private void linkLabel25_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //如果 key 不存在，一个新的哈希表被创建并进行 HSET 操作。
            //如果域 field 已经存在于哈希表中，旧值将被覆盖。

            using (var channel = RedisClientManager.GetClient())
            {
                channel.SetEntryInHash("LabelTemplate_Hash", "LabelTemplateName", "A4格式模板");
                channel.SetEntryInHash("LabelTemplate_Hash", "LabelTemplateCode", "A5格式模板");
            }
        }

        // 返回哈希表 key 中给定域 field 的值。
        private void linkLabel26_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var bytes = channel.HGet(Encoding.UTF8.GetBytes("LabelTemplate_Hash"), Encoding.UTF8.GetBytes("LabelTemplateName"));
                var result = Encoding.UTF8.GetString(bytes);
                richTextBox1.Text = result.ToString();
            }
        }

        // 返回哈希表 key 中，所有的域和值。
        private void linkLabel28_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.HGetAll("LabelTemplate_Hash");
                richTextBox1.Text = result.ToString();
            }
        }

        // 返回哈希表 key 中的所有域。
        private void linkLabel29_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.HKeys("LabelTemplate_Hash");
            }
        }

        // 返回哈希表 key 中域的数量。
        private void linkLabel30_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.HLen("LabelTemplate_Hash");
                richTextBox1.Text = result.ToString();
            }
        }

        // 返回哈希表 key 中所有域的值。
        private void linkLabel31_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.HVals("LabelTemplate_Hash");
                richTextBox1.Text = result.ToString();
            }
        }

        // 命令返回的每个元素都是一个键值对，一个键值对由一个键和一个值组成。
        private void linkLabel32_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var result = channel.HScan("LabelTemplate_Hash", 0);
                richTextBox1.Text = Encoding.UTF8.GetString(result.Results[3]).ToString();
            }
        }

        #endregion

        #region 列表

        // 将一个或多个值 value 插入到列表 key 的表头
        private void linkLabel33_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                string j = new LabelTemplate { LabelTemplateName = "Ok 以上" }.SerializeAndFormat();
                var result = channel.LPush("LabelTemplate_List", Encoding.UTF8.GetBytes(j));
                //richTextBox1.Text = result.ToString();

            }
        }

        // 移除并返回列表 key 的头元素。
        private void linkLabel34_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var bytes = channel.LPop("LabelTemplate_List");
                var result = Encoding.UTF8.GetString(bytes);
                richTextBox1.Text = result.ToString();
            }
        }

        #endregion

        #region 集合

        private void linkLabel35_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //using (
            var channel = RedisClientManager.GetClient();
            //)
            {
                //channel.SAdd()
                //channel.ZInterStore
                //channel.AcquireLock();

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    RedisLock(channel, "Template_lock", TimeSpan.FromSeconds(10));
                });
            }

            System.Threading.Thread.Sleep(1000);

            //using (
            var channel2 = RedisClientManager.GetClient();
            //)
            {

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    RedisLock(channel2, "Template_lock", TimeSpan.FromSeconds(10));
                });
            }
        }

        private void linkLabel24_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {

            }
        }

        private void linkLabel36_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var channel = RedisClientManager.GetClient())
            {
                var int2 = Math.Pow(2, 4);
            }
        }

        public void RedisLock(IRedisClient redisClient, string key, TimeSpan? timeOut)
        {
            //this.redisClient = redisClient;
            //this.key = key;
            ExecUtils.RetryUntilTrue(delegate
            {
                TimeSpan value = timeOut ?? new TimeSpan(365, 0, 0, 0);
                DateTime dateTime = DateTime.UtcNow.Add(value);
                string lockString = (dateTime.ToUnixTimeMs() + 1).ToString();
                if (redisClient.SetValueIfNotExists(key, lockString))
                {
                    return true;
                }
                redisClient.Watch(key);
                long result = -1L;
                string s = redisClient.Get<string>(key);
                if (!long.TryParse(s, out result))
                {
                    redisClient.UnWatch();
                    return false;
                }

                if (result <= DateTime.UtcNow.ToUnixTimeMs())
                {
                    using (IRedisTransaction redisTransaction = redisClient.CreateTransaction())
                    {
                        redisTransaction.QueueCommand((IRedisClient r) => r.Set(key, lockString));

                        System.Threading.Thread.Sleep(5 * 1000);

                        bool ok = redisTransaction.Commit();
                        var lockString2 = redisClient.Get<string>(key);
                        if (lockString2 != lockString)
                        {

                        }

                        return ok;
                    }
                }
                redisClient.UnWatch();
                return false;
            }, timeOut);
        }

        private void linkLabel38_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //https://blog.csdn.net/WuLex/article/details/52712664
            using (var channel = RedisClientManager.GetClient())
            {
                channel.Publish("channel_news", richTextBox1.Text.ToUtf8Bytes());
            }
        }

        private void linkLabel39_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                using (var consumer = RedisClientManager.GetClient())
                {
                    IRedisSubscription subscription = consumer.CreateSubscription();
                    // 接收到消息
                    subscription.OnMessage = (channel, message) =>
                    {
                        AppendTextAsync("接收到消息：" + ":" + message + " [" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "]");
                        AppendTextAsync(Environment.NewLine);
                        AppendTextAsync(string.Format("频道：{0} 订阅数：{1}", channel, subscription.SubscriptionCount));
                        AppendTextAsync(Environment.NewLine);
                        AppendTextAsync("------------------------------------------");
                        AppendTextAsync(Environment.NewLine);
                    };

                    //订阅事件
                    subscription.OnSubscribe = channel => AppendTextAsync(string.Format("开始订阅 {0}", channel));

                    //取消订阅事件
                    subscription.OnUnSubscribe = channel => AppendTextAsync(string.Format("取消订阅 {0}", channel));

                    //订阅频道
                    subscription.SubscribeToChannels("channel_news");
                }
            });
        }

        #endregion

        #region 集群



        private void linkLabel37_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //int slot = CRC16.GetSlot("template");

            //using (var channel = RedisClientManager.GetClient())
            //{
            //    var data= channel.RawCommand(Encoding.UTF8.GetBytes("CLUSTER"), Encoding.UTF8.GetBytes("NODES"));
            //    string liunes = Encoding.UTF8.GetString(data.Data);
            //}

            // redis配置文件信息 password@ip:port
            RedisConfig setting = RedisConfig.GetConfig();
            RedisClientManagerConfig config = new RedisClientManagerConfig();
            config.MaxWritePoolSize = setting.MaxWritePoolSize;
            config.MaxReadPoolSize = setting.MaxReadPoolSize;
            config.AutoStart = setting.AutoStart;
            config.DefaultDb = setting.DefaultDb;

            var node = new ClusterNode("127.0.0.1", 7001);
            var c = RedisClusterFactory.Configure(node, config);
            int taskQty = 1;
            for (int index = 1; index <= taskQty; index++)
            {
                Task.Factory.StartNew(() => ExecuteCluster(c));
            }

            //var keys = c.Keys("*");
            //c.FlushAll();
        }

        void ExecuteCluster(RedisCluster redisCluster)
        {
            char[] chars = new[] {
                '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            };
            var rd = new Random();
            DateTime lastWriteTime = new DateTime(1970, 01, 01);
            while (true)
            {
                try
                {
                    // 保持 0.5s 写一次 REDIS
                    if ((DateTime.Now - lastWriteTime).TotalSeconds < 0.5) continue;

                    char[] keyChars = new char[10];
                    keyChars[0] = 'B';
                    keyChars[1] = '0';
                    keyChars[2] = '7';
                    for (int index = 3; index < 10; index++)
                    {
                        var r = rd.Next(0, chars.Length - 1);
                        keyChars[index] = chars[r];
                    }

                    string key = new string(keyChars);
                    redisCluster.Set(key, key);
                    string value = redisCluster.Get<string>(key);
                    //redisCluster.Del(key);

                    this.AppendTextAsync("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), value);
                    lastWriteTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    this.AppendTextAsync2("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message);
                }
            }
        }

        #endregion

        string AppendTextAsync(string message, params object[] args)
        {
            if (message != null && args != null && args.Length > 0)
                message = string.Format(message, args);
            base.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(message);
                richTextBox1.AppendText(Environment.NewLine);
            }));

            return message;
        }

        string AppendTextAsync2(string message, params object[] args)
        {
            if (message != null && args != null && args.Length > 0)
                message = string.Format(message, args);
            base.Invoke(new Action(() =>
            {
                richTextBox2.AppendText(message);
                richTextBox2.AppendText(Environment.NewLine);
            }));

            return message;
        }
    }

    public class LabelTemplate
    {
        public string LabelTemplateCode { get; set; }
        public string LabelTemplateName { get; set; }
    }
}

// 参考：
// 1.IRedisClient 常用方法说明 https://www.bbsmax.com/A/l1dyyRwVde/
// 2.redis 命令说明：http://doc.redisfans.com/