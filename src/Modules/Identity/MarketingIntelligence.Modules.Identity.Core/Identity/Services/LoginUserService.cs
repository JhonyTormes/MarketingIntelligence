using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;
using MarketingIntelligence.Shared.Contracts;
using System.Net;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Services
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventPublisher _eventPublisher;

        public LoginUserService(
            IUserCredentialRepository userCredentialRepository,
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IPasswordHasher passwordHasher,
            IEventPublisher eventPublisher
            )
        {
            _userCredentialRepository = userCredentialRepository;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var credential = await _userCredentialRepository.GetByEmailAsync(email);

            if (credential == null || !_passwordHasher.Verify(password, credential.PasswordHash))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            var user = await _userRepository.GetByIdAsync(credential.Id);

            await _eventPublisher.PublishAsync(new UserLogedInEvent(
                 Name: user.FirstName,
                 Email: credential.Email,
                 LogedInAt: DateTime.UtcNow
             ));

            return _tokenProvider.GenerateToken(credential);
        }
    }
}
