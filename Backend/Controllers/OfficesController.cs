using System.Security.Claims;
using System.Threading.Tasks;
using HealthcareApp.DTOs;
using HealthcareApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OfficesController : ControllerBase
    {
        private readonly IOfficeService _service;

        public OfficesController(IOfficeService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<OfficeDto>>> Search([FromQuery] OfficeSearchDto searchDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _service.SearchOfficesAsync(searchDto, userId, ipAddress);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OfficeDto>> GetOffice(int id)
        {
            var result = await _service.GetOfficeByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
