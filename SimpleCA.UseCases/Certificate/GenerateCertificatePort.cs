using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Models;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.Certificate
{
    public class GenerateCertificatePort : IGenerateCertificatePort
    {
        private readonly ICertificateService certService;
        private readonly ILogger<IGenerateCertificatePort> logger;

        public GenerateCertificatePort(ICertificateService certService, ILogger<IGenerateCertificatePort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public CertificateData GenerateCertificate(CertRequest certrequest)
        {
            this.logger.LogInformation("IN -> GenerateCertificate");
            try
            {
                return this.certService.GenerateCertificate(certrequest);
            } 
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error generating certificate");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- GenerateCertificate");
            }
        }
    }
}
