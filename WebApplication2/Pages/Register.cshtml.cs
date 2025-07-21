using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApplication2.Data;
using WebApplication2.Helpers;
using WebApplication2.Models;

namespace WebApplication2.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext appDbContext;

        public RegisterModel(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public DateTime Birthday { get; set; }

        public string Message { get; set; }
        public void OnGet()
        { }
            public async Task<IActionResult> OnPostAsync(string username, string password, string email, DateTime birthday)
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
                {
                    ModelState.AddModelError("", "Username, password and email cannot be empty.");
                    return Page();
                }
            if (appDbContext.Users.Any(u => u.Username == Username))
            { 
            Message = "Username already exists.";
            return Page();
            }
            var hashedPassword = PasswordHasher.HashPassword(Password);
            var user = new User
                {
                    Username = Username,
                    Password = hashedPassword,
                    Email = Email,
                    Birthday = Birthday
                };
            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToPage("/Login");
        }
    }
}
