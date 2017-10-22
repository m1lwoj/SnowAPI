using EmailSender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SnowAPI.Filters;
using SnowAPI.Infrastracture;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Resolvers;
using SnowBLL.Service.Concrete;
using SnowBLL.Service.Interfaces;
using SnowDAL;
using SnowDAL.Concrete.Repositories;
using SnowDAL.Repositories.Concrete;
using SnowDAL.Repositories.Interfaces;
using Swashbuckle.Swagger.Model;
using System;
using System.Linq;
using System.Text;

namespace SnowAPI
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        private const string SecretKey = "needtogetthisfromenvironment";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var sqlConnectionString = Configuration["DataAccessPostgreSqlProvider:ConnectionString"];

            services.AddDbContext<EFContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("SnowAPI")
                )
            );

            services.AddTransient<IRouteBLService, RouteBLService>();
            services.AddTransient<IRouteRepository, RouteRepository>();
            services.AddTransient<IRoutePointRepository, RoutePointRepository>();
            services.AddTransient<IUserBLService, UserBLService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthBLService, AuthBLService>();
            services.AddTransient<IUserResolverService, UserResolverService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IUserResolver, UserResolver>();
            services.AddTransient<IRequestLogService, RequestLogService>();
            services.AddTransient<IApiLogRepository, ApiLogRepository>();
            services.AddTransient<ISender, Sender>();
            services.AddTransient<ISystemCodeRepository, SystemCodeRepository>();

            services.AddCors();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();

                config.Filters.Add(new AuthenticationFilter(policy));
            });

            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "SnowApp API",
                });
                options.IncludeXmlComments(GetSwaggerXMLPath());
                options.DescribeAllEnumsAsStrings();
            });

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim("User", "User"));
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin", "Admin"));
            });

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        private string GetSwaggerXMLPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(app.ApplicationBasePath, "SnowAPI.xml");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.Use((context, next) =>
            {
                if (context.Request.Headers.Any(k => k.Key.Contains("Origin")) && context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    return context.Response.WriteAsync("handled");
                }

                return next.Invoke();
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        string id = Guid.NewGuid().ToString();
                        ILogger logger = loggerFactory.CreateLogger<ErrorResult>();
                        logger.LogError($"Identifier: {id}{Environment.NewLine} Error: {ex.Message}{Environment.NewLine} CallStack: {ex.StackTrace}");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResult()
                        {
                            Id = id,
                            Message = "Unhandled error"
                        }));
                    }
                });
            });

            // Enables JWT authorization
            ConfigureAuthorization(app);

            app.UseApiLogMiddleware();

            app.UseStaticFiles();

            app.UseMvc();

            app.UseCors(builder =>
               builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
              );

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();


        }

        private void ConfigureAuthorization(IApplicationBuilder app)
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
        }
    }
}
