namespace SimpleCA.Core.Ports
{
    public interface IRevokeCertPort
    {
        void RevokeCert(byte[] certpk);
    }
}
