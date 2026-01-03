using System.Threading.Tasks;
using HealthcareApp.DTOs;

namespace HealthcareApp.Repositories
{
    public interface IOfficeRepository
    {
        Task<PagedResult<OfficeDto>> SearchOfficesAsync(OfficeSearchDto searchDto);
        Task<OfficeDto> GetOfficeByIdAsync(int id);
    }
}
