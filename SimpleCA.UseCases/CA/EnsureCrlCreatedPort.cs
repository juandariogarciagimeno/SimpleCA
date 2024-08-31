using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class EnsureCrlCreatedPort : IEnsureCrlCreatedPort
    {
        private readonly ICertificateService certService;
        private readonly ILogger<IEnsureCrlCreatedPort> logger;

        public EnsureCrlCreatedPort(ICertificateService certService, ILogger<IEnsureCrlCreatedPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }
        public void EnsureCrlCreated()
        {
            try
            {
                this.logger.LogInformation("IN -> EnsureCrlCreated");
                certService.EnsureCrlCreated();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating CRL");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- EnsureCrlCreated");
            }
        }
    }
}
