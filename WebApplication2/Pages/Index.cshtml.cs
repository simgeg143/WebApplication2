using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;


namespace WebApplication2.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppDbContext appDbContext;

        public List<UserDto> Users { get; set; } = new();

        public IndexModel(IHttpClientFactory clientFactory, AppDbContext appDbContext)
        {
            _clientFactory = clientFactory;
            this.appDbContext = appDbContext;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var currentUsername = User.Identity?.Name;
            Users = await appDbContext.Users
                .Where(u => u.Username != currentUsername)
                .Select (u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Birthday = u.Birthday
                })
                .ToListAsync();

            return Page();

        }
        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime Birthday { get; set; }
        }

    }
}
