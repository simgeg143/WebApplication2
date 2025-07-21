using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Data.AppDbContext _context;
        public UserController(Data.AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("others")]
        public async Task<IActionResult> GetUsers()
        {
            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
            {
                return Unauthorized("User is not authenticated.");
            }
            var users = await _context.Users
                .Where(u => u.Username != currentUsername)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Birthday
                })
                .ToListAsync();
            return Ok(users);
        }       
    }
}
