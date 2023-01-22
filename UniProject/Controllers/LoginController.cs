using Microsoft.AspNetCore.Mvc;
using UniProject.Models;
using MySqlConnector;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace UniProject.Controllers
{
    public class LoginController : Controller
    {
        //connection to DB
        
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataReader dataReader;


        //Login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //Logout action
        public async Task<IActionResult> Logout()
        {
            GlobalVars.currentUser = ""; //set the current user to be empty on logout
            GlobalVars.isLoggedIn = false; //set the user to be not loggedin anymore
            GlobalVars.isCurrentUserAdmin= false; //set is admin to be false for the current user

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //remove the Authentication Cookie on logout

            return View("~/Views/Login/login.cshtml");
        }
        void connectionString()
        {
            connection.ConnectionString = "server=127.0.0.1;port=3306;username=root;password=;database=booksdb;";
        }
        
        //Function to decode passwords from Base/64 
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        //In this function we check the user auth information with the database
        public async Task<IActionResult> Verify(Login acc)
        {
            try
            {
                connectionString();
                connection.Open(); //open connection
                command.Connection = connection;
                command.CommandText = "SELECT Password, Email, IsAdmin, Firstname FROM accounts WHERE Email='" + acc.Email + "'"; //get most of the data for the user
                dataReader = command.ExecuteReader(); //execut command

                if (dataReader.Read()) //if we have such user 
                {
                    if (DecodeFrom64(dataReader["Password"].ToString()) == acc.Password) //check password after decode if it matchs the input of the user
                    {
                        GlobalVars.currentUser = dataReader["Firstname"].ToString();

                        if (dataReader["IsAdmin"].ToString() == "True") //if the user is admin set him to be admin to see the admin panel
                        {
                            GlobalVars.isCurrentUserAdmin = true; //set him to be the current user
                        }

                        GlobalVars.isLoggedIn = true; //set him to be logged it
                        GlobalVars.currentUserEmail = dataReader["Email"].ToString();

                         
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, dataReader["Email"].ToString())
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal));//Add auth cookie

                        //close connection and reader
                        connection.Close();
                        dataReader.Close();
                        return RedirectToAction("Index", "Books");
                    }
                    else //if password does not match
                    {
                        connection.Close();
                        dataReader.Close();
                        ViewBag.loginValidation = "Wrong password!"; //show the user
                        return View("~/Views/Login/login.cshtml");
                    }
                }
                else //if wrong email which means no such email in DB
                {
                    connection.Close();
                    dataReader.Close();
                    ViewBag.loginValidation = "There is no such user in our database!"; //show the user
                    return View("~/Views/Login/login.cshtml");
                }

            }
            catch (Exception ex)
            {
                ViewBag.loginValidation = "Somthing went wrong!"; //this case mostly happens if the DB is down
                return View("~/Views/Login/login.cshtml");
            }
           
        }
        public IActionResult RedirectToRegister() //take the user to register page
        {

            return RedirectToAction("Register", "Accounts");

        }

    }
}
