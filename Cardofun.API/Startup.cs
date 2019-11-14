using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Cardofun.Core.Helpers.Security;
using Cardofun.Core.NameConstants;
using Cardofun.DataContext.Data;
using Cardofun.DataContext.Repositories;
using Cardofun.Interfaces.Repositories;
using Cardofun.API.Helpers.Extensions;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Cardofun.Interfaces.ServiceProviders;
using Cardofun.Modules.Cloudinary;
using Cardofun.API.Helpers;

namespace Cardofun.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            InitializeAsync();
        }
        /// <summary>
        /// Initializes the Web API
        /// </summary>
        /// <returns></returns>
        private async void InitializeAsync()
        {
            await Task.Run(() => 
            {
                // ToDo: Uncomment next line before production
                // Configuration.GetSection(AppSettingsConstants.Token).Value = TokenGenerator.Generate();
                Configuration.GetSection(AppSettingsConstants.Token).Value = "super strong key";
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CardofunContext>(x => x.UseSqlServer(Configuration.GetConnectionString(ConnectionStringConstants.CardofunSqlServerConnection)));
            // Uncomment next line if you want to use SqlLite
            // services.AddDbContext<CardofunContext>(x => x.UseSqlite(Configuration.GetConnectionString(ConnectionStringConstants.CardofunSqlLiteConnection)));
            services.AddMvc(options => 
                {
                    // Updates "LastActive" property of a User that executed an action
                    options.Filters.Add(typeof(LogUserActivity));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddJsonOptions(options => options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter() { NamingStrategy = new CamelCaseNamingStrategy() }));
            services.AddCors();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICardofunRepository, CardofunRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection(AppSettingsConstants.Token).Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    }
                );
            services.AddOptions();
            #region ImageProvider config
            // Next section configures cloudinary image provider. Change the configs
            // in case if you decided to use another one (own file system, for instance)
            services.Configure<CloudinaryProviderSettings>(Configuration.GetSection("ImageProviderProviderSettings"));
            services.AddTransient<IImageProvider, CloudinaryImageProvider>();
            #endregion ImageProvider config
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder => builder.AddGlobalErrorHandling());
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
