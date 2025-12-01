using System.ComponentModel.DataAnnotations;

namespace ArreglaMiCiudad.Models
{
    public class RegisterViewModel
    {
        // User
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        // Client
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public char Gender { get; set; }
        public IFormFile? ProfileImage { get; set; }

        public IFormFile? IdCardImage { get; set; }
    }
}
