using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SRGD.Models;

namespace SRGD
{

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public class Startup
        {
            private IConfiguration _config;

            public Startup(IConfiguration config)
            {
                _config = config;
            }

            public void ConfigureServices(IServiceCollection services)
            {

            services.AddMvc();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
            services.AddTransient<Indexer>();
            services.AddTransient<RI>();
            services.AddTransient<Overlapping>();
            services.AddTransient<Tools>();
            services.AddTransient<Alignment>();
            services.AddTransient<Assembly>();
            services.AddTransient<Reports>();
          
            services.AddDbContextPool<MasterDbContext>(options => options.UseSqlServer(_config.GetConnectionString("MASTER"), options => options.CommandTimeout(4000000)));
            services.AddDbContextPool<ADbContext>(options => options.UseSqlServer(_config.GetConnectionString("A"), options => options.CommandTimeout(4000000)));
            services.AddDbContextPool<CDbContext>(options => options.UseSqlServer(_config.GetConnectionString("C"), options => options.CommandTimeout(4000000)));
            services.AddDbContextPool<GDbContext>(options => options.UseSqlServer(_config.GetConnectionString("G"), options => options.CommandTimeout(4000000)));
            services.AddDbContextPool<TDbContext>(options => options.UseSqlServer(_config.GetConnectionString("T"), options => options.CommandTimeout(4000000)));
          
        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();

                }
                app.UseStaticFiles();
                app.UseRouting();
         
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=home}/{action=index}/{id?}");
                });

            }
        }

    }
