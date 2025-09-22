using BackEndInfrastructure.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace IDGF.Auth.Services.AdminServices
{
    public class IdentityAdminService<TRole, TUser> : IIdentityAdminService<TRole, TUser>
            where TUser : IdentityUser
            where TRole : IdentityRole
    {
        private readonly IPasswordValidator<TUser> _passwordValidator;
        private readonly UserManager<TUser> _userManager;
        public IdentityAdminService(UserManager<TUser> userManager,
            IPasswordValidator<TUser> passwordValidator)
        {
            _passwordValidator = passwordValidator;
            _userManager = userManager;
        }
        public async Task<IdentityResult> ChangePasswordOfUserAsync(string userID, string newPassword)
        {
            var userForChangePassword = await _userManager.FindByIdAsync(userID);
            if (userForChangePassword == null)
            {
                var noObj = new ServiceObjectNotFoundException(nameof(userForChangePassword) + " Not Found For ChangePassword");
                throw noObj;
            }

            var checkPasswordValidate = await _passwordValidator.ValidateAsync(_userManager, userForChangePassword, newPassword);
            string error = "";
            if (!checkPasswordValidate.Succeeded)
            {
                foreach (var item in checkPasswordValidate.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        error += "- ";
                    }
                    error += item.Description;
                }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                var x = new PasswordValidateException(error);
                throw x;
            }

            userForChangePassword.PasswordHash = _userManager.PasswordHasher.HashPassword(userForChangePassword, newPassword);

            var result = await _userManager.UpdateAsync(userForChangePassword);

            if (!result.Succeeded)
            {
                throw new UserException("Error In Changing Password Of Current User");
            }

            return result;
        }

        public async Task<IdentityResult> ChangePasswordForMyselfAsync(string userID,
            string oldPassword, string newPassword, string repeatPassword)
        {
            if (newPassword != repeatPassword)
            {
                throw new PasswordValidateException("newpassword and repaetofnewpassword is not the same");
            }
            var userForChangePassword = await _userManager.FindByIdAsync(userID);
            if (userForChangePassword == null)
            {
                var noObj = new ServiceObjectNotFoundException(nameof(userForChangePassword) + " Not Found For ChangePassword");
                throw noObj;
            }

            var currentHashedPassword = userForChangePassword.PasswordHash;

            var resultOldPassword = _userManager.PasswordHasher
                .VerifyHashedPassword(userForChangePassword, currentHashedPassword, oldPassword);

            if (((byte)resultOldPassword) != 1)
            {
                throw new PasswordValidateException("Code2 CurrentPassword Is Not True");
            }

            var checkPasswordValidate = await _passwordValidator.ValidateAsync(_userManager, userForChangePassword, newPassword);
            string error = "";
            if (!checkPasswordValidate.Succeeded)
            {
                foreach (var item in checkPasswordValidate.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        error += "- ";
                    }
                    error += item.Description;
                }
            }

            userForChangePassword.PasswordHash = _userManager.PasswordHasher.HashPassword(userForChangePassword, newPassword);

            var result = await _userManager.UpdateAsync(userForChangePassword);

            if (!result.Succeeded)
            {
                throw new UserException("Error In Changing Password Of Current User");
            }

            return result;
        }
    }
}
