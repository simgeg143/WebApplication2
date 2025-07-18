using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebApplication2.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet() { }
            public async Task<IActionResult> OnPostAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "Username cannot be empty.");
                return Page();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }
    }
}
