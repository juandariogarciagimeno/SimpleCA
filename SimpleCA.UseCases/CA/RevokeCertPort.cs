using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class RevokeCertPort : IRevokeCertPort
    {
        private readonly ICertificateService certService;
        private readonly ILogger<IRevokeCertPort> logger;

        public RevokeCertPort(ICertificateService certService, ILogger<IRevokeCertPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public void RevokeCert(byte[] certpk)
        {
            try
            {
                this.logger.LogInformation("IN -> RevokeCert");
                certService.Revoke(certpk);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error revocating cert");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- RevokeCert");
            }
        }
    }
}
