using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Data;

namespace WebApplication2.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext appDbContext;
        private readonly IHttpClientFactory _clientFactory;
        public List<UserDto> Users { get; set; } = new List<UserDto>();
        public List<ChatMessageDto> ChatHistory { get; set; } = new List<ChatMessageDto>();


        public IndexModel(AppDbContext appDbContext, IHttpClientFactory clientFactory)
        {
            this.appDbContext = appDbContext;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(string? chatWith = null)
        {
            var currentUser = User.Identity.Name;

            Users = await appDbContext.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Birthday = u.Birthday
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(chatWith))
            {
                ChatHistory = await appDbContext.Messages
                    .Where(m =>
                    (m.SenderUsername == currentUser && m.ReceiverUsername == chatWith) ||
                    (m.SenderUsername == chatWith && m.ReceiverUsername == currentUser))
                    .OrderBy(m => m.SentAt)
                    .Select(m => new ChatMessageDto
                    {
                        SenderUsername = m.SenderUsername,
                        ReceiverUsername = m.ReceiverUsername,
                        Content = m.Content,
                        SentAt = m.SentAt,
                    })
                    .ToListAsync();
            }

            return Page();
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public DateTime Birthday { get; set; }
        }
        public class ChatMessageDto
        {
            public  int Id { get; set; }
            public string SenderUsername { get; set; }
            public string ReceiverUsername { get; set; }
            public string Content { get; set; }
            public DateTime SentAt { get; set; }
        }
    }
}
