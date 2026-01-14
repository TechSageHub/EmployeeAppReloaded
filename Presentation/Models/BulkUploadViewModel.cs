using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class BulkUploadViewModel
    {
        [Required(ErrorMessage = "Please select a CSV file to upload.")]
        [Display(Name = "Select CSV File")]
        public IFormFile? File { get; set; }
    }
}
