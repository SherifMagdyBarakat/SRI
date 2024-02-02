using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SRGD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRGD.Models
{
    public class TDbContext: DbContext
    {
     
            public TDbContext(DbContextOptions<TDbContext> options) : base(options)
            {
            }
       

    }
}
