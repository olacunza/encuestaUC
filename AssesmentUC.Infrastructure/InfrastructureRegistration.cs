using AssesmentUC.Infrastructure.Repository.Impl;
using AssesmentUC.Infrastructure.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssesmentUC.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IEncuestaRepository, EncuestaRepository>();
            services.AddScoped<IRespuestaRepository, RespuestaRepository>();

            return services;
        }
    }
}
