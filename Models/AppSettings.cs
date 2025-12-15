namespace OwaspTop10Demo.Models
{
    public class AppSettings
    {
        public int SessionTimeoutMinutes { get; set; }
        public bool EnableDangerousFeatureX { get; set; }
        public string? AdminEmail { get; set; }
    }
}