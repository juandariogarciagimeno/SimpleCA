using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleCA.Controllers.Filters;
using SimpleCA.Core.Ports;
using SimpleCA.Dtos.V1.Certificate.Generate;
using SimpleCA.Dtos.V1.Revocation.Revoke;

namespace SimpleCA.Controllers.V1
{
    [ApiController]
    [ApiVersion(1)]
    [Route("[controller]")]
    public class RevocationController : ControllerBase
    {
        private readonly IGetCrlPort getCrl;
        private readonly IRevokeCertPort revokeCert;

        public RevocationController(IGetCrlPort getCrl, IRevokeCertPort revokeCert)
        {
            this.getCrl = getCrl;
            this.revokeCert = revokeCert;
        }

        [HttpGet("revocation.crl")]
        [MapToApiVersion(1)]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [Produces("application/pkix-cr")]
        [ProduceFileDescriptor("application/pkix-cr", "revocation.crl", "The Certificate Revocation List")]
        public IActionResult Generate()
        {
            return File(getCrl.GetCrl(), "application/pkix-crl", "revocation.crl");
        }

        [HttpPost("revoke/crt")]
        [MapToApiVersion(1)]
        [ProducesResponseType(200)]
        [Consumes("multipart/form-data")]
        public IActionResult Revoke(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var data = ms.ToArray();
                revokeCert.RevokeCert(data);
            }
            return Ok();
        }

        [HttpPost("revoke/json")]
        [MapToApiVersion(1)]
        [ProducesResponseType(200)]
        [Consumes("application/json")]
        public IActionResult Revoke(RevokeRequest request)
        {
            revokeCert.RevokeCert(request.certpk);
            return Ok();
        }
    }
}
