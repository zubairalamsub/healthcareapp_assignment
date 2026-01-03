using System;
using System.Collections.Generic;

namespace HealthcareApp.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Patient> Patients { get; set; }
        public ICollection<Caregiver> Caregivers { get; set; }
    }
}
