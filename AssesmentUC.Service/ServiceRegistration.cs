using AssesmentUC.Service.Service.Impl;
using AssesmentUC.Service.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesmentUC.Service
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEncuestaService, EncuestaService>();
            services.AddScoped<IRespuestaService, RespuestaService>();
            return services;
        }
    }
}
