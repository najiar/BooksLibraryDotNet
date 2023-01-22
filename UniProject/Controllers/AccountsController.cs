using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniProject.Models;
using Microsoft.AspNetCore.Authorization;
using MySqlConnector;
using System.Text.RegularExpressions;


namespace UniProject.Controllers
{
    public class AccountsController : Controller
    {
        //For the connection to DB
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataReader dataReader;

        private readonly BooksDB _context;

        //Connection string auth to DB
        void connectionString()
        {
            connection.ConnectionString = "server=127.0.0.1;port=3306;username=root;password=;database=booksdb;";
        }
        public AccountsController(BooksDB context)
        {
            _context = context;
        }

        //Returns the list of accounts exist in the DB
        //Its Authorize which mean onlt access if you have Cookie
        // GET: Accounts/Index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //if current user isnt admin return notFound
            if(GlobalVars.isCurrentUserAdmin == true)
            {
                return View(await _context.Accounts.ToListAsync()); //if admin show the list of users **Admin Panel**
            }
            else
            {
                return NotFound();
            }
              
        }

        // GET: Accounts/Register
        public IActionResult Register()
        {
            return View();
        }
        //Function used to encode password to Base64
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //Regex check for the input fields
        public bool isInputCorrect(Register register, bool forEdit)
        {
            Regex firstNameLastNameRegex = new Regex(@"^(?=[a-zA-Z\s]{2,25}$)(?=[a-zA-Z\s])(?:([\w\s*?])\1?(?!\1))+$");
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$");

            Match firstNameMatch = firstNameLastNameRegex.Match(register.FirstName);
            Match lastNameMatch = firstNameLastNameRegex.Match(register.LastName);
                        

            if (!firstNameMatch.Success || !lastNameMatch.Success)
            {
                ViewBag.regexValidation = "The First name and last name should contain only alphabetical characters and the name length should be 2 or more!";
                return false;
            }

            Match emailMatch = emailRegex.Match(register.Email);
            if (!emailMatch.Success)
            {
                ViewBag.regexValidation = "This email is not a valid email address!"; //show the user
                return false;
            }

            if (forEdit == false) //since edit does not include edit password
            {
                Match passwordMatch = passwordRegex.Match(register.Password);
                if (!passwordMatch.Success)
                {
                    ViewBag.regexValidation = "The password should be minimum five characters, at least one letter and one number!";
                    return false;
                }
            }

            return true;
        }

        //When the register button is clicked this action will be preformed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("AccountId,FirstName,LastName,Gender,Age,Email,Password,IsAdmin")] Register register)
        {
            if(isInputCorrect(register, false) == false) //if the regex does not pass and its not for edin but for create 
            {
                return View(register);
            }

            if (!register.Empty) { //Double check if the fields are not empty
                connectionString();
                connection.Open(); //open connection to DB
                command.Connection = connection;
                command.CommandText = "SELECT Email FROM accounts WHERE Email = '" + register.Email + "'"; //check if email already exist
                dataReader = command.ExecuteReader();

                if (dataReader.Read()) 
                {
                    ViewBag.regexValidation = "This email is already used by another user!"; //show the user 
                    return View(register);
                }
                else //if email does not exist encode the password and add it to the database
                {
                    register.Password = EncodePasswordToBase64(register.Password);
                    register.IsAdmin = "False"; //not admin as default
                    _context.Add(register);
                    await _context.SaveChangesAsync();
                    ViewBag.regexValidation = "";
                    return RedirectToAction("Login", "Login", new { area = "" });//get the user to the login page
                }
            }
            return View(register);  
        }
         
        // GET: Accounts/Edit/
        public async Task<IActionResult> Edit(int? id) //for editing an account
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var register = await _context.Accounts.FindAsync(id); //find the account with id
            if (register == null)
            {
                return NotFound();
            }
            return View(register); //return it
        }

        // POST: Accounts/Edit/
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FirstName,LastName,Gender,Age,Email,IsAdmin")] Register register)
        {
            if (isInputCorrect(register, true) == false) //if regex does not pass and we are in edit
            {
                return View(register);
            }

            if (id != register.AccountId)
            {
                return NotFound();
            }
            try
            {
                connectionString();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Password FROM accounts WHERE Email = '" + register.Email + "'"; //since we lose password on edit because we dont show it to the user, get it again
                dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    register.Password = dataReader["Password"].ToString();

                }

                _context.Update(register);
                await _context.SaveChangesAsync();
                connection.Close();
                dataReader.Close();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisterExists(register.AccountId))
                {
                    return NotFound();

                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Accounts/Delete/
        // get user id for delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var register = await _context.Accounts
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (register == null)
            {
                return NotFound();
            }

            return View(register);
        }

        // POST: Accounts/Delete/id
        //delete the user
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'BooksDB.Accounts'  is null.");
            }
            var register = await _context.Accounts.FindAsync(id);
            try
            {
                connectionString();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Email FROM accounts WHERE AccountId = '" + id + "'";
                dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    if (dataReader["Email"].ToString() == GlobalVars.currentUserEmail)
                    {
                        connection.Close();
                        dataReader.Close();
                        return RedirectToAction(nameof(Index));
                    }
                    connection.Close();
                    dataReader.Close();
                }
            }
            catch
            {

                return RedirectToAction(nameof(Index));
            }
            if (register != null)
            {
                _context.Accounts.Remove(register);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterExists(int id)
        {
          return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
