using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AuthWebApp.Models
{
    public class UserContext : DbContext
    {
        public UserContext() :
            base("DefaultConnection")
        { }

        public DbSet<User> Users { get; set; }
    }

    public class Create_DB : DropCreateDatabaseAlways<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            base.Seed(context);
        }
    }
}
