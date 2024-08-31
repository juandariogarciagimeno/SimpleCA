namespace SimpleCA.Dtos.V1.Certificate.Generate
{
    public class GenerateCertificateRequest
    {
        /// <summary>
        /// Gets or sets the Common Name.
        /// </summary>
        public string CN { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Organization.
        /// </summary>
        public string O {  get; set; } = null!;

        /// <summary>
        /// Gets or sets the Organization Unit.
        /// </summary>
        public string OU { get; set; } = null!;

        /// <summary>
        /// Gets or sets the State.
        /// </summary>
        public string S { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Locality.
        /// </summary>
        public string L { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string C { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public string E { get; set; } = null!;
    }
}
