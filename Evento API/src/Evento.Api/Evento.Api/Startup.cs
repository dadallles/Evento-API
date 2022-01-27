using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Evento.Api.Framework;
using Evento.Core.Repositories;
using Evento.Infrastructure.Mappers;
using Evento.Infrastructure.Repositories;
using Evento.Infrastructure.Services;
using Evento.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;

namespace Evento.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IContainer Container { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented);

            services.AddMemoryCache();
            services.AddAuthorization(x => x.AddPolicy("HasAdminRole", p => p.RequireRole("admin")));
            //services.AddScoped<IEventRepository, EventRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IEventService, EventService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<ITicketService, TicketService>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            //services.AddSingleton<IJwtHandler, JwtHandler>();
            services.Configure<JwtSettings>(Configuration.GetSection("jwt"));
            services.Configure<AppSettings>(Configuration.GetSection("app"));


            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "http://localhost:65059",
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"))
                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<EventRepository>().As<IEventRepository>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<EventService>().As<IEventService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<TicketService>().As<ITicketService>().InstancePerLifetimeScope();
            builder.RegisterType<DataInitializer>().As<IDataInitializer>().InstancePerLifetimeScope();
            builder.RegisterType<JwtHandler>().As<IJwtHandler>().SingleInstance();
            Container = builder.Build();

            return new AutofacServiceProvider(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            SeedData(app);

            app.UseErrorHandler();

            app.UseHttpsRedirection();
            app.UseMvc();

            applicationLifetime.ApplicationStopped.Register( () => Container.Dispose() );
        }

        private void SeedData(IApplicationBuilder app)
        {
            var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>();
            if(settings.Value.SeedData)
            {
                var DataInitializer = app.ApplicationServices.GetService<IDataInitializer>();
                DataInitializer.SeedAsync();
            }
        }

    }
}
