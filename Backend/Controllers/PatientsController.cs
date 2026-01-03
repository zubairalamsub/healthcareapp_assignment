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
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PatientDto>>> GetPatients(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "FirstName",
            [FromQuery] bool sortDescending = false)
        {
            var result = await _service.GetPatientsAsync(page, pageSize, sortBy, sortDescending);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var result = await _service.GetPatientByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto patientDto)
        {
            var result = await _service.CreatePatientAsync(patientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "ManagerOrAdmin")]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] CreatePatientDto patientDto)
        {
            var result = await _service.UpdatePatientAsync(id, patientDto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var result = await _service.DeletePatientAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
