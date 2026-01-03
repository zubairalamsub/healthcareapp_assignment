using System;
using System.Collections.Generic;

namespace HealthcareApp.Models
{
    public class Caregiver
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Office Office { get; set; }
        public ICollection<PatientCaregiver> PatientCaregivers { get; set; }
    }
}
