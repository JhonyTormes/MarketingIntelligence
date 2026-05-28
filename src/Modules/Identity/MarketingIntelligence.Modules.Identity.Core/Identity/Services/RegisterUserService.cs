using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;
using MarketingIntelligence.Shared.Contracts;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;

public class RegisterUserService : IRegisterUserService
{
    private readonly IUserCredentialRepository _credentialRepo;
    private readonly IUserRepository _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserService(
        IUserCredentialRepository credentialRepo,
        IUserRepository profileRepo,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _credentialRepo = credentialRepo;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> RegisterAsync(string email, string password, string firstName, string lastName, string taxPayerId, string phoneNumber)
    {
        string hashedPassword = _passwordHasher.Hash(password);

        var credential = new UserCredential(email, hashedPassword); // ID generated here
        var profile = new User(credential.Id, firstName, lastName, taxPayerId, phoneNumber);

        await _credentialRepo.AddAsync(credential);
        await _profileRepo.AddAsync(profile);

        await _unitOfWork.SaveChangesAsync();

        return credential.Id;
    }
}