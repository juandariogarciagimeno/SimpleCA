using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCA.Core.Ports
{
    public interface IGetCAPublicPort
    {
        byte[] GetCAPublic();
    }
}
