using Diplom2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Npgsql;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
namespace Diplom2.Controllers
{
    public class AuthController : Controller
    {
        #region
        private bool IsLoginExists(string login)
        {
            //добавить подключение
            using (var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db"))
            {
                con.Open();
                using (var command = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM users WHERE login = @login)", con))
                {
                    command.Parameters.AddWithValue("@login", login);
                    return (bool)command.ExecuteScalar();
                }
            }
        }
        private byte[] HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return hashBytes;
            }
        }
        private bool IsPasswordCorrect(byte[] password)
        {
            using (var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db"))
            {
                con.Open();
                using (var command = new NpgsqlCommand("SELECT EXISTS(SELECT 1 FROM users WHERE password = @password)", con))
                {
                    command.Parameters.AddWithValue("@password", password);
                    return (bool)command.ExecuteScalar();
                }
            }
        }
        private string GetUserRole(NpgsqlConnection con, string login)
        {
            using (var conn = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db"))
            {
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT role FROM users WHERE login = @login", con);
                cmd.Parameters.AddWithValue("login", login);
                var role = cmd.ExecuteScalar() as string;
                return role;
            }
        }

        #endregion
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Registration()
        {
            return View("~/Views/Auth/Registration.cshtml");
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registration(RegistrationModel data)
        {
            if (!ModelState.IsValid)
                return Content("Заполните данные!");
            else
            {
                if (IsLoginExists(data.Login))
                    return View("Views/Auth/LoginAlreadyClaimed.cshtml");
                else
                {
                    using (var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db"))
                    {
                        con.Open();
                        using (var command = new NpgsqlCommand("INSERT INTO users(login, password) VALUES(@Login, @Password)", con))
                        {
                            byte[] hashedPassword = HashPassword(data.Password);
                            command.Parameters.AddWithValue("login", data.Login);
                            command.Parameters.AddWithValue("password", hashedPassword);
                            command.ExecuteNonQuery();
                        }

                    }
                }

                return View("/Views/Auth/RegistrationSuccess.cshtml");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Views/Auth/Login.cshtml");
        }
        [AllowAnonymous]
        [HttpPost]
        async public Task<IActionResult> Login(LoginModel data)
        {
            using (var con = new NpgsqlConnection("Server=localhost;Port=5432;Username=postgres;Password=18082004;Database=users_db"))
            {
                if (!ModelState.IsValid)
                {
                    return Content("Заполните Данные!");
                }
                else
                {
                    if (IsLoginExists(data.Login))
                    {
                        if (IsPasswordCorrect(HashPassword(data.Password)))
                        {
                            con.Open();
                            var role = GetUserRole(con,data.Login); 
                            var claims = new List<Claim>
                            {
                                 new Claim(ClaimTypes.Name, data.Login),
                                 new Claim(ClaimTypes.Role, role)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true, 
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                            };
                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);
                            return role == "Admin" ? RedirectToAction("ScanAdmin", "AINetwork") : RedirectToAction("Scan", "AINetwork");
                        }
                        else
                        {
                            return View("Views/Auth/IncorrectPasswordError.cshtml");
                        }
                    }
                    else
                    {
                        return View("Views/Auth/IncorrectData.cshtml");
                    }
                }
            }
        }
    }
}

