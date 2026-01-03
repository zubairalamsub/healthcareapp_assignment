using System.Threading.Tasks;
using HealthcareApp.DTOs;

namespace HealthcareApp.Services
{
    public interface IOfficeService
    {
        Task<PagedResult<OfficeDto>> SearchOfficesAsync(OfficeSearchDto searchDto, string userId, string ipAddress);
        Task<OfficeDto> GetOfficeByIdAsync(int id);
    }
}
