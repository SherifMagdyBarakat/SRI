using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SRGD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class MasterDbContext: DbContext
    {
     /* public MasterDbContext()
        {
            var adapter = (IObjectContextAdapter)this;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = 1 * 60; // value in seconds
        }*/
            public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
            {
          
            }

        public DbSet<Users> users { get; set; }
        public DbSet<LoginViewModel> credential { get; set; }
        public DbSet<Experiments> experiments { get; set; }
        public DbSet<Nmetrics> nmetrics { get; set; }
        
    }
}
