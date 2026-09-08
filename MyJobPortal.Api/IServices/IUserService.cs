public interface IUserService
{
    Task<string> CreateUserAsync (UserCreateRequestDto request);
}