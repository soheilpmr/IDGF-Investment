using IDGFAuth.Models.Dtos;
using IDGFAuth.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace IDGFAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private readonly IAuthService _authService; 
        private readonly ILogger<UserServiceController> _logger;

        public UserServiceController(IAuthService authService, ILogger<UserServiceController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _authService.RequestPasswordResetAsync(dto);
                return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unexpected error during forgot password request for {Email}", dto.Email);
                return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(dto);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Password has been reset successfully." });
                }
                return BadRequest(new { message = "Password reset failed. Please check the token and password requirements." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset for {Email}", dto.Email);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }

}
