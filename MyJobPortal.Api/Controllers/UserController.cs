using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService   _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequestDto request)
    {
       var result = await _userService.CreateUserAsync(request);
        return Ok(result);
    }
}