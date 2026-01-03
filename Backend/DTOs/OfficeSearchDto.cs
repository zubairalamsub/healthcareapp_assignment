namespace HealthcareApp.DTOs
{
    public class OfficeSearchDto
    {
        public string? Keyword { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }
}
