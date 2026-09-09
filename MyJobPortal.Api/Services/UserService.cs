using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly IRedisCacheService _cacheService;
    private readonly IEmailService _emailService;
    public UserService(UserManager<ApplicationUser> userManager,
     ILogger<UserService> logger, IRedisCacheService cacheService,
     IEmailService emailService)
    {
        _userManager = userManager;
        _logger = logger;
        _cacheService = cacheService;
        _emailService = emailService;
    }
    public async Task<string> CreateUserAsync(UserCreateRequestDto request)
    {
        _logger.LogInformation("Registration attempt strated for {Email} with Role {Role}",
        request.Email, request.Role);

        var cachekey = $"user:{request.Email}";
        var cachedUser = await _cacheService.GetAsync<ApplicationUser>(cachekey);
        if(cachedUser != null)
        {
            return "Email already exists";
        }
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            await _cacheService.SetAsync(cachekey,existingUser,TimeSpan.FromMinutes(15));
            _logger.LogWarning("Registration failed:Email already exists:{Email}", request.Email);
            return "Email already exists";
        }
        if (request.Role != "JobSeeker" && request.Role != "Employer")
        {
            return "Role must be JobSeeker or Employer";
        }
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FullName,
            LastName = request.FullName,
            CreatedAt = DateTime.UtcNow,
            Role = request.Role,
        };
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed for {Email} with errors: {Errors}",
            request.Email, result.Errors);
            return "Registration failed";
        }
        await _userManager.AddToRoleAsync(user, request.Role);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var body = $"Hello {request.FullName} ,<br><br>" +
                    $"Please use this token: {token} to confirm your email. UserId: {user.Id}";
        await _emailService.SendEmailAsync(request.Email,"Confirm Your Email",body);
        _logger.LogInformation("User registered for {Email}, with ID {Id} and role {Role}",
        user.Email, user.Id, user.Role);
        return "Registration Successfull. Please check your email to verify.";
    }
}