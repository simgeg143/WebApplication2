using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebApplication2.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static Dictionary<string, List<string>> UserConnections = new();
        public override Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                if (!UserConnections.ContainsKey(username))
                { 
                   UserConnections[username] = new List<string>();
                }
                UserConnections[username].Add(Context.ConnectionId);
              
                Clients.All.SendAsync("UserUpdated", UserConnections.Keys);
            }
           
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(username) && UserConnections.ContainsKey(username))
            {
                UserConnections[username].Remove(Context.ConnectionId);
                if (!UserConnections[username].Any())
                {
                    UserConnections.Remove(username);
                }

                Clients.All.SendAsync("UserUpdated", UserConnections.Keys);
            }
            return Task.CompletedTask;
        }
        public async Task SendPrivateMessageToUser(string receiverUsername, string message)
        {
            var sender = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(receiverUsername))
                return;

            var sentAt = DateTime.Now.ToString("HH:mm");

            if (UserConnections.TryGetValue(receiverUsername, out var connections))
            {
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", sender, message, sentAt, receiverUsername);
                }
            }

            if (UserConnections.TryGetValue(sender, out var senderConnections))
            { 
                foreach (var connectionId in senderConnections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", sender, message, sentAt, receiverUsername);
                }

            }
        }
    }
}
