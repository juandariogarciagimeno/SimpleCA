using Microsoft.Extensions.DependencyInjection;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;
using SimpleCA.Services;
using SimpleCA.UseCases.CA;
using SimpleCA.UseCases.Certificate;

namespace SimpleCA.IoC
{
    public static class DepencencyContainer
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<IEnsureCACreatedPort, EnsureCACreatedPort>();
            services.AddScoped<IGenerateCertificatePort, GenerateCertificatePort>();
            services.AddScoped<IGetCAPublicPort, GetCAPublicPort>();
            services.AddScoped<IEnsureCrlCreatedPort, EnsureCrlCreatedPort>();
            services.AddScoped<IGetCrlPort, GetCrlPort>();
            services.AddScoped<IRevokeCertPort, RevokeCertPort>();
            services.AddScoped<IVerifyOcspPort, VerifyOcspPort>();
            
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICertificateService, CertificateService>();

            return services;
        }
    }
}
