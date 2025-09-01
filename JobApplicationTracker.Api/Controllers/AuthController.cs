using JobApplicationTracker.Api.Models;
using JobApplicationTracker.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IJwtService _jwtService;

	public AuthController(UserManager<IdentityUser> userManager, IJwtService jwtService)
	{
		_userManager = userManager;
		_jwtService = jwtService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDto dto)
	{
		var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
		var result = await _userManager.CreateAsync(user, dto.Password);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		return Ok();
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDto dto)
	{
		var user = await _userManager.FindByEmailAsync(dto.Email);
		if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
			return Unauthorized();

		var token = _jwtService.GenerateToken(user);
		return Ok(new { token });
	}
}
