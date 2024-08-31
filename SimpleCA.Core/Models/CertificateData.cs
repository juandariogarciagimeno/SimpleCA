namespace SimpleCA.Core.Models
{
    public class CertificateData
    {
        /// <summary>
        /// Gets or sets the PFX Data.
        /// </summary>
        public byte[] Pfx { get; set; } = null!;

        /// <summary>
        /// Gets or sets the certificate public key (crt).
        /// </summary>
        public byte[] Crt { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Certificate's password.
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
