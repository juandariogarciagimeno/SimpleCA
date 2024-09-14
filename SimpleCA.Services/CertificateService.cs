using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SimpleCA.Core.IServices;
using SimpleCA.Core.Models;
using System;
using System.Dynamic;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using BCBigInteger = Org.BouncyCastle.Math.BigInteger;
using X509Exts = Org.BouncyCastle.Asn1.X509.X509Extensions;
using X509Extension = System.Security.Cryptography.X509Certificates.X509Extension;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SimpleCA.Services
{
    public class CertificateService : ICertificateService
    {
        private static string LocalFolder { get { return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simpleca"); } }
        private static string CAFilePfx { get { return Path.Join(LocalFolder, "ca.pfx"); } }
        private static string CAFileCer { get { return Path.Join(LocalFolder, "ca.cer"); } }
        private static string CRLFile { get { return Path.Join(LocalFolder, "revocation.crl"); } }

        private void EnsureLocalPathCreated()
        {
            if (!string.IsNullOrEmpty(LocalFolder))
            {
                if (!Directory.Exists(LocalFolder))
                {
                    Directory.CreateDirectory(LocalFolder);
                }
            }
        }

        public void EnsureCACreated()
        {
            EnsureLocalPathCreated();

            if (File.Exists(CAFilePfx) && VerifyPassword())
                return;

            File.Delete(CAFilePfx);
            var cacert = GenerateCA();
            if (cacert == null)
                throw new Exception("Error while generating CA Certificate");

            string pfxPassword = Environment.GetEnvironmentVariable("CA_PWD") ?? string.Empty;
            byte[] pfxBytes = cacert.Export(X509ContentType.Pfx, pfxPassword);
            File.WriteAllBytes(CAFilePfx, pfxBytes);

            byte[] cerBytes = cacert.Export(X509ContentType.Cert);
            File.WriteAllBytes(CAFileCer, cerBytes);
        }

        public void EnsureCrlCreated()
        {
            EnsureCACreated();

            if (File.Exists(CRLFile))
                return;

            var crl = new CertificateRevocationListBuilder();
            SaveCrl(crl);
        }

        private void SaveCrl(CertificateRevocationListBuilder crl, BigInteger? crlCount = null)
        {
            DateTime thisUpdate = DateTime.UtcNow;
            DateTime nextUpdate = thisUpdate.AddDays(30);
            var ca = GetCA();

            var crldata = crl.Build(ca, crlCount ?? BigInteger.Zero, nextUpdate, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1, thisUpdate);

            File.WriteAllBytes(CRLFile, crldata);
        } 

        private X509Certificate2 GenerateCA()
        {
            using (RSA rsa = RSA.Create(4096))
            {
                var certRequest = CertRequest.ReadCAFromEnv();
                var distinguishedName = new X500DistinguishedName(certRequest.ToString());

                var certificateRequest = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
                certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));

                var ski = new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false);
                certificateRequest.CertificateExtensions.Add(ski);
                certificateRequest.CertificateExtensions.Add(new X509Extension(new AsnEncodedData(new Oid("2.5.29.35"), ski.RawData), false));

                var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(10));

                return certificate;
            }
        }

        public byte[] VerifyOcsp(byte[] request)
        {
            var capk = GetCAAsymetrycKeyParameter();
            var bcCa = DotNetUtilities.FromX509Certificate(GetPublicCA());

            var ocspReq = new OcspReq(request);
            var ocspRespGen = new BasicOcspRespGenerator(new RespID(bcCa.SubjectDN));
            var certs = ocspReq.GetRequestList();

            var crl = GetCrlObj();

            foreach (var cert in certs)
            {
                var certid = cert.GetCertID();

                var entry = crl.GetRevokedCertificate(certid.SerialNumber);

                if (entry == null)
                {
                    ocspRespGen.AddResponse(certid, RevokedStatus.Good);
                    continue;
                }

                Asn1OctetString reasonExtensionValue = entry.GetExtensionValue(X509Exts.ReasonCode);
                DerEnumerated reasonCode = (DerEnumerated)Asn1Object.FromByteArray(reasonExtensionValue.GetOctets());
                int reasonValue = reasonCode.Value.IntValue;

                ocspRespGen.AddResponse(certid, new RevokedStatus(entry.RevocationDate, reasonValue));
            }

            var bresp = ocspRespGen.Generate("SHA256WithRSAEncryption", capk, null, DateTime.UtcNow);
            var ocspgen = new OCSPRespGenerator();
            var ocsp = ocspgen.Generate(OcspRespStatus.Successful, bresp);
            return ocsp.GetEncoded();
        }

        private AsymmetricKeyParameter GetCAAsymetrycKeyParameter()
        {
            using (FileStream fs = new FileStream(CAFilePfx, FileMode.Open, FileAccess.Read))
            {
                // Parse the PKCS#12 store
                var b = new Pkcs12StoreBuilder();
                var pkcs12 = b.Build();
                pkcs12.Load(fs, "Abc123".ToCharArray());

                // Iterate over all aliases in the PFX
                foreach (string alias in pkcs12.Aliases)
                {
                    // Find the private key entry
                    if (pkcs12.IsKeyEntry(alias))
                    {
                        // Extract the private key as AsymmetricKeyParameter
                        AsymmetricKeyEntry keyEntry = pkcs12.GetKey(alias);
                        return keyEntry.Key;
                    }
                }
            }

            return null;
        }

        private bool VerifyPassword()
        {
            X509Certificate2 cert = null;
            try
            {
                var pfx = File.ReadAllBytes(CAFilePfx);
                var password = Environment.GetEnvironmentVariable("CA_PWD");
                cert = new X509Certificate2(pfx, password);
            }
            catch (CryptographicException ex)
            {
                if ((ex.HResult & 0xFFFF) == 0x56)
                {
                    return false;
                };

                throw;
            }
            finally
            {
                cert?.Dispose();
            }

            return true;
        }

        private X509Certificate2 GetCA()
        {
            var pfx = File.ReadAllBytes(CAFilePfx);
            var pwd = Environment.GetEnvironmentVariable("CA_PWD");

            return new X509Certificate2(pfx, pwd, X509KeyStorageFlags.Exportable);
        }

        public X509Certificate2 GetPublicCA()
        {
            var crt = File.ReadAllBytes(CAFileCer);

            return new X509Certificate2(crt);
        }

        public byte[] GetCACert()
        {
            return File.ReadAllBytes(CAFileCer);
        }

        public byte[] GetCrl()
        {
            return File.ReadAllBytes(CRLFile);
        }

        public CertificateData GenerateCertificate(CertRequest certrequest)
        {
            using (RSA rsa = RSA.Create(2048))
            {
                var ca = GetCA();

                var distinguishedName = new X500DistinguishedName(certrequest.ToString());

                var certificateRequest = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false)); // Not a CA
                certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DataEncipherment, true));
                //certificateRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, true)); // Client Authentication
                certificateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

                // Authority Key Identifier
                //var aki = new X509SubjectKeyIdentifierExtension(ca.PublicKey, false);
                //certificateRequest.CertificateExtensions.Add(new X509Extension(new AsnEncodedData(new Oid("2.5.29.35"), aki.RawData), false));

                var hosts = Environment.GetEnvironmentVariable("CRL_HOSTS")?.Split(';') ?? ["http://localhost"];
                var hostscrl = hosts.Select(x => $"{x}/revocation/revocation.crl").ToArray();
                var hostocsp = hosts.Select(x => $"{x}/revocation/ocsp").ToArray();

                certificateRequest.CertificateExtensions.Add(CertificateRevocationListBuilder.BuildCrlDistributionPointExtension(hostscrl));
                certificateRequest.CertificateExtensions.Add(CreateOcspAiaExtension(hostocsp));

                var serial = Guid.NewGuid().ToByteArray();
                var certificate = certificateRequest.Create(ca, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(3), serial);

                certificate = certificate.CopyWithPrivateKey(rsa);

                //X509Certificate2Collection chain = [capublic, certificate];

                var pwd = GenPwd(8);
                var pfx = certificate.Export(X509ContentType.Pfx, pwd);
                var crt = certificate.Export(X509ContentType.Cert);

                return new CertificateData()
                {
                    Pfx = pfx,
                    Crt = crt,
                    Password = pwd
                };
            }
        }

        private X509Extension CreateOcspAiaExtension(string[] ocspUrls)
        {
            // Create an ASN.1 Sequence to hold OCSP access descriptions
            var aiaSeq = new Asn1EncodableVector();

            foreach (string ocspUrl in ocspUrls)
            {
                // Create an access description for each OCSP URL
                var accessDescription = new AccessDescription(
                    AccessDescription.IdADOcsp,
                    new GeneralName(GeneralName.UniformResourceIdentifier, ocspUrl)
                );

                aiaSeq.Add(accessDescription);
            }

            // Encode the sequence as an Authority Information Access extension
            var aiaExtension = new DerSequence(aiaSeq);
            return new X509Extension(new AsnEncodedData(new Oid("1.3.6.1.5.5.7.1.1"), aiaExtension.GetEncoded()), false);
        }

        public void Revoke(byte[] certpk)
        {
            EnsureCrlCreated();

            var currCrlData = GetCrl();

            var currentCrl = CertificateRevocationListBuilder.Load(currCrlData, out BigInteger currentCrlNumber);

            var certToRevoke = new X509Certificate2(certpk);

            currentCrl.AddEntry(certToRevoke, DateTimeOffset.UtcNow, X509RevocationReason.CessationOfOperation);
            currentCrlNumber++;

            SaveCrl(currentCrl, currentCrlNumber);
        }

        private X509Crl GetCrlObj()
        {
            var currCrlData = GetCrl();

            X509CrlParser crlParser = new X509CrlParser();
            X509Crl x509Crl = crlParser.ReadCrl(currCrlData);
            return x509Crl;
        }

        private string GenPwd(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+-.()$#@!,";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
