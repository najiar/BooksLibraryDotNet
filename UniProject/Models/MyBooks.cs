using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniProject.Models
{

    public class MyBooks
    {
        [Key]
        public int savedBookId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        public int BookId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Image { get; set; }


        [Column(TypeName = "nvarchar(100)")]

        public string Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]

        public string Author { get; set; }

        [Column(TypeName = "nvarchar(100)")]

        public string isbn { get; set; }

        [Column(TypeName = "nvarchar(100)")]

        public string Category { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string PublicationYear { get; set; }

    }
}
