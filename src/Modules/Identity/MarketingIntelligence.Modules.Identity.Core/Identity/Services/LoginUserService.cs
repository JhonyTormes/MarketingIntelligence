using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Services
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHasher _passwordHasher;
        public LoginUserService(
            IUserCredentialRepository userCredentialRepository,
            ITokenProvider tokenProvider,
            IPasswordHasher passwordHasher  
            )
        {
            _userCredentialRepository = userCredentialRepository;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var credential = await _userCredentialRepository.GetByEmailAsync(email);

            if (credential == null || !_passwordHasher.Verify(password, credential.PasswordHash))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            return _tokenProvider.GenerateToken(credential);
        }
    }
}
