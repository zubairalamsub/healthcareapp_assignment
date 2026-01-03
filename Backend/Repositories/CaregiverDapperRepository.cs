using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HealthcareApp.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HealthcareApp.Repositories
{
    public class CaregiverDapperRepository : ICaregiverRepository
    {
        private readonly string _connectionString;

        public CaregiverDapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<PagedResult<CaregiverResponseDto>> SearchCaregiversAsync(CaregiverSearchDto searchDto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Phone,
                    c.Email,
                    c.Specialization,
                    o.Name as OfficeName
                FROM Caregivers c
                INNER JOIN Offices o ON c.OfficeId = o.Id
                WHERE (@Keyword IS NULL OR
                       c.FirstName LIKE @KeywordPattern OR
                       c.LastName LIKE @KeywordPattern OR
                       c.Phone LIKE @KeywordPattern)
                ORDER BY
                    CASE WHEN @SortBy = 'FirstName' AND @SortDescending = 0 THEN c.FirstName END ASC,
                    CASE WHEN @SortBy = 'FirstName' AND @SortDescending = 1 THEN c.FirstName END DESC,
                    CASE WHEN @SortBy = 'LastName' AND @SortDescending = 0 THEN c.LastName END ASC,
                    CASE WHEN @SortBy = 'LastName' AND @SortDescending = 1 THEN c.LastName END DESC,
                    CASE WHEN @SortBy = 'Phone' AND @SortDescending = 0 THEN c.Phone END ASC,
                    CASE WHEN @SortBy = 'Phone' AND @SortDescending = 1 THEN c.Phone END DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var countQuery = @"
                SELECT COUNT(*)
                FROM Caregivers c
                WHERE (@Keyword IS NULL OR
                       c.FirstName LIKE @KeywordPattern OR
                       c.LastName LIKE @KeywordPattern OR
                       c.Phone LIKE @KeywordPattern)";

            var keyword = string.IsNullOrWhiteSpace(searchDto.Keyword) ? null : searchDto.Keyword.Trim();
            var keywordPattern = keyword != null ? $"%{keyword}%" : null;

            var parameters = new
            {
                Keyword = keyword,
                KeywordPattern = keywordPattern,
                SortBy = searchDto.SortBy ?? "FirstName",
                SortDescending = searchDto.SortDescending ? 1 : 0,
                Offset = (searchDto.Page - 1) * searchDto.PageSize,
                PageSize = searchDto.PageSize
            };

            var caregivers = await connection.QueryAsync<CaregiverResponseDto>(query, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new { Keyword = keyword, KeywordPattern = keywordPattern });

            return new PagedResult<CaregiverResponseDto>
            {
                Data = caregivers,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }
    }
}
