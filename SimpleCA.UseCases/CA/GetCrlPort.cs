using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class GetCrlPort : IGetCrlPort
    {
        private readonly ICertificateService certService;
        private readonly ILogger<IGetCrlPort> logger;

        public GetCrlPort(ICertificateService certService, ILogger<IGetCrlPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public byte[] GetCrl()
        {
            try
            {
                this.logger.LogInformation("IN -> GetCrl");
                return certService.GetCrl();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting CRL");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- GetCrl");
            }
        }
    }
}
