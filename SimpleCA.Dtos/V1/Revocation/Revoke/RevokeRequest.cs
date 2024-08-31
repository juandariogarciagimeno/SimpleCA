using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCA.Dtos.V1.Revocation.Revoke
{
    public class RevokeRequest
    {
        public byte[] certpk { get; set; }
    }
}
