using System.Threading.Tasks;
using HealthcareApp.DTOs;

namespace HealthcareApp.Services
{
    public interface IPatientService
    {
        Task<PagedResult<PatientDto>> GetPatientsAsync(int page, int pageSize, string sortBy = "FirstName", bool sortDescending = false);
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<PatientDto> CreatePatientAsync(CreatePatientDto patientDto);
        Task<PatientDto> UpdatePatientAsync(int id, CreatePatientDto patientDto);
        Task<bool> DeletePatientAsync(int id);
    }
}
