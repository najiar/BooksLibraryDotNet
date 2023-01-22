using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UniProject.Models;

namespace UniProject.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {

        //For the DB connection
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataReader dataReader;

        private readonly BooksDB _context;

        //DB connection string
        void connectionString()
        {
            connection.ConnectionString = "server=127.0.0.1;port=3306;username=root;password=;database=booksdb;Allow User Variables=True";
        }

        public BooksController(BooksDB context)
        {
            _context = context;
        }

        //return all Books
        // GET: Books/Index
        public async Task<IActionResult> Index()
        {
              return View(await _context.Books.ToListAsync());
        }

        // GET: Books/AddOrEdit
        public IActionResult AddOrEdit(int id=0 )
        {
            if (id == 0) //if no id // create new book
            {
                return View(new Book());
            }
            else
            {
                return View(_context.Books.Find(id)); //if id then edit
            }
        }
        public static bool CheckURLValid (string ImageUrl) //check if the provided image url valid or not
        {
            Uri? uriResult;
            return Uri.TryCreate(ImageUrl, UriKind.Absolute, out uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        // POST: Books/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("BookId,Image,Title,Author,isbn,Category,PublicationYear")] Book book)
        {
            if (ModelState.IsValid)
            {
                if (!CheckURLValid(book.Image)) //if the url is not valid
                {
                    book.Image = "https://sciendo.com/product-not-found.png"; //add default image
                }
                if (book.BookId == 0) //if its create id is 0
                {
                    
                    _context.Add(book);
                }
                else
                {

                    _context.Update(book); // if its update
                    
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); //get the users back to books page
            }
            return View(book);
        }

        //Delete book by id
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        //This funtion to save a book to mybooks of the current user
        public async Task<IActionResult> SaveBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id); //find the book we want to save

                connectionString();
                connection.Open(); //open connection to DB
                command.Connection = connection;
                //check if book already saved to the current users books
                command.CommandText = "SELECT * FROM mybooks WHERE Email = '" + GlobalVars.currentUserEmail + "' AND BookId =  '" + id + "'"; 
                dataReader = command.ExecuteReader();
                if (dataReader.Read()) // if its already saved
                {
                    ViewBag.bookAlreadySaved = "Book already saved to your books!";
                    connection.Close();
                    dataReader.Close();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    dataReader.Close(); // Add the book to the mybooks table with bookid and current user email
                    command.CommandText = "INSERT INTO mybooks(Email, BookId, Image, Title, Author, isbn, Category, PublicationYear)" +
                    "VALUES('" + @GlobalVars.currentUserEmail + "', '" +@book.BookId  + "', '" +@book.Image  + "', '" +@book.Title  + "', '" +@book.Author  + "', '" +@book.isbn  + "','" +@book.Category  + "', '" +@book.PublicationYear  + "')"; 
                    dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        ViewBag.bookAlreadySaved = "The book is saved to your books!"; //show the user
                    }
                    //close connection and reader
                    connection.Close();
                    dataReader.Close();
                    return RedirectToAction(nameof(Index)); //return the user to all books

                }   
            }
            catch 
            { 
                 connection.Close();
                 dataReader.Close();
                 return RedirectToAction(nameof(Index));
            }
           
        }

        //Delete book by id 
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BooksDB.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);

            }
            
            await _context.SaveChangesAsync();
            try
            {
                connectionString();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "DELETE FROM mybooks WHERE BookId =  '" + id + "'"; //delete the books for all users also in mybooks
                dataReader = command.ExecuteReader();
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
           
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return _context.Books.Any(e => e.BookId == id);
        }
    }
}
