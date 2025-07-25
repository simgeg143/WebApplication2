using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;


namespace WebApplication2.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public MessagesController(AppDbContext context)
        {
            appDbContext = context;
        }
        [HttpGet("{user1}/{user2}")]
        public async Task<IActionResult> GetMessages(string user1, string user2)
        {
            var messages = await appDbContext.Messages
                .Where(m =>
                (m.SenderUsername == user1 && m.ReceiverUsername == user2) ||
                (m.SenderUsername == user2 && m.ReceiverUsername == user1))
                .OrderBy(m => m.SentAt)
                 .Select(m => new {
                     m.Id,
                     m.SenderUsername,
                     m.ReceiverUsername,
                     m.Content,
                     m.SentAt
                 })
                .ToListAsync();
            return Ok(messages);
        }
    }
}
