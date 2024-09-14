using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class VerifyOcspPort : IVerifyOcspPort
    {

        private readonly ICertificateService certService;
        private readonly ILogger<IVerifyOcspPort> logger;

        public VerifyOcspPort(ICertificateService certService, ILogger<IVerifyOcspPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public byte[] VerifyOcsp(byte[] data)
        {
            try
            {
                this.logger.LogInformation("IN -> VerifyOcsp");
                return certService.VerifyOcsp(data);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error verifying OCSP");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- VerifyOcsp");
            }
        }
    }
}
