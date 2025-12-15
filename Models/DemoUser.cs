namespace OwaspTop10Demo.Models
{
    public class DemoUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }
}