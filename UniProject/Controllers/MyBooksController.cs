using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniProject.Models;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;

namespace UniProject.Controllers
{
    [Authorize]
    public class MyBooksController : Controller
    {
        private readonly BooksDB _context;

        //For the DB connection
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataReader dataReader;

        //connection string for DB
        void connectionString()
        {
            connection.ConnectionString = "server=127.0.0.1;port=3306;username=root;password=;database=booksdb;Allow User Variables=True";
        }
        public MyBooksController(BooksDB context)
        {
            _context = context;
        }
        public async Task<IActionResult> Books() //return all saved books
        {
            return View(await _context.MyBooks.ToListAsync());
        }

        public async Task<IActionResult> removeBookFromSaved(int id) //removes a book from saved books only for current user by ID and Email
        {
            try
            {
                var book = await _context.Books.FindAsync(id);

                connectionString();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "DELETE FROM mybooks WHERE BookId =  '" + id + "' AND Email = '"+GlobalVars.currentUserEmail+"'";
                dataReader = command.ExecuteReader();
                
                connection.Close();
                dataReader.Close();
                return RedirectToAction(nameof(Books));
               
            }
            catch
            {
                return RedirectToAction(nameof(Books));
            }

        }
    }
}
