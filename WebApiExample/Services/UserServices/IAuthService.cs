using IDGFAuth.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace IDGFAuth.Services.UserServices
{
    public interface IAuthService
    {
        Task RequestPasswordResetAsync(ForgotPasswordDto dto);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
