using System;
using System.Collections.Generic;

namespace HealthcareApp.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<CaregiverResponseDto> Caregivers { get; set; }
    }
}
