using IDGFAuth.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IDGFAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddRolesToUser(string userId, [FromBody] List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"User with ID '{userId}' not found.");

            var result = await _userManager.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Roles {string.Join(", ", roles)} added to user '{user.UserName}'.");
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully", user.Id, user.UserName });
        }
    }
}
public record RegisterUserDto(string UserName, string Email, string Password);
