using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
namespace ArreglaMiCiudad.Models
{
    [Table("User")]
    public class User
    {
        public int UserId { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;
        public Client? Client { get; set; }
        public Administrator? Administrator { get; set; }
    }
}

