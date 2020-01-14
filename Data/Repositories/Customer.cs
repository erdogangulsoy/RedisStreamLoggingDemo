using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories
{
    public class Customer:RepositoryBase
    {
        public Customer(IDatabase db) : base(db, "Customer")
        {

        }
    }
}
