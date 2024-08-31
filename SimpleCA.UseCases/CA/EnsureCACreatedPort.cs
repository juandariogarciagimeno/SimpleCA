using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class EnsureCACreatedPort : IEnsureCACreatedPort
    {
        private ICertificateService certService;
        private readonly ILogger<IEnsureCACreatedPort> logger;

        public EnsureCACreatedPort(ICertificateService certService, ILogger<IEnsureCACreatedPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public void EnsureCACreated()
        {
            this.logger.LogInformation("IN -> Ensure CA Created");
            try
            {
                this.certService.EnsureCACreated();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating CA");
            }

            this.logger.LogInformation("OUT <- Ensure CA Created");
        }
    }
}
