using BackEndInfrastructure.CustomAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IDGFAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private const string ClaimType = "Permission";

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost(nameof(AddClaimsToRole))]
        //[RequirePermission("CanDeleteInvoice", AuthenticationSchemes = "Bearer")]
        //[Authorize]
        [Authorize(AuthenticationSchemes ="Bearer", Roles = "Admin")]
        public async Task<IActionResult> AddClaimsToRole([FromBody] AddClaimsToRoleDto dto)
        {
            var role = await _roleManager.FindByNameAsync(dto.RoleName);
            if (role == null)
                return NotFound($"Role '{dto.RoleName}' not found.");

            foreach (var claim in dto.claims)
            {
                var exists = (await _roleManager.GetClaimsAsync(role))
                             .Any(c => c.Type == ClaimType && c.Value == claim.Value);

                if (!exists)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim(ClaimType, claim.Value));
                    if (!result.Succeeded)
                        return BadRequest(result.Errors);
                }
            }

            return Ok($"Claims added to role '{dto.RoleName}'.");
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var exists = await _roleManager.RoleExistsAsync(dto.Name);
            if (exists)
                return Conflict($"Role '{dto.Name}' already exists.");

            var result = await _roleManager.CreateAsync(new IdentityRole(dto.Name));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Role created successfully", Role = dto.Name });
        }
    }
}

public record ClaimDto(string Value);

public record CreateRoleDto(string Name);


public class AddClaimsToRoleDto
{
    public string RoleName { get; set; }
    public List<ClaimDto> claims { get; set; }
}