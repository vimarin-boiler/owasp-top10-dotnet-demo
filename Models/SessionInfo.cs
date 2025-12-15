namespace OwaspTop10Demo.Models
{
    public class SessionInfo
    {
        public string Token { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}