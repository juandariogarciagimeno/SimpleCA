using SimpleCA.Core.Models;
namespace SimpleCA.Core.IServices
{
    public interface ICertificateService
    {
        void EnsureCACreated();
        void EnsureCrlCreated();
        CertificateData GenerateCertificate(CertRequest certrequest);
        byte[] GetCACert();
        byte[] GetCrl();
        void Revoke(byte[] certPK);
    }
}
