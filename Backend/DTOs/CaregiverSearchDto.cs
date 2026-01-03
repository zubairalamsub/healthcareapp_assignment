namespace HealthcareApp.DTOs
{
    public class CaregiverSearchDto
    {
        public string? Keyword { get; set; } = string.Empty;
        public int? OfficeId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "FirstName";
        public bool SortDescending { get; set; } = false;
    }
}
