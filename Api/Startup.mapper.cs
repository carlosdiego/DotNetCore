using System;
using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.DependencyInjection;


namespace WebApi
{
    public partial class Startup
    {
        /// <summary>
        /// Configures auto mapper.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureMapper(IServiceCollection services)
        {
            IMapper GetMapper(IServiceProvider serviceProvider)
            {
                var mce = new MapperConfigurationExpression();
                mce.ConstructServicesUsing(serviceProvider.GetService);

                mce.AddMaps(repoType.Assembly);
                mce.AddExpressionMapping();

                var mc = new MapperConfiguration(mce);

                // Throws an exception should any mapping be invalid.
                mc.AssertConfigurationIsValid();

                return new Mapper(mc, t => serviceProvider.GetService(t));
            }

            services.AddSingleton(provider => GetMapper(provider));
        }
    }

}
