using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Web
{
    public class Startup
    {
       
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAntiforgery(option =>
            //{
            //    option.HeaderName = "X-CSRF-TOKEN";
            //});

            services.AddSingleton<IConnectionMultiplexer>(i => ConnectionMultiplexer.Connect("127.0.0.1"));
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.RootDirectory = "/Pages";
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });

        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();

        }
    }
}
