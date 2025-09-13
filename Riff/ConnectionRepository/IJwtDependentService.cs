using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.ConnectionRepository
{
    public interface IJwtDependentService
    {
        Task SetJwtToken(string token);
    }
}
