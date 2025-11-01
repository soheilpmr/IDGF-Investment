using System.ComponentModel.DataAnnotations;

namespace IDGFAuth.Models.Dtos
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
