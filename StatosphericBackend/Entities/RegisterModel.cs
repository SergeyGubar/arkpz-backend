using System.ComponentModel.DataAnnotations;

namespace StatosphericBackend.Entities
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        public string Role { get; set; }


    }
}