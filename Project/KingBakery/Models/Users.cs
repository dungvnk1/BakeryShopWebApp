using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingBakery.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string? Address { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Role { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [NotMapped] // Indicates that this property should not be mapped to any database column
        public string ConfirmPassword { get; set; }
    }
}
