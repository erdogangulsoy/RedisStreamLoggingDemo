using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data
{
    public class RedisScripts
    {
        private static byte[] _script1;
        public static byte[] Script1
        {
            get
            {
                if (_script1 == null)
                {
                    #region Redis

                    //make sure all the scripts have been loaded

                    string log_proc = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "redis_log_proc.lua"));
                    ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect("127.0.0.1");
                    IServer server = multiplexer.GetServer("127.0.0.1:6379");

                    return server.ScriptLoad(log_proc);

                    #endregion
                }

                return _script1;
            }
        }

    }
}
