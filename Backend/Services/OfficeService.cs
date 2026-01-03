using System;
using System.Threading.Tasks;
using HealthcareApp.Data;
using HealthcareApp.DTOs;
using HealthcareApp.Models;
using HealthcareApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace HealthcareApp.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public OfficeService(IOfficeRepository repository, ApplicationDbContext context, IMemoryCache cache)
        {
            _repository = repository;
            _context = context;
            _cache = cache;
        }

        public async Task<PagedResult<OfficeDto>> SearchOfficesAsync(OfficeSearchDto searchDto, string userId, string ipAddress)
        {
            var cacheKey = $"office_search_{searchDto.Keyword}_{searchDto.Page}_{searchDto.PageSize}_{searchDto.SortBy}_{searchDto.SortDescending}";

            if (!_cache.TryGetValue(cacheKey, out PagedResult<OfficeDto> result))
            {
                result = await _repository.SearchOfficesAsync(searchDto);

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                _cache.Set(cacheKey, result, cacheOptions);
            }

            await LogSearchAuditAsync(userId, "Office", searchDto.Keyword, result.TotalCount, ipAddress);

            return result;
        }

        public async Task<OfficeDto> GetOfficeByIdAsync(int id)
        {
            var cacheKey = $"office_{id}";

            if (!_cache.TryGetValue(cacheKey, out OfficeDto result))
            {
                result = await _repository.GetOfficeByIdAsync(id);

                if (result != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    };

                    _cache.Set(cacheKey, result, cacheOptions);
                }
            }

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
