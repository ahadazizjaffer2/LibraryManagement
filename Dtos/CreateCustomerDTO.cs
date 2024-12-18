using System.ComponentModel.DataAnnotations;

namespace librarymanagement.Dtos
{
    public class CreateCustomerDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must be between 4 and 100 characters.", MinimumLength = 4)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password must be between 8 and 100 characters.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
