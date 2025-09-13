using Riff.ConnectionRepository;

namespace Riff.Managers
{
    sealed class JwtManager : IJwtManager
    {

        private readonly IEnumerable<IJwtDependentService> _reposWithJwt;

        public JwtManager(IEnumerable<IJwtDependentService> reposWithJwt)
        {
            _reposWithJwt = reposWithJwt;
        }

        public async Task SetGlobalJwt(string token)
        {
            foreach(var repo in _reposWithJwt)
            {
                if(repo != null)
                {
                    await repo.SetJwtToken(token);
                }
            }
            await SecureStorage.SetAsync("TOKEN", token);
        }


        
    }
}
