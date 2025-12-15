using System.ComponentModel.DataAnnotations;

namespace OwaspTop10Demo.Models
{
    public class UpdateSettingsDto
    {
        [Range(1, 60)]
        public int SessionTimeoutMinutes { get; set; }

        public bool EnableDangerousFeatureX { get; set; }

        [EmailAddress]
        public string? AdminEmail { get; set; }
    }
}