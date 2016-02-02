using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class DataContext : DbContext
    {
        public DataContext():base("name=DefaultConnection")
        {
            Database.SetInitializer<DataContext>(null);
        }

        public DbSet<User> Users { get; set; }

    }
}
