using System.ComponentModel.DataAnnotations;

namespace StatosphericBackend.Entities
{
    public class AuthModel
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }
}