using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SimpleCA.Controllers.Filters;
using SimpleCA.Core.Helpers;
using SimpleCA.Core.Models;
using SimpleCA.Core.Ports;
using SimpleCA.Dtos.V1.Certificate.Generate;
using SimpleCA.Dtos.V1.Certificate.GetCA;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleCA.Controllers.V1
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CertificateController : ControllerBase
    {
        private readonly IGenerateCertificatePort genCert;
        private readonly IGetCAPublicPort getCA;

        public CertificateController(IGenerateCertificatePort genCert, IGetCAPublicPort getCA)
        {
            this.genCert = genCert;
            this.getCA = getCA;
        }

        [HttpPost("generate")]
        [MapToApiVersion(1)]
        [ProducesResponseType(typeof(GenerateCertificateResponse), 200)]
        [ProduceFileDescriptor("application/zip", "certs.zip", "A zip file containing the generated certificate")]
        [Produces("application/json", "application/zip")]
        public IActionResult Generate(GenerateCertificateRequest request)
        {
            var certRequest = new CertRequest()
            {
                CN = request.CN,
                O = request.O,
                OU = request.OU,
                C = request.C,
                L = request.L,
                S = request.S,
                E = request.E
            };

            var certData = this.genCert.GenerateCertificate(certRequest);

            if (HttpContext.Request.Headers.Accept == "application/json")
            {
                var response = new GenerateCertificateResponse()
                {
                    Crt = certData.Crt,
                    Pfx = certData.Pfx,
                    Password = certData.Password
                };

                return Ok(response);
            } 
            else if (HttpContext.Request.Headers.Accept == "application/zip")
            {
                Dictionary<string, byte[]> files = new()
                {
                    { "cert.pfx", certData.Pfx },
                    { "cert.crt", certData.Crt },
                    { "pwd.txt", Encoding.UTF8.GetBytes(certData.Password) }
                };

                var zip = ZipHelper.ZipFiles(files);

                return File(zip, "application/zip", "cert.zip");
            } 
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("getca")]
        [MapToApiVersion(1)]
        [ProducesResponseType(typeof(GetCAResponse), 200)]
        [ProduceFileDescriptor("application/x-x509-ca-cert", "ca.crt", "Public Key of the CA")]
        [Produces("application/json", "application/x-x509-ca-cert")]
        public IActionResult CetCA()
        {
            var ca = getCA.GetCAPublic();

            if (HttpContext.Request.Headers.Accept == "application/json")
            {
                var response = new GetCAResponse()
                {
                    CACrt = ca
                };

                return Ok(response);
            }
            else if (HttpContext.Request.Headers.Accept == "application/x-x509-ca-cert")
            {
                return File(ca, "application/x-x509-ca-cert", "ca.crt");
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
