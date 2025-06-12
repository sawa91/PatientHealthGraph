using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.Examples;
using HealthcareGraphAPI.Models.DTOs;

using HealthcareGraphAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace HealthcareGraphAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public DoctorsController(IDoctorService doctorService, IPatientService patientService)
        {
            _doctorService = doctorService;
            _patientService = patientService;
        }

        // ---------------------------------------
        // 1. CRUD Operations for Doctors
        // ---------------------------------------
        /// <summary>
        /// Retrieves all active Doctors.
        /// </summary>
        /// <returns>A list of active Doctors.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorssAsync();
            return Ok(doctors);
        }


        /// <summary>
        /// Retrieves a specific doctor by ID.
        /// </summary>
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctorById(string doctorId)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new { Message = $"Doctor with ID {doctorId} not found." });

            return Ok(doctor);
        }

        /// <summary>
        /// Creates a new doctor using CreateDoctorDto.
        /// </summary>
        /// <returns>The created doctor.</returns>
        [HttpPost]
        [SwaggerResponse(201, "Doctor successfully created", typeof(CreateDoctorDto))]
        [SwaggerRequestExample(typeof(CreateDoctorDto), typeof(DoctorExample))]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto doctorDto)
        {
            if (doctorDto == null)
                return BadRequest(new { Message = "Invalid doctor data." });

            // Convert DTO to Doctor model
            var doctor = new Doctor
            {
                Id = Guid.NewGuid().ToString(), // Generate a unique ID
                FirstName = doctorDto.FirstName,
                LastName = doctorDto.LastName,
                Specialization = doctorDto.Specialization,
                StartYear = doctorDto.StartYear,
                LicenseNumber = doctorDto.LicenseNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true

            };

            await _doctorService.CreateDoctorAsync(doctor);
            return CreatedAtAction(nameof(GetDoctorById), new { doctorId = doctor.Id }, doctor);
        }

        /// <summary>
        /// Updates an existing doctor using UpdateDoctorDto.
        /// </summary>
        [HttpPut("{doctorId}")]
        [SwaggerRequestExample(typeof(UpdateDoctorDto), typeof(DoctorExample))]

        public async Task<IActionResult> UpdateDoctor(string doctorId, [FromBody] UpdateDoctorDto updateDto)
        {
            // Retrieve the existing doctor from the service
            var doctor = await _doctorService.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound();
            // Update the modification timestamp
            doctor.UpdatedAt = DateTime.UtcNow;

            // Update only provided fields
            if (updateDto.FirstName != null)
                doctor.FirstName = updateDto.FirstName;
            if (updateDto.LastName != null)
                doctor.LastName = updateDto.LastName;
            if (updateDto.StartYear != null)
                doctor.StartYear = updateDto.StartYear;
            if (updateDto.Gender.HasValue)
                doctor.Gender = updateDto.Gender.Value;
            if (updateDto.LicenseNumber != null)
                doctor.LicenseNumber = updateDto.LicenseNumber;
            if (updateDto.Specialization != null)
                doctor.Specialization = updateDto.Specialization;



            // Save the changes via the service layer
            await _doctorService.UpdateDoctorAsync(doctor);
            return NoContent();
        }

        /// <summary>
        /// Deletes a doctor by ID. Rather than permanently deleting a doctor by ID, this method flags the record as inactive for auditing and version tracking purposes.
        /// </summary>
        /// <param name="id">The doctor ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            await _doctorService.DeleteDoctorAsync(id);
            return NoContent();
        }


        // ---------------------------------------
        // 2. Retrieve Doctors by Specific Queries
        // ---------------------------------------


        /// <summary>
        /// Retrieves all patients treated by a specific doctor.
        /// </summary>
        [HttpGet("{doctorId}/patients")]
        public async Task<IActionResult> GetPatientsByDoctor(string doctorId)
        {
            var patients = await _doctorService.GetPatientsByDoctorAsync(doctorId);
            return patients.Any() ? Ok(patients) : NotFound(new { Message = $"No patients found for doctor {doctorId}." });
        }

        /// <summary>
        /// Retrieves all treatments specialized by a doctor.
        /// </summary>
        [HttpGet("{doctorId}/treatments")]
        public async Task<IActionResult> GetTreatmentsByDoctor(string doctorId)
        {
            var treatments = await _doctorService.GetTreatmentsByDoctorAsync(doctorId);
            return treatments.Any() ? Ok(treatments) : NotFound(new { Message = $"No treatments found for doctor {doctorId}." });
        }

        // ---------------------------------------
        // 3. Manage Relationships (Doctor ↔ Facility, Doctor ↔ Treatments)
        // ---------------------------------------

        /// <summary>
        /// Assigns an abstract treatment to a doctor by creating the appropriate relationship.
        /// </summary>
        /// <param name="doctorId">The doctor ID.</param>
        /// <param name="treatmentId">The treatment ID.</param>
        /// <returns>An HTTP 200 response if successful.</returns>
         [HttpPost("{doctorId}/treatments/{treatmentId}")]
        [SwaggerOperation(Summary = "Assigns a treatment to a doctor", Description = "Creates a relationship linking a treatment to a doctor.")]
      
        [SwaggerResponse(200, "Treatment successfully assigned to doctor.")]
        [SwaggerResponse(400, "Invalid input. Either doctorId or treatmentId is missing or invalid.")]
            public async Task<IActionResult> AssignTreatmentToDoctor(string doctorId, string treatmentId)
        {
            await _doctorService.AssignTreatmentToDoctorAsync<Doctor, Treatment>(doctorId, treatmentId);            return Ok();
        }

}
    
}
