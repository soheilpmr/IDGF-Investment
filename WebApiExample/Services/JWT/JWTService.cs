using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IDGFAuth.Controllers;
using IDGFAuth.Data.Entities;
using IdentityModel;

namespace IDGFAuth.Services.JWT
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public JWTService(UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            // 1. Get roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var roleName in userRoles)
            {
                // Add role as claim
                roleClaims.Add(new Claim(JwtClaimTypes.Role, roleName));

                // Add role permissions
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claimsForRole = await _roleManager.GetClaimsAsync(role);

                    // Only include Permission claims
                    roleClaims.AddRange(claimsForRole.Where(c => c.Type == "Permission"));
                }
            }

            // 2. Standard user claims
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            // 3. Combine with role claims
            claims.AddRange(roleClaims);

            // 4. Signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTBearerSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Create token with issuer and audience matching middleware
            var token = new JwtSecurityToken(
                issuer: _config["JWTBearerSettings:Issuer"],
                audience: _config["JWTBearerSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ApplicationUser?> GetUserByID(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
