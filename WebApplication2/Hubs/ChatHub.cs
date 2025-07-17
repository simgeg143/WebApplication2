using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage (string message)
        {
            var user = Context.User?.Identity?.Name ?? "Anonymous";
            var sentAt = DateTime.Now.ToString("HH:mm:ss");
            await Clients.All.SendAsync("ReceiveMessage", user, message, sentAt);
        }
    }
}
