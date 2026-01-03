using System.Threading.Tasks;
using HealthcareApp.DTOs;

namespace HealthcareApp.Repositories
{
    public interface ICaregiverRepository
    {
        Task<PagedResult<CaregiverResponseDto>> SearchCaregiversAsync(CaregiverSearchDto searchDto);
    }
}
