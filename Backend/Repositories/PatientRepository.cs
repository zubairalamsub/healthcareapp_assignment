using System;
using System.Linq;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.DTOs;
using HealthcareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareApp.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PatientDto>> GetPatientsAsync(int page, int pageSize, string sortBy = "FirstName", bool sortDescending = false)
        {
            var query = _context.Patients
                .Include(p => p.PatientCaregivers)
                    .ThenInclude(pc => pc.Caregiver)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDescending);

            var patients = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    OfficeId = p.OfficeId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Caregivers = p.PatientCaregivers.Select(pc => new CaregiverResponseDto
                    {
                        Id = pc.Caregiver.Id,
                        FirstName = pc.Caregiver.FirstName,
                        LastName = pc.Caregiver.LastName,
                        Phone = pc.Caregiver.Phone,
                        Email = pc.Caregiver.Email,
                        Specialization = pc.Caregiver.Specialization
                    }).ToList()
                })
                .ToListAsync();

            return new PagedResult<PatientDto>
            {
                Data = patients,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.PatientCaregivers)
                    .ThenInclude(pc => pc.Caregiver)
                .Where(p => p.Id == id)
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    OfficeId = p.OfficeId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Caregivers = p.PatientCaregivers.Select(pc => new CaregiverResponseDto
                    {
                        Id = pc.Caregiver.Id,
                        FirstName = pc.Caregiver.FirstName,
                        LastName = pc.Caregiver.LastName,
                        Phone = pc.Caregiver.Phone,
                        Email = pc.Caregiver.Email,
                        Specialization = pc.Caregiver.Specialization
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto patientDto)
        {
            var patient = new Patient
            {
                OfficeId = patientDto.OfficeId,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                Phone = patientDto.Phone,
                Email = patientDto.Email,
                Address = patientDto.Address,
                CreatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            if (patientDto.CaregiverIds?.Any() == true)
            {
                foreach (var caregiverId in patientDto.CaregiverIds)
                {
                    _context.PatientCaregivers.Add(new PatientCaregiver
                    {
                        PatientId = patient.Id,
                        CaregiverId = caregiverId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
            }

            return await GetPatientByIdAsync(patient.Id);
        }

        public async Task<PatientDto> UpdatePatientAsync(int id, CreatePatientDto patientDto)
        {
            var patient = await _context.Patients
                .Include(p => p.PatientCaregivers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return null;

            patient.OfficeId = patientDto.OfficeId;
            patient.FirstName = patientDto.FirstName;
            patient.LastName = patientDto.LastName;
            patient.DateOfBirth = patientDto.DateOfBirth;
            patient.Phone = patientDto.Phone;
            patient.Email = patientDto.Email;
            patient.Address = patientDto.Address;
            patient.UpdatedAt = DateTime.UtcNow;

            _context.PatientCaregivers.RemoveRange(patient.PatientCaregivers);

            if (patientDto.CaregiverIds?.Any() == true)
            {
                foreach (var caregiverId in patientDto.CaregiverIds)
                {
                    _context.PatientCaregivers.Add(new PatientCaregiver
                    {
                        PatientId = patient.Id,
                        CaregiverId = caregiverId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return await GetPatientByIdAsync(id);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<Patient> ApplySorting(IQueryable<Patient> query, string sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "lastname" => descending ? query.OrderByDescending(p => p.LastName) : query.OrderBy(p => p.LastName),
                "dateofbirth" => descending ? query.OrderByDescending(p => p.DateOfBirth) : query.OrderBy(p => p.DateOfBirth),
                "phone" => descending ? query.OrderByDescending(p => p.Phone) : query.OrderBy(p => p.Phone),
                "email" => descending ? query.OrderByDescending(p => p.Email) : query.OrderBy(p => p.Email),
                _ => descending ? query.OrderByDescending(p => p.FirstName) : query.OrderBy(p => p.FirstName)
            };
        }
    }
}
