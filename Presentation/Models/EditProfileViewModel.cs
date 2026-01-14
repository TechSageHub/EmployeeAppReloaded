using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class EditProfileViewModel
    {
        public Guid? EmployeeId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = default!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? Photo { get; set; }
    }
}
