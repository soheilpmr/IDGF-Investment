
using IDGFAuth.Data.Entities;
using IDGFAuth.Services.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IDGFAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _jWTService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _hostingEnvironment;
        public AuthController(IJWTService jWTService, UserManager<ApplicationUser> userManager, Microsoft.Extensions.Hosting.IHostingEnvironment configuration)
        {
            _jWTService = jWTService;
            _userManager = userManager;
            _hostingEnvironment = configuration;
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequestDTO request)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var user = await _userManager.FindByNameAsync(request.username);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.password))
                {
                    return Unauthorized("Invalid username or password");
                }

                // Authentication successful, generate JWT
                var token = await _jWTService.GenerateToken(user);
                return Ok(new { Token = token, Result = "Successfully Authorized" });
            }
            else
            {
                if (request.username != "admin@gmail.com" && request.password != "Admin123*")
                {
                    return Unauthorized("Invalid username or password");
                }
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.UserName = request.username;
                applicationUser.Id = "27d131eb-0da6-4d8e-a70d-8ba9d8536810";
                // Authentication successful, generate JWT
                var token = await _jWTService.GenerateToken(applicationUser);
                return Ok(new { Token = token, Result = "Successfully Authorized" });
            }

        }
        
        [HttpPost(nameof(LogOut))]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                await _userManager.UpdateSecurityStampAsync(user);

                return Ok(new { message = "User logged out successfully. All existing tokens are now invalid." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
    public record LoginRequestDTO(string username, string password);
    //public class TokenGenerationRequest
    //{
    //    public string UserID { get; set; }
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //}
}
