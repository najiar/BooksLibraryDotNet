using Microsoft.EntityFrameworkCore;

namespace UniProject.Models
{
    public class BooksDB:DbContext
    {
        public BooksDB(DbContextOptions<BooksDB> options) : base(options) 
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Register> Accounts { get; set; }

        public DbSet<MyBooks> MyBooks { get; set; }
    }
}
