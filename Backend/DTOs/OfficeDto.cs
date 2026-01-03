namespace HealthcareApp.DTOs
{
    public class OfficeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int PatientCount { get; set; }
        public int CaregiverCount { get; set; }
    }
}
