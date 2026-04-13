using System.ComponentModel.DataAnnotations;

namespace UserAdminPanel.Models
{
    public enum UserStatus 
    { 
        Unverified = 0, 
        Active = 1, 
        Blocked = 2 
    }

    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty; 
        
        public DateTime? LastLoginTime { get; set; }

        public List<DateTime>? LoginTimestamps { get; set; } = new List<DateTime>();
        
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        
        public UserStatus Status { get; set; } = UserStatus.Unverified;
    }
}
