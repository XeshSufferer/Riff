using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Services.Interfaces
{
    public interface IAccountService
    {
        public event Action OnRegisterFailed;
        public event Action<string> OnRegisterSuccess;
        public event Action OnLoginFailed;
        public event Action<string> OnLoginSuccess;

        Task Register(string name, string login, string password);
        Task Login(string login, string password);
    }
}
