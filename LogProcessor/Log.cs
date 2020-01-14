using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogProcessor
{
    public class Log
    {
        public HashEntry[] Current { get; set; }
        public HashEntry[] Snapshot { get; set; }

        public List<string> Added { get; set; }
        public List<string> Updated { get; set; }
        public List<string> Removed { get; set; }

        public Log()
        {
            Added = new List<string>();
            Updated = new List<string>();
            Removed = new List<string>();
        }

    }
}
