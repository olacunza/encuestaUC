using AssesmentUC.Infrastructure.Data;
using AssesmentUC.Infrastructure.Repository.Impl;
using AssesmentUC.Infrastructure.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssesmentUC.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("BDPRACTICAS"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddDbContext<BannerDbContext>(options =>
                options.UseOracle(
                    configuration.GetConnectionString("BANNER"),
                    oracleOptions =>
                    {
                        oracleOptions.CommandTimeout(300);
                    }));

            services.AddScoped<IEncuestaRepository, EncuestaRepository>();
            services.AddScoped<IRespuestaRepository, RespuestaRepository>();
            services.AddScoped<IReporteRepository, ReporteRepository>();

            return services;
        }
    }
}