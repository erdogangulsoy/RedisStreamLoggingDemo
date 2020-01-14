using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Web.Pages
{
    public class EditModel : PageModel
    {
        IConnectionMultiplexer _connection;
        public Dictionary<StackExchange.Redis.RedisValue, StackExchange.Redis.RedisValue> Customer = null;
        public EditModel(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }
        public async Task<IActionResult> OnGet(string id)
        {
            IDatabase db = _connection.GetDatabase();
            Data.Repositories.Customer customerRep = new Data.Repositories.Customer(db);
            Customer = await customerRep.GetById(id);

            return Page();
        }

        public async Task<JsonResult> OnPost(string id, string name, string creditlimit, string salesrep)
        {
            IDatabase db = _connection.GetDatabase();

            Data.Repositories.Customer customerRep = new Data.Repositories.Customer(db);
            var customer = await customerRep.GetById(id);

            customer[Data.Entities.Customer.NAME] = name;
            customer[Data.Entities.Customer.CREDITLIMIT] = creditlimit;
            customer[Data.Entities.Customer.SALESREP] = salesrep;

            await customerRep.Store(customer);

            return new JsonResult("ok");

        }
    }
}