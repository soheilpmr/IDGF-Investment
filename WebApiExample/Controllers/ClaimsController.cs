using IDGF.AuthDB.Data.Entities;
using IDGFAuth.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDGFAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IDGFAuthDbContextSQL _db;

        public ClaimsController(IDGFAuthDbContextSQL db)
        {
            _db = db;
        }

        [HttpGet("GetAllClaims")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> GetAvailableClaims()
        {
            var claims = await _db.ClaimDefinitions
                .Select(c => new { c.Id, c.Type, c.Value, c.Description })
                .ToListAsync();

            return Ok(claims);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<IActionResult> AddClaimDefinition([FromBody] CreateClaimDefinitionDto dto)
        {
            var exists = await _db.ClaimDefinitions
                .AnyAsync(c => c.Value == dto.Value);

            if (exists)
                return Conflict($"Claim '{dto.Value}' already exists.");

            var claimDef = new ClaimDefinition
            {
                Type = "Permission",
                Value = dto.Value,
                Description = dto.Description
            };

            _db.ClaimDefinitions.Add(claimDef);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Claim definition added", claimDef.Id, claimDef.Value });
        }
    }
}
public class CreateClaimDefinitionDto
{
    public string Value { get; set; } = string.Empty;      
    public string Description { get; set; } = string.Empty; 
}
