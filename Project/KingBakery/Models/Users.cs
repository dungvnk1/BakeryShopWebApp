using System.ComponentModel.DataAnnotations;

namespace KingBakery.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Role { get; set; }
    }
}
