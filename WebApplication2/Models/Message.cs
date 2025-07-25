namespace WebApplication2.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
