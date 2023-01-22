using Microsoft.Build.Framework;
using System.ComponentModel;

namespace UniProject.Models
{
    public class Login
    {
        [DisplayName("Email: ")]
        [Required]
        public string Email { get; set; }

        [DisplayName("Password: ")]
        [Required]
        public string Password { get; set; }
        
    }
}
