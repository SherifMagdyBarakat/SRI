using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SRGD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class ADbContext: DbContext
    {
     
            public ADbContext(DbContextOptions<ADbContext> options) : base(options)
            {
            }

      
        

    }
}
