using Domain.Models.Validations;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WebApi
{
    public partial class Startup
    {
        /// <summary>
        /// Configures Fluent Validation
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureValidators(IServiceCollection services)
        {
            // Transient validators are the ones requiring scoped injections (e.g.: Repository).
            var transientValidators = new Type[]
            {
                typeof(CreateUserRequestValidator)
            };

            // By default, validators are injected as singletons for better performance.
            services
                    .AddMvcCore()
                    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateUserRequestValidator>(filter: scan => transientValidators.Contains(scan.ValidatorType), lifetime: ServiceLifetime.Transient))
                    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<CreateUserRequestValidator>(filter: scan => !transientValidators.Contains(scan.ValidatorType), lifetime: ServiceLifetime.Singleton));
        }
    }

}
