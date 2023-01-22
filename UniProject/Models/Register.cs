using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniProject.Models
{
    public class Register
    {
        [Key]
        public int AccountId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("First name: ")]
        [Required(ErrorMessage = "First name should not be empty!")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Last name: ")]
        [Required(ErrorMessage = "Last name should not be empty!")]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Gender: ")]
        [Required(ErrorMessage = "Gender should not be empty!")]
        public string Gender { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Age: ")]
        [Required(ErrorMessage = "Age  should not be empty!")]
        public string Age { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Email: ")]
        [Required(ErrorMessage = "Email should not be empty!")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Password: ")]
        [Required(ErrorMessage = "Password should not be empty!")]
        public string Password { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("IsAdmin: ")]
        [Required]
        public string IsAdmin { get; set; }
        public bool Empty
        {
            get
            {
                return (
                          string.IsNullOrWhiteSpace(FirstName) &&
                          string.IsNullOrWhiteSpace(LastName) &&
                          string.IsNullOrWhiteSpace(Email) &&
                          string.IsNullOrWhiteSpace(Password) &&
                          string.IsNullOrWhiteSpace(Age) &&
                          string.IsNullOrWhiteSpace(Gender));
                    
            }
        }
        

    }
}
