using System.ComponentModel.DataAnnotations;

namespace KingBakery.ViewModel
{
    public class ContactViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Message { get; set; }
    }
}
