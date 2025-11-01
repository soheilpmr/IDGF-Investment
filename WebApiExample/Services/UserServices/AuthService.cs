using IDGFAuth.Data;
using IDGFAuth.Data.Entities;
using IDGFAuth.Models.Dtos;
using IDGFAuth.Services.EmailService;
using IDGFAuth.Services.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace IDGFAuth.Services.UserServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly IDGFAuthDbContextSQL _context;
        public AuthService(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IJWTService jwtService,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IDGFAuthDbContextSQL context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _jwtService = jwtService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
        }

        public async Task RequestPasswordResetAsync(ForgotPasswordDto dto)
        {
            var normalizedEmail = _userManager.NormalizeEmail(dto.Email);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
            {
                _logger.LogWarning("Password reset request for non-existent email: {Email}", dto.Email);
                return;
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var clientAppUrl = _config["ClientAppUrl"];
                var resetLink = $"{clientAppUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={encodedToken}";

                var emailBody = $"لطفا از طریق لینک زیر برای تغییر رمز عبور خود اقدام کنید : <a href='{resetLink}'>Reset Password</a>";
                await _emailService.SendAsync(user.Email, "بازنشانی رمز عبور", emailBody);
                _logger.LogInformation("Password reset email initiated for {Email}", dto.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during password reset request generation for {Email}", dto.Email);
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var normalizedEmail = _userManager.NormalizeEmail(dto.Email);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
            {
                _logger.LogWarning("Password reset attempt for non-existent email: {Email}", dto.Email);
                return IdentityResult.Failed(new IdentityError { Description = "Invalid token or email." });
            }

            try
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password successfully reset for user {Email}", dto.Email);
                }
                else
                {
                    _logger.LogWarning("Password reset failed for user {Email}. Errors: {Errors}", dto.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                return result;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid token format during password reset for {Email}", dto.Email);
                return IdentityResult.Failed(new IdentityError { Description = "Invalid token format." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password reset for {Email}", dto.Email);
                return IdentityResult.Failed(new IdentityError { Description = "An unexpected error occurred." });
            }
        }
    }
}
