using Core.Repositories;
using Data;
using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Linq;
using System.Reflection;

namespace WebApi
{
    public partial class Startup
    {
        private static readonly Type repoType = typeof(Repository<,>);

        private static readonly Type entityType = typeof(Entity);

        private readonly Container _container = new Container();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();
            Initialize(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My App", Version = "v1" });
            });
        }
        
        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            // Sets up the basic configuration that for integrating Simple Injector with
            // ASP.NET Core by setting the DefaultScopedLifestyle, and setting up auto
            // cross wiring.
            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                        .AddControllerActivation();
            });
            services.UseSimpleInjectorAspNetRequestScoping(_container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options =>
            {
                options.WithOrigins("http://localhost:4200");
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });

            _container.Verify();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My App");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void Initialize(IServiceCollection services)
        {
            var specificRepos = entityType.Assembly
                  .ExportedTypes
                  .Where(t => t.IsClass && !t.IsAbstract && entityType.IsAssignableFrom(t))
                  .Select(oneEntityType =>
                  {
                      var implementationType = repoType.MakeGenericType(oneEntityType, typeof(DataContext));
                      var interfaceType = typeof(IRepository<>).MakeGenericType(oneEntityType);
                      return (interfaceType, implementationType);
                  });

            foreach (var (interfaceType, implementationType) in specificRepos)
                services.AddScoped(interfaceType, implementationType);

            var typesToRegister = repoType.GetTypeInfo().Assembly.ExportedTypes.Select(t => t.GetTypeInfo());

            var registrations = from type in typesToRegister
                                let @interface = type.ImplementedInterfaces.FirstOrDefault(inter => inter.Name == $"I{type.Name}")
                                where @interface != null && type.IsClass && !type.IsGenericType
                                select (@interface, type.AsType());

            foreach (var (@interface, @class) in registrations)
                services.AddScoped(@interface, @class);

            ConfigureValidators(services);
            ConfigureMapper(services);
            ConfigureMediator(services);
        }

    }
}
