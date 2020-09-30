using System;
using SimpleInjector;
using Data.Repositories;
using Domain.Models;
using Data;
using System.Linq;
using Core.Repositories;
using System.Reflection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using System.Collections.Generic;
using MediatR;
using MediatR.Pipeline;
using Domain.RequestHandlers;
using Microsoft.EntityFrameworkCore;
using Domain.RequestHandlers.Pipelines;

namespace IoC
{
    public class SimpleInjectorBootstrap
    {
        private static readonly Type repoType = typeof(Repository<,>);
        private static readonly Type entityType = typeof(Entity);

        public static void Initialize(Container container)
        {
            container.Register<DbContext>(() =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                var config = container.GetInstance<Microsoft.Extensions.Configuration.IConfiguration>();
                optionsBuilder.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
                return new DataContext(optionsBuilder.Options);
            }, Lifestyle.Scoped);


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
                container.Register(interfaceType, implementationType, Lifestyle.Scoped);


            var typesToRegister = repoType.GetTypeInfo().Assembly.ExportedTypes.Select(t => t.GetTypeInfo());

            var registrations = from type in typesToRegister
                                let @interface = type.ImplementedInterfaces.FirstOrDefault(inter => inter.Name == $"I{type.Name}")
                                where @interface != null && type.IsClass && !type.IsGenericType
                                select (@interface, type.AsType());
            
            foreach (var (@interface, @class) in registrations)
                container.Register(@interface, @class, Lifestyle.Scoped);

            container.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));

            ConfigureValidators(container);

            container.RegisterSingleton(() => GetMapper(container));

            ConfigureMediatR(container);
        }

        private static void ConfigureValidators(Container container)
        {
            var baseType = typeof(AbstractValidator<>);
            var validatorTypes = entityType.Assembly.ExportedTypes
                    .Where(t => t.IsClass && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == baseType)
                    .ToArray();

            foreach (var type in validatorTypes)
                container.Register(type.BaseType, type, Lifestyle.Scoped);
        }


        private static IMapper GetMapper(Container container)
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(container.GetInstance);


            mce.AddMaps(repoType.Assembly);
            mce.AddExpressionMapping();

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc, t => container.GetInstance(t));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(CreateUserRequestHandler).GetTypeInfo().Assembly;
        }
        private static void ConfigureMediatR(Container container)
        {
            var assemblies = GetAssemblies().ToArray();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);

            // we have to do this because by default, generic type definitions (such as the Constrained Notification Handler) won't be registered
            var notificationHandlerTypes = container.GetTypesToRegister(typeof(INotificationHandler<>), assemblies, new TypesToRegisterOptions
            {
                IncludeGenericTypeDefinitions = true,
                IncludeComposites = false,
            });

            //Notification
            container.Collection.Register(typeof(INotificationHandler<>), notificationHandlerTypes);

            //Pipeline
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
                typeof(ExceptionPipelineBehavior<,>),
                typeof(ValidationPipelineBehavior<,>)
            });

            container.Collection.Register(typeof(IRequestPreProcessor<>));
            container.Collection.Register(typeof(IRequestPostProcessor<,>));

            container.Register(() => new ServiceFactory(container.GetInstance), Lifestyle.Singleton);
        }
    }
}
