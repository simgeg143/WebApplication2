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
    public class LoginModel : PageModel
    {
        private readonly AppDbContext appDbContext;

        public LoginModel(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "Username or password cannot be empty.");
                return Page();
            }

            var user = appDbContext.Users.FirstOrDefault(u => u.Username == Username);
            if (user != null && PasswordHasher.VerifyPassword(Password, user.Password))
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                return RedirectToPage("/Index");
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return Page();
        }
    }
}
