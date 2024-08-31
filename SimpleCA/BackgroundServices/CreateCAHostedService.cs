using SimpleCA.Core.Models;
using SimpleCA.Core.Ports;
using System.Security.Cryptography.X509Certificates;

namespace SimpleCA.API.BackgroundServices
{
    public class CreateCAHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public CreateCAHostedService (IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var ecap = scope.ServiceProvider.GetRequiredService<IEnsureCACreatedPort>();
                ecap.EnsureCACreated();

                var ecrlp = scope.ServiceProvider.GetRequiredService<IEnsureCrlCreatedPort>();
                ecrlp.EnsureCrlCreated();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
