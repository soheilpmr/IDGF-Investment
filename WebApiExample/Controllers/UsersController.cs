using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Auth.Models.Dtos;
using IDGF.Auth.Services.AdminServices;
using IDGFAuth.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace IDGFAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityAdminService<IdentityRole, ApplicationUser> _identityAdminService;
        public UsersController(UserManager<ApplicationUser> userManager, IIdentityAdminService<IdentityRole, ApplicationUser> identityAdminService)
        {
            _userManager = userManager;
            _identityAdminService = identityAdminService;
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

        [HttpPost(nameof(Register))]
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

        [HttpGet(nameof(GetAllUsers))]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = new List<object>();

                foreach (var user in _userManager.Users.ToList())
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    result.Add(new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        Roles = roles
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }   
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("ChangePasswordForMyself")]
        public async Task<ActionResult> ChangePasswordForMyself(ChangePasswordOfMySelfDto dto)
        {
            var userID = User.Claims.Where(ss => ss.Type == "sub").Select(ss => ss.Value).FirstOrDefault();
            if(userID == null)
                return Unauthorized("User ID not found in token.");
            if (string.IsNullOrWhiteSpace(userID))
            {
                return BadRequest("UserID Is Not Defined");
            }
            if (string.IsNullOrWhiteSpace(dto.OldPassword))
            {
                return BadRequest("OldPassword Not Definedn To be Change");
            }
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("NewPasword Not Definedn To be Change");
            }
            if (string.IsNullOrWhiteSpace(dto.RepeatNewPassword))
            {
                return BadRequest("RepeatnewPassword Not Definedn To be Change");
            }
            try
            {
                var result = await _identityAdminService.ChangePasswordForMyselfAsync(userID,
                 dto.OldPassword, dto.NewPassword, dto.RepeatNewPassword);

                return Ok("User Cahnge Password Set Successfully");
            }
            catch (ServiceException ex)
            {
                if (ex is ServiceObjectNotFoundException)
                {
                    return BadRequest(ex.ToServiceExceptionString());
                }
                else if (ex is UserException)
                {
                    return StatusCode(500, ex.ToServiceExceptionString());
                }
                else if (ex is PasswordValidateException)
                {
                    return StatusCode(500, ex.ToServiceExceptionString());
                }
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
public record RegisterUserDto(string UserName, string Email, string Password, string firstName, string lastName);

public class AddRolesToUserDto
{
    public string userId { get; set; }
    public string roleName { get; set; }
}
