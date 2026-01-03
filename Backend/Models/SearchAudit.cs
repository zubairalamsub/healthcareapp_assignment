using System;

namespace HealthcareApp.Models
{
    public class SearchAudit
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EntityType { get; set; }
        public string SearchTerm { get; set; }
        public int ResultCount { get; set; }
        public DateTime SearchedAt { get; set; }
        public string IpAddress { get; set; }
    }
}
