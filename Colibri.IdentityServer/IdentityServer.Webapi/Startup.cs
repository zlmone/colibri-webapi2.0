﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer.Webapi.Data;
using IdentityServer.Webapi.Services;
using IdentityServer4.Services;
using IdentityServer.Webapi.Integration;
using IdentityServer.Webapi.Configurations;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Webapi
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "damienbodserver.pfx"), "");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            var guestPolicy = new AuthorizationPolicyBuilder()
           .RequireAuthenticatedUser()
           .RequireClaim("scope", "dataEventRecords")
           .Build();

            services.AddTransient<IProfileService, IdentityProfileService>();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryClients(Clients.GetClients())
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityProfileService>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              .AddIdentityServerAuthentication(options =>
              {
                  options.Authority = Clients.HOST_URL + "/";
                  options.ApiName = "dataEventRecords";
                  options.ApiSecret = "dataEventRecordsSecret";
                  options.SupportedTokens = SupportedTokens.Both;
              });

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            //{
            //    Authority = Config.HOST_URL + "/",
            //    AllowedScopes = new List<string> { "dataEventRecords" },
            //    ApiSecret = "dataEventRecordsSecret",
            //    ApiName = "dataEventRecords",
            //    AutomaticAuthenticate = true,
            //    SupportedTokens = SupportedTokens.Both,
            //    // TokenRetriever = _tokenRetriever,
            //    // required if you want to return a 403 and not a 401 for forbidden responses
            //    AutomaticChallenge = true,
            //};

            //app.UseIdentityServerAuthentication(identityServerValidationOptions);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("dataEventRecordsAdmin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "dataEventRecords.admin");
                });
                options.AddPolicy("admin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "admin");
                });
                options.AddPolicy("dataEventRecordsUser", policyUser =>
                {
                    policyUser.RequireClaim("role", "dataEventRecords.user");
                });
                options.AddPolicy("dataEventRecords", policyUser =>
                {
                    policyUser.RequireClaim("scope", "dataEventRecords");
                });
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var angularRoutes = new[] {
                "/Unauthorized",
                "/Forbidden",
                "/uihome",
                "/dataeventrecords",
                "/dataeventrecords/",
                "/dataeventrecords/create",
                "/dataeventrecords/edit/",
                "/dataeventrecords/list",
                "/usermanagement",
                };

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && null != angularRoutes.FirstOrDefault(
                    (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.Path = new PathString("/");
                }

                await next();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}