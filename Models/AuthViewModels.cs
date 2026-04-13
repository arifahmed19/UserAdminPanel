using System.ComponentModel.DataAnnotations;

namespace UserAdminPanel.Models
{
    // This defines the fields required for the Registration form
    public class RegisterViewModel
    {
        [Required] 
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress] 
        public string Email { get; set; } = string.Empty;
        
        // No minimum length specified -> fulfills the 1-character password rule
        [Required] 
        public string Password { get; set; } = string.Empty; 
    }

    // This defines the fields required for the Login form
    public class LoginViewModel
    {
        [Required, EmailAddress] 
        public string Email { get; set; } = string.Empty;
        
        [Required] 
        public string Password { get; set; } = string.Empty;
    }
}
