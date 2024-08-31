using Microsoft.Extensions.Logging;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Ports;

namespace SimpleCA.UseCases.CA
{
    public class GetCAPublicPort : IGetCAPublicPort
    {
        private ICertificateService certService;
        private ILogger<IGetCAPublicPort> logger;

        public GetCAPublicPort(ICertificateService certService, ILogger<IGetCAPublicPort> logger)
        {
            this.certService = certService;
            this.logger = logger;
        }

        public byte[] GetCAPublic()
        {
            try
            {
                return certService.GetCACert();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting CA public key");
                throw;
            }
            finally
            {
                this.logger.LogInformation("OUT <- GetCAPublic");
            }
        }
    }
}
