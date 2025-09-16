using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IDGFAuth.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("{roleName}/claims")]
        public async Task<IActionResult> AddClaimsToRole(string roleName, [FromBody] List<ClaimDto> claims)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound($"Role '{roleName}' not found.");

            foreach (var claim in claims)
            {
                var exists = (await _roleManager.GetClaimsAsync(role))
                             .Any(c => c.Type == claim.Type && c.Value == claim.Value);

                if (!exists)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
                    if (!result.Succeeded)
                        return BadRequest(result.Errors);
                }
            }

            return Ok($"Claims added to role '{roleName}'.");
        }


        [HttpPost]
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

public record ClaimDto(string Type, string Value);

public record CreateRoleDto(string Name);
