using System;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.DTOs;
using HealthcareApp.Models;
using HealthcareApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace HealthcareApp.Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly ICaregiverRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public CaregiverService(ICaregiverRepository repository, ApplicationDbContext context, IMemoryCache cache)
        {
            _repository = repository;
            _context = context;
            _cache = cache;
        }

        public async Task<PagedResult<CaregiverResponseDto>> SearchCaregiversAsync(CaregiverSearchDto searchDto, string userId, string ipAddress)
        {
            var cacheKey = $"caregiver_search_{searchDto.Keyword}_{searchDto.OfficeId}_{searchDto.Page}_{searchDto.PageSize}_{searchDto.SortBy}_{searchDto.SortDescending}";

            if (!_cache.TryGetValue(cacheKey, out PagedResult<CaregiverResponseDto> result))
            {
                result = await _repository.SearchCaregiversAsync(searchDto);

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                _cache.Set(cacheKey, result, cacheOptions);
            }

            await LogSearchAuditAsync(userId, "Caregiver", searchDto.Keyword, result.TotalCount, ipAddress);

            return result;
        }

        private async Task LogSearchAuditAsync(string userId, string entityType, string searchTerm, int resultCount, string ipAddress)
        {
            var audit = new SearchAudit
            {
                UserId = userId,
                EntityType = entityType,
                SearchTerm = searchTerm,
                ResultCount = resultCount,
                SearchedAt = DateTime.UtcNow,
                IpAddress = ipAddress
            };

            _context.SearchAudits.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}
