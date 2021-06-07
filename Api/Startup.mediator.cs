using System;
using Domain.RequestHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace WebApi
{
    public partial class Startup
    {
        /// <summary>
        /// Configures MediatR
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureMediator(IServiceCollection services)
        {
            var mediatorImplementations = new Type[]
            {
                typeof(CreateUserRequestHandler),
                typeof(GetUsersRequestHandler)
            };

            services.AddMediatR(mediatorImplementations, config =>
            {
                config.Using<Mediator>().AsScoped();
            });
        }
    }

}
