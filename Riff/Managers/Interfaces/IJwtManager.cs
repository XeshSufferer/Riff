using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Managers
{
    public interface IJwtManager
    {
        Task SetGlobalJwt(string token);
    }
}
