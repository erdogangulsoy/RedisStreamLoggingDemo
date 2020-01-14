using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public static class Extensions
    {
        public static string GetStringOrNull(this Dictionary<RedisValue, RedisValue> record, string field)
        {
            return record.ContainsKey(field) ? record[field].ToString() : null;
        }
        public static string GetStringOrEmpty(this Dictionary<RedisValue, RedisValue> record, string field)
        {
            return record.ContainsKey(field) ? record[field].ToString() : "";
        }
        public static string GetStringOrAlternate(this Dictionary<RedisValue, RedisValue> record, string field, string alternate)
        {
            return record.ContainsKey(field) ? record[field].ToString() : alternate;
        }
    }
}
