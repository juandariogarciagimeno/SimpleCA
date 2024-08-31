namespace SimpleCA.Core.Models
{
    public class CertRequest
    {
        public static string[] Subject = [ "CN", "O", "OU", "S", "L", "C", "E" ];

        public CertRequest() { }

        public CertRequest(Dictionary<string,string> subject)
        {
            CN = subject.TryGetValue("CN", out string? v) ? v : string.Empty;
            O = subject.TryGetValue("O", out v) ? v : string.Empty;
            OU = subject.TryGetValue("OU", out v) ? v : string.Empty;
            S = subject.TryGetValue("S", out v) ? v : string.Empty;
            L = subject.TryGetValue("L", out v) ? v : string.Empty;
            C = subject.TryGetValue("C", out v) ? v : string.Empty;
            E = subject.TryGetValue("E", out v) ? v : string.Empty;
        }

        /// <summary>
        /// Gets or sets the Common Name.
        /// </summary>
        public string CN { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Organization.
        /// </summary>
        public string O { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Organizational Unit.
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

        public override string ToString()
        {
            return string.Format("CN={0}, O={1}, OU={2}, L={3}, S={4}, C={5}, E={6}", CN, O, OU, L, S, C, E);
        }

        public static CertRequest ReadCAFromEnv()
        {
            Dictionary<string, string> subject = new Dictionary<string, string>();
            foreach (var s in Subject)
            {
                subject.Add(s, Environment.GetEnvironmentVariable($"CA_{s}") ?? string.Empty);
            }

            var carequest = new CertRequest(subject);
            return carequest;
        }
    }
}
