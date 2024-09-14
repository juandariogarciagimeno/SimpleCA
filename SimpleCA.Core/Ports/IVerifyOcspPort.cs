namespace SimpleCA.Core.Ports
{
    public interface IVerifyOcspPort
    {
        byte[] VerifyOcsp(byte[] data);
    }
}
