using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Auth.Models.Dtos;
using IDGF.Auth.Services.AdminServices;
using IDGFAuth.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
    public class AdminServiceController : ControllerBase
    {
        private readonly IIdentityAdminService<IdentityRole, ApplicationUser> _identityAdminService;
        private readonly ILogger<AdminServiceController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminServiceController(
            IIdentityAdminService<IdentityRole, ApplicationUser> identityAdminService
            , UserManager<ApplicationUser> userManager)
        {
            _identityAdminService = identityAdminService;
            _userManager = userManager;
        }
        [HttpPost]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {

            if (string.IsNullOrWhiteSpace(changePasswordDto.UserID))
            {
                return BadRequest("User Is Not Defined");
            }

            if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
            {
                return BadRequest("NewPassword Not Definedn To be Change");
            }

            try
            {
                var result = await _identityAdminService.ChangePasswordOfUserAsync(changePasswordDto.UserID,
                 changePasswordDto.NewPassword);

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
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
