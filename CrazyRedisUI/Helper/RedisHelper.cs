using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyRedisUI
{
    public class RedisHelper
    {
        public static List<string> ConnectionStrings;
        public static ConcurrentBag<string> Keys = new ConcurrentBag<string>();
        public static ConnectionMultiplexer Redis = null;
        public static List<IServer> Servers = null;
        public static void FillKeys()
        {
            Keys = new ConcurrentBag<string>();
            //先只关注集群模式，其他的怎么判断再说
            Parallel.ForEach(Servers, new ParallelOptions { MaxDegreeOfParallelism = 10 }, server =>
            {
                if (server.IsConnected)
                {
                    foreach (var key in server.Keys().ToList())
                    {
                        Keys.Add(key.ToString());
                    }
                }
            });


            //System.Windows.Forms.MessageBox.Show("可以连接到的:\r\n" + string.Join("\r\n", Servers.Select(p => p.EndPoint.ToString())));

        }

        public static void Connect()
        {
            Redis = ConnectionMultiplexer.Connect(string.Join(",", ConnectionStrings));
            Servers = Redis.GetEndPoints().Select(p => Redis.GetServer(p)).Where(p => p.IsConnected).ToList();
        }

        static RedisHelper()
        {
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += (a, b) => FillKeys();
            timer.Start();
        }

    }
}
