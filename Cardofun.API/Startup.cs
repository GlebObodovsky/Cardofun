using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Cardofun.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Cardofun.API.Helpers.Constants;
using Cardofun.API.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using Cardofun.API.Policies.Requirements;

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
                // ToDo: ATTENTION! NEXT LINES SHOULD BE REMOVED BEFORE PUBLISHING
                // Configuration.GetSection(AppSettingsConstants.Token).Value = TokenGenerator.Generate();
                Configuration.GetSection(AppSettingsConstants.Token).Value = "super strong key";
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var corsOrigins = Configuration.GetSection(AppSettingsConstants.CorsOrigins).Get<string[]>();
            var signalrEndpoints = Configuration.GetSection(AppSettingsConstants.SignalrEndpoints).Get<Dictionary<string, string>>();

            IdentityBuilder builder = services.AddIdentityCore<User>(options => 
            {
                // ToDo: ATTENTION! NEXT LINES SHOULD BE REMOVED BEFORE PUBLISHING
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<CardofunContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection(AppSettingsConstants.Token).Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (signalrEndpoints.Any(se => path.StartsWithSegments(se.Value))))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddSingleton<IAuthorizationHandler, AccessedUserMatchesCurrentHandler>();
            services.AddAuthorization(ConfigurePolicies);
            services.AddDbContext<CardofunContext>(x => x.UseSqlServer(Configuration.GetConnectionString(ConnectionStringConstants.CardofunSqlServerConnection)));
            // Uncomment next line if you want to use SqlLite
            // services.AddDbContext<CardofunContext>(x => x.UseSqlite(Configuration.GetConnectionString(ConnectionStringConstants.CardofunSqlLiteConnection)));
            services.AddSignalR().AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.PayloadSerializerOptions.IgnoreNullValues = true;
                });
            services.AddCors(options => 
                { 
                    options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
                    .WithOrigins(corsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); 
                });
            services.AddControllers(options => 
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddNewtonsoftJson(options => 
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter() { NamingStrategy = new CamelCaseNamingStrategy() });
                })
                // Updates "LastActive" property of a User that executed an action
                .AddMvcOptions(options => options.Filters.Add(typeof(LogUserActivity)));
            
            services.AddCors();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddScoped<ICardofunRepository, CardofunRepository>();
            services.AddOptions();
            #region ImageProvider config
            // Next section configures cloudinary image provider. Change the configs
            // in case if you decided to use another one (own file system, for instance)
            services.Configure<CloudinaryProviderSettings>(Configuration.GetSection(AppSettingsConstants.ImageProviderSettings));
            services.AddTransient<IImageProvider, CloudinaryImageProvider>();
            #endregion ImageProvider config
            services.AddScoped<LogUserActivity>();
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
                app.UseExceptionHandler(builder => builder.AddGlobalErrorHandling());
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // ToDo: Uncomment next line before production
            // app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            var signalrEndpoints = Configuration.GetSection(AppSettingsConstants.SignalrEndpoints).Get<Dictionary<string, string>>();

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>(signalrEndpoints["chat"]);
                endpoints.MapHub<FriendsHub>(signalrEndpoints["friends"]);
                endpoints.MapHub<NotificationsHub>(signalrEndpoints["notifications"]);
            });
        }

        /// <summary>
        /// Configures all the policies that are used by the application
        /// </summary>
        /// <param name="options"></param>
        private void ConfigurePolicies(AuthorizationOptions options)
        {
            // The policy below is requires a user to be the same user whom information
            // is about to be accessed
            options.AddPolicy(PolicyConstants.UserMatchRequired, 
                policy => policy.AddRequirements(new AccessedUserMatchesCurrentRequirement()));

            // For each of the policies below Admin will be added as a role that is allowed 
            // to proceed in any circumstances
            options.AddPolicy(PolicyConstants.AdminRoleRequired, 
                policy => policy.RequireRole(RoleConstants.Admin));

            options.AddPolicy(PolicyConstants.ModeratorRoleRequired, 
                policy => policy.RequireRole(RoleConstants.Admin, RoleConstants.Moderator));
            // ...
        }
    }

    
}
