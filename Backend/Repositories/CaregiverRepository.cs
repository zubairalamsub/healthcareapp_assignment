using System;
using System.Linq;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HealthcareApp.Repositories
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly ApplicationDbContext _context;

        public CaregiverRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CaregiverResponseDto>> SearchCaregiversAsync(CaregiverSearchDto searchDto)
        {
            var query = _context.Caregivers
                .Include(c => c.Office)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim().ToLower();
                var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (keywords.Length > 1)
                {
                    // Multiple words: match all words against FirstName or LastName (for full name search)
                    query = query.Where(c =>
                        keywords.All(k =>
                            c.FirstName.ToLower().Contains(k) ||
                            c.LastName.ToLower().Contains(k)) ||
                        c.Phone.Contains(keyword) ||
                        c.Email.ToLower().Contains(keyword));
                }
                else
                {
                    // Single word: original behavior
                    query = query.Where(c =>
                        c.FirstName.ToLower().Contains(keyword) ||
                        c.LastName.ToLower().Contains(keyword) ||
                        c.Phone.Contains(keyword) ||
                        c.Email.ToLower().Contains(keyword));
                }
            }

            if (searchDto.OfficeId.HasValue)
            {
                query = query.Where(c => c.OfficeId == searchDto.OfficeId.Value);
            }

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, searchDto.SortBy, searchDto.SortDescending);

            var caregivers = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(c => new CaregiverResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Specialization = c.Specialization,
                    OfficeName = c.Office != null ? c.Office.Name : "N/A"
                })
                .ToListAsync();

            return new PagedResult<CaregiverResponseDto>
            {
                Data = caregivers,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }

        private IQueryable<Models.Caregiver> ApplySorting(IQueryable<Models.Caregiver> query, string sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "lastname" => descending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                "phone" => descending ? query.OrderByDescending(c => c.Phone) : query.OrderBy(c => c.Phone),
                "email" => descending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                _ => descending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName)
            };
        }
    }
}
