// Controllers/PatientsController.cs
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using HealthcareGraphAPI.Models.Examples;
using HealthcareGraphAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Annotations;


namespace HealthcareGraphAPI.Controllers
{
    /// <summary>
    /// API controller for managing Patient entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Retrieves all active patients.
        /// </summary>
        /// <returns>A list of active patients.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Retrieves a patient by ID.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>The patient with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(string id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="patient">The patient object to create.</param>
        /// <returns>The created patient object and location header.</returns>

        [HttpPost]
        [SwaggerRequestExample(typeof(CreatePatientDto), typeof(PatientExample))]

        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createDto)
        {
            // Map DTO to domain model
            var patient = new Patient
            {
                Id = "P-" + Guid.NewGuid().ToString(),  // Auto-generate the ID
                CreatedAt = DateTime.UtcNow,
                Active = true,
                UpdatedAt = DateTime.UtcNow,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                DateOfBirth = DateTime.Parse(createDto.DateOfBirth),
                Gender = createDto.Gender,
                HealthCardNumber = createDto.HealthCardNumber
            };

            await _patientService.CreatePatientAsync(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Updates an existing patient with only provided fields.
        /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <param name="updateDto">The DTO with update data (optional fields).</param>
        /// <returns>No content if successful.</returns>


        [HttpPut("{id}")]
        [SwaggerRequestExample(typeof(CreatePatientDto), typeof(PatientExample))]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] UpdatePatientDto updateDto)
        {
            // Retrieve the existing patient from the service
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();
            // Update the modification timestamp
            patient.UpdatedAt = DateTime.UtcNow;


            // Update only provided fields
            if (updateDto.FirstName != null)
                patient.FirstName = updateDto.FirstName;
            if (updateDto.LastName != null)
                patient.LastName = updateDto.LastName;
            if (updateDto.DateOfBirth != null)
                patient.DateOfBirth = DateTime.Parse(updateDto.DateOfBirth);
            if (updateDto.Gender.HasValue)
                patient.Gender = updateDto.Gender.Value;
            if (updateDto.HealthCardNumber != null)
                patient.HealthCardNumber = updateDto.HealthCardNumber;



            // Save the changes via the service layer
            await _patientService.UpdatePatientAsync(patient);
            return NoContent();
        }

        /// <summary>
        /// Deletes a patient by ID by flaffing it as Inactive. Choice of no deletion for tracking purposes.
         /// </summary>
        /// <param name="id">The patient ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }



        /// <summary>
        /// Retrieves the network graph of relationships for a specific patient, limited to a depth of two.
        /// </summary>
        /// <remarks>
        /// The depth is limited to two for the following reasons:
        /// - Performance: Deeper queries are more resource-intensive and can slow down response times.
        /// - Relevancy: Most directly relevant relationships (e.g., immediate physicians or linked facilities) are found within one degree. In the current model, only followUpAction (Medicine, LabTest, Radiology) are found at a two degree from the patient.
        /// - Readability: A shallower graph is easier to visualize and interpret.
        /// - Security: Limiting depth minimizes exposure of sensitive or extraneous data.
        /// </remarks>

        [HttpGet("{patientId}/network")]
        public async Task<ActionResult<object>> GetPatientNetworkGraph(string patientId)
        {
            var network = await _patientService.GetPatientNetworkGraphAsync(patientId);
            return Ok(network);
        }
    
    /// <summary>
    /// Assigns a patient to a doctor by creating a relationship between them.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <param name="doctorId">The unique identifier of the doctor.</param>
    /// <returns>An HTTP 200 response when the assignment is successful.</returns>
    [HttpPost("patients/{patientId}/doctors/{doctorId}")]
    [SwaggerOperation(Summary = "Assign patient to doctor", 
                      Description = "Creates a relationship linking a patient and a doctor.")]
    [SwaggerResponse(200, "Patient successfully assigned to doctor.")]
    [SwaggerResponse(400, "Invalid patientId or doctorId provided.")]
    public async Task<IActionResult> AssignPatientToDoctor(string patientId, string doctorId)
    {
        await _patientService.AssignPatienttToDoctorAsync<Patient,Doctor>(patientId, doctorId);
        return Ok();
    }


    }
}
