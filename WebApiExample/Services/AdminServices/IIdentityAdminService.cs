using Microsoft.AspNetCore.Identity;

namespace IDGF.Auth.Services.AdminServices
{
    public interface IIdentityAdminService<TRole, TUser>
        where TRole : IdentityRole
        where TUser : IdentityUser
    {
        Task<IdentityResult> ChangePasswordOfUserAsync(string userID, string newPassword);

        Task<IdentityResult> ChangePasswordForMyselfAsync(string userID,
                   string oldPassword,
                   string newPassword,
                   string repeatPassword);

    }
}
