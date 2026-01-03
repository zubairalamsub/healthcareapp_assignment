using System;
using System.Threading.Tasks;
using HealthcareApp.DTOs;
using HealthcareApp.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace HealthcareApp.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repository;
        private readonly IMemoryCache _cache;

        public PatientService(IPatientRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResult<PatientDto>> GetPatientsAsync(int page, int pageSize, string sortBy = "FirstName", bool sortDescending = false)
        {
            var cacheKey = $"patients_{page}_{pageSize}_{sortBy}_{sortDescending}";

            if (!_cache.TryGetValue(cacheKey, out PagedResult<PatientDto> result))
            {
                result = await _repository.GetPatientsAsync(page, pageSize, sortBy, sortDescending);

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };

                _cache.Set(cacheKey, result, cacheOptions);
            }

            return result;
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var cacheKey = $"patient_{id}";

            if (!_cache.TryGetValue(cacheKey, out PatientDto result))
            {
                result = await _repository.GetPatientByIdAsync(id);

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

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto patientDto)
        {
            var result = await _repository.CreatePatientAsync(patientDto);
            InvalidatePatientsCache();
            InvalidateCaregiversCache(); // Clear caregiver cache as assignments changed
            return result;
        }

        public async Task<PatientDto> UpdatePatientAsync(int id, CreatePatientDto patientDto)
        {
            var result = await _repository.UpdatePatientAsync(id, patientDto);
            InvalidatePatientCache(id);
            InvalidatePatientsCache();
            InvalidateCaregiversCache(); // Clear caregiver cache as assignments changed
            return result;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var result = await _repository.DeletePatientAsync(id);
            if (result)
            {
                InvalidatePatientCache(id);
                InvalidatePatientsCache();
                InvalidateCaregiversCache(); // Clear caregiver cache as assignments changed
            }
            return result;
        }

        private void InvalidatePatientCache(int id)
        {
            _cache.Remove($"patient_{id}");
        }

        private void InvalidatePatientsCache()
        {
            // Clear patient list cache for all common page sizes and sort combinations
            var pageSizes = new[] { 10, 20, 50, 100 };
            // Note: Frontend sends camelCase field names (firstName, lastName, etc.)
            var sortFields = new[] { "firstName", "lastName", "dateOfBirth", "phone", "email", "FirstName", "LastName", "DateOfBirth", "Phone", "Email" };
            var sortOrders = new[] { true, false };

            for (int page = 1; page <= 10; page++)
            {
                foreach (var pageSize in pageSizes)
                {
                    foreach (var sortBy in sortFields)
                    {
                        foreach (var sortDesc in sortOrders)
                        {
                            _cache.Remove($"patients_{page}_{pageSize}_{sortBy}_{sortDesc}");
                        }
                    }
                }
            }
        }

        private void InvalidateCaregiversCache()
        {
            // Clear caregiver search cache for all common combinations
            // Cache key format: caregiver_search_{Keyword}_{OfficeId}_{Page}_{PageSize}_{SortBy}_{SortDescending}
            var pageSizes = new[] { 10, 20, 50, 100 };
            // Frontend uses PascalCase for caregivers but include both cases to be safe
            var sortFields = new[] { "FirstName", "LastName", "Phone", "Email", "firstName", "lastName", "phone", "email" };
            var sortOrders = new[] { true, false };
            var keywords = new[] { "", null };

            for (int page = 1; page <= 10; page++)
            {
                foreach (var pageSize in pageSizes)
                {
                    foreach (var sortBy in sortFields)
                    {
                        foreach (var sortDesc in sortOrders)
                        {
                            foreach (var keyword in keywords)
                            {
                                // Clear with no office filter
                                _cache.Remove($"caregiver_search_{keyword}__{page}_{pageSize}_{sortBy}_{sortDesc}");

                                // Clear for different office IDs (assuming up to 20 offices)
                                for (int officeId = 1; officeId <= 20; officeId++)
                                {
                                    _cache.Remove($"caregiver_search_{keyword}_{officeId}_{page}_{pageSize}_{sortBy}_{sortDesc}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
