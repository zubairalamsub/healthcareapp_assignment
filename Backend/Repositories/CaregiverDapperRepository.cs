using System.Data;
using System.Linq;
using System.Text;
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

            var keyword = string.IsNullOrWhiteSpace(searchDto.Keyword) ? null : searchDto.Keyword.Trim();
            var parameters = new DynamicParameters();
            parameters.Add("SortBy", searchDto.SortBy ?? "FirstName");
            parameters.Add("SortDescending", searchDto.SortDescending ? 1 : 0);
            parameters.Add("Offset", (searchDto.Page - 1) * searchDto.PageSize);
            parameters.Add("PageSize", searchDto.PageSize);

            // Build WHERE clause for keyword search
            var whereClause = new StringBuilder();
            if (keyword != null)
            {
                var keywords = keyword.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                if (keywords.Length > 1)
                {
                    // Multi-word search: all words must match against FirstName or LastName
                    var conditions = new List<string>();
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        parameters.Add($"Keyword{i}", $"%{keywords[i]}%");
                        conditions.Add($"(c.FirstName LIKE @Keyword{i} OR c.LastName LIKE @Keyword{i})");
                    }
                    // Also allow exact phrase match on phone
                    parameters.Add("KeywordPattern", $"%{keyword}%");
                    whereClause.Append($"(({string.Join(" AND ", conditions)}) OR c.Phone LIKE @KeywordPattern)");
                }
                else
                {
                    // Single word search
                    parameters.Add("KeywordPattern", $"%{keyword}%");
                    whereClause.Append("(c.FirstName LIKE @KeywordPattern OR c.LastName LIKE @KeywordPattern OR c.Phone LIKE @KeywordPattern)");
                }
            }
            else
            {
                whereClause.Append("1=1");
            }

            var query = $@"
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
                WHERE {whereClause}
                ORDER BY
                    CASE WHEN @SortBy = 'FirstName' AND @SortDescending = 0 THEN c.FirstName END ASC,
                    CASE WHEN @SortBy = 'FirstName' AND @SortDescending = 1 THEN c.FirstName END DESC,
                    CASE WHEN @SortBy = 'LastName' AND @SortDescending = 0 THEN c.LastName END ASC,
                    CASE WHEN @SortBy = 'LastName' AND @SortDescending = 1 THEN c.LastName END DESC,
                    CASE WHEN @SortBy = 'Phone' AND @SortDescending = 0 THEN c.Phone END ASC,
                    CASE WHEN @SortBy = 'Phone' AND @SortDescending = 1 THEN c.Phone END DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY";

            var countQuery = $@"
                SELECT COUNT(*)
                FROM Caregivers c
                WHERE {whereClause}";

            var caregivers = await connection.QueryAsync<CaregiverResponseDto>(query, parameters);
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

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
