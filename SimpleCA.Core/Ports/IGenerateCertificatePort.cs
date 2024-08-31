using SimpleCA.Core.Models;

namespace SimpleCA.Core.Ports
{
    public interface IGenerateCertificatePort
    {
        CertificateData GenerateCertificate(CertRequest certrequest);
    }
}
