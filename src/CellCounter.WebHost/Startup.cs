using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace CellCounter.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddRazorPages();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // serialize enums as strings in api responses (e.g. Role)
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    //保持属性名称一致
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    //中文不编码
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();


            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
