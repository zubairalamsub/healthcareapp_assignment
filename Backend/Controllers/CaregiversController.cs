using System.Linq;
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
    public class CaregiversController : ControllerBase
    {
        private readonly ICaregiverService _service;

        public CaregiversController(ICaregiverService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<CaregiverResponseDto>>> Search([FromQuery] CaregiverSearchDto searchDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _service.SearchCaregiversAsync(searchDto, userId, ipAddress);
            return Ok(result);
        }
    }
}
