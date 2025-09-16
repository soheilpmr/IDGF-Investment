using IDGFAuth.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        [HttpPost(nameof(AddRolesToUser))]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> AddRolesToUser(AddRolesToUserDto addRolesToUserDto)
        {
            List<string> rtn = new List<string>()
            {
                addRolesToUserDto.roleName
            };
            var user = await _userManager.FindByIdAsync(addRolesToUserDto.userId);
            if (user == null)
                return NotFound($"User with ID '{addRolesToUserDto.userId}' not found.");

            var result = await _userManager.AddToRolesAsync(user, rtn);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Roles {string.Join(", ", rtn)} added to user '{user.UserName}'.");
        }

        [HttpPost("register")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = false,
                FirstName = dto.firstName,
                LastName = dto.lastName,
                AccessFailedCount = 0,
                LockoutEnabled = false,
                TwoFactorEnabled = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "User registered successfully", user.Id, user.UserName });
        }
    }
}
public record RegisterUserDto(string UserName, string Email, string Password, string firstName, string lastName);

public class AddRolesToUserDto
{
    public string userId { get; set; }
    public string roleName { get; set; }
}
