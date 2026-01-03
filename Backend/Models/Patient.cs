using System;
using System.Collections.Generic;

namespace HealthcareApp.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Office Office { get; set; }
        public ICollection<PatientCaregiver> PatientCaregivers { get; set; }
    }
}
