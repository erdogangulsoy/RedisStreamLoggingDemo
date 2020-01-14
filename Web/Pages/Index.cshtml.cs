using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        IConnectionMultiplexer _connection;
        public HashSet<Dictionary<StackExchange.Redis.RedisValue, StackExchange.Redis.RedisValue>> Customers = null;
        public IndexModel(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task<IActionResult> OnGet()
        {
            IDatabase db = _connection.GetDatabase();
            Data.Repositories.Customer customerRep = new Data.Repositories.Customer(db);
            Customers = await customerRep.GetAll();

            return Page();
        }
    }
}