using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniProject.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image URL:")]
        public string Image { get; set; }


        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Title:")]
        [Required]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Author: ")]
        [Required]
        public string Author { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("ISBN:")]
        [Required]
        public string isbn { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Category { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Publication year:")]
        public string PublicationYear { get; set; }


    }
}
