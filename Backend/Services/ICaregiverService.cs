using System.Threading.Tasks;
using HealthcareApp.DTOs;

namespace HealthcareApp.Services
{
    public interface ICaregiverService
    {
        Task<PagedResult<CaregiverResponseDto>> SearchCaregiversAsync(CaregiverSearchDto searchDto, string userId, string ipAddress);
    }
}
