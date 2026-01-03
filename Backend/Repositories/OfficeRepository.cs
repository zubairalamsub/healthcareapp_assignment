using System.Linq;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HealthcareApp.Repositories
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<OfficeDto>> SearchOfficesAsync(OfficeSearchDto searchDto)
        {
            var query = _context.Offices
                .Include(o => o.Patients)
                .Include(o => o.Caregivers)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim();
                query = query.Where(o =>
                    EF.Functions.Like(o.Name, $"%{keyword}%") ||
                    EF.Functions.Like(o.Address, $"%{keyword}%") ||
                    EF.Functions.Like(o.Phone, $"%{keyword}%"));
            }

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, searchDto.SortBy, searchDto.SortDescending);

            var offices = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(o => new OfficeDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Address = o.Address,
                    Phone = o.Phone,
                    PatientCount = o.Patients.Count,
                    CaregiverCount = o.Caregivers.Count
                })
                .ToListAsync();

            return new PagedResult<OfficeDto>
            {
                Data = offices,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<OfficeDto> GetOfficeByIdAsync(int id)
        {
            return await _context.Offices
                .Include(o => o.Patients)
                .Include(o => o.Caregivers)
                .Where(o => o.Id == id)
                .Select(o => new OfficeDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Address = o.Address,
                    Phone = o.Phone,
                    PatientCount = o.Patients.Count,
                    CaregiverCount = o.Caregivers.Count
                })
                .FirstOrDefaultAsync();
        }

        private IQueryable<Models.Office> ApplySorting(IQueryable<Models.Office> query, string sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "address" => descending ? query.OrderByDescending(o => o.Address) : query.OrderBy(o => o.Address),
                "phone" => descending ? query.OrderByDescending(o => o.Phone) : query.OrderBy(o => o.Phone),
                _ => descending ? query.OrderByDescending(o => o.Name) : query.OrderBy(o => o.Name)
            };
        }
    }
}
