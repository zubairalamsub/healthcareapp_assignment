using System;
using System.Collections.Generic;

namespace HealthcareApp.DTOs
{
    public class CreatePatientDto
    {
        public int OfficeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<int>? CaregiverIds { get; set; }
    }
}
