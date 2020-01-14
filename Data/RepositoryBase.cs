using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class RepositoryBase
    {
        private IDatabase _db;

        public RepositoryBase(IDatabase db, string entityKeyName)
        {
            _db = db;

            SquenceKey = string.Concat("seq:", entityKeyName);
            EntityKey = entityKeyName;
            TypeIdsKey = string.Concat("ids:", entityKeyName);
        }
        public string SquenceKey { get; set; }
        public string EntityKey { get; set; }
        public string TypeIdsKey { get; set; }
        public string GetNextSquence() => _db.StringIncrement(SquenceKey).ToString();
        public virtual string GetEntityKey(string entityId)
        {
            return String.Concat("{", EntityKey, "}", ":", entityId);
        }
        public async Task<HashSet<Dictionary<RedisValue, RedisValue>>> GetAll()
        {
            RedisValue[] ids = await _db.SortedSetRangeByScoreAsync(TypeIdsKey);
            return await GetByIds(ids.Select(i => i.ToString()).ToArray());
        }
        public async Task<Dictionary<RedisValue, RedisValue>> GetById(string id)
        {
            string key = GetEntityKey(id);
            HashEntry[] data = await _db.HashGetAllAsync(key);

            var t = data.ToDictionary();

            return t;
        }
        public async Task<HashSet<Dictionary<RedisValue, RedisValue>>> GetByIds(IEnumerable<string> ids)
        {
            HashSet<Dictionary<RedisValue, RedisValue>> result = new HashSet<Dictionary<RedisValue, RedisValue>>();

            foreach (var Id in ids)
            {
                string key = GetEntityKey(Id);
                HashEntry[] data = await _db.HashGetAllAsync(key);
                result.Add(data.ToDictionary());
            }

            return result;
        }

        public async Task<string> Store(Dictionary<RedisValue, RedisValue> data)
        {
            bool isNewRecord = false;

            //Required Fields: Id, Updated

            if (!data.ContainsKey(Entity.CREATED))
            {
                data[Entity.CREATED] = DateTime.Now.ToString();
            }

            if (!data.ContainsKey(Entity.ID))
            {
                isNewRecord = true;
                //new record. No lastupdated field check required
                data.Add(Entity.ID, GetNextSquence());
                DateTime dtTimeStamp = DateTime.Parse(data[Entity.CREATED].ToString());
                await _db.SortedSetAddAsync(TypeIdsKey, data[Entity.ID], dtTimeStamp.Ticks);
            }



            string key = GetEntityKey(data[Entity.ID].ToString());

            await _db.HashSetAsync(key, data.Select(s => new HashEntry(s.Key, s.Value)).ToArray());

            #region Logging

            string snapshotKey = isNewRecord ? "" : DateTime.Now.Ticks.ToString();
            await _db.ScriptEvaluateAsync(RedisScripts.Script1, keys: new RedisKey[] { GetEntityKey(data[Entity.ID]), snapshotKey });
            #endregion

            return data[Entity.ID].ToString();
        }
    }
}
