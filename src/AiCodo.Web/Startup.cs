// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo.Data;
using AiCodo.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo.Web
{
    public class Startup
    {
        const string _CorsAllowAll = "AllowAll";

        private bool _UseCors = true;
        private bool _UseToken = true;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_UseCors)
            {
                var urls = Configuration["AppCores"].Split(',');
                services.AddCors(options => options.AddPolicy(_CorsAllowAll,
                    policy => policy.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials())
                );
            }
            //AiCodo:Set db provider
            DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);
            services.AddSingleton<IUserService, UserService>();

            //var urls = Configuration["AppCores"].Split(',');
            //services.AddCors(options => options.AddPolicy("AllowAll",
            //    policy => policy.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials())
            //);
            if (_UseToken)
            {
                var jwtSection = Configuration.GetSection("Jwt");
                services.Configure<JwtConfig>(jwtSection);

                services.AddSingleton<ITokenService, TokenService>();

                var jwt = new JwtConfig();
                jwtSection.Bind(jwt);

                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwt.Issuer,
                            ValidAudience = jwt.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecurityKey)),
                            ClockSkew= TimeSpan.FromMinutes(1),
                        };
                    });
            }

            services.AddControllersWithViews();
            services.AddRazorPages();
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

            //AiCodo:Set config root path
            var appDataPath = Configuration["AppDataPath"];
            if (appDataPath.IsNullOrEmpty())
            {
                appDataPath = "App_Data";
            }
            ApplicationConfig.LocalDataFolder = System.IO.Path.Combine(env.ContentRootPath, appDataPath);
            ApplicationConfig.LocalConfigFolder = "configs".FixedAppDataPath();

            this.Log($"Set LocalDataFolder:{ApplicationConfig.LocalDataFolder}");

            //if use cors,enable policy "AllowAll"
            if (_UseCors)
            {
                app.UseCors(_CorsAllowAll);
            }

            app.UseStaticFiles();

            app.UseRouting();

            //����֤
            app.UseAuthentication();
            //����Ȩ
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }

    public class JwtConfig
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiresMinutes { get; set; }
    }
}
