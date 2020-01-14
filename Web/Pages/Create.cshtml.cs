using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Web.Pages
{
    public class CreateModel : PageModel
    {
        IConnectionMultiplexer _connection;
        public Dictionary<StackExchange.Redis.RedisValue, StackExchange.Redis.RedisValue> Customer = null;
        public CreateModel(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task<JsonResult> OnPost(string name, string creditlimit, string salesrep)
        {
            IDatabase db = _connection.GetDatabase();
            
            Dictionary<RedisValue, RedisValue> data = new Dictionary<RedisValue, RedisValue>();
            data.Add(Data.Entities.Customer.NAME, name);
            data.Add(Data.Entities.Customer.CREDITLIMIT, creditlimit);
            data.Add(Data.Entities.Customer.SALESREP, salesrep);

            Data.Repositories.Customer customerRep = new Data.Repositories.Customer(db);
            string id = await customerRep.Store(data);

            return new JsonResult("ok");

        }
    }
}