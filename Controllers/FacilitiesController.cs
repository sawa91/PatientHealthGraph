using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using HealthcareGraphAPI.Models.Examples;
using HealthcareGraphAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilitiesController : ControllerBase
    {
        private readonly IFacilityService _facilityService;

        public FacilitiesController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        // ---------------------------------------
        // 1. CRUD Operations for Facilities
        // ---------------------------------------

        /// <summary>
        /// Retrieves all active facilities.
        /// </summary>
        /// <returns>A list of active facilities.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all active facilities", Description = "Returns a list of all active facilities.")]
        [SwaggerResponse(200, "Facilities retrieved successfully", typeof(IEnumerable<Facility>))]
        public async Task<ActionResult<IEnumerable<Facility>>> GetAllFacilities()
        {
            var facilities = await _facilityService.GetAllFacilitiesAsync(); // corrected method name.
            return Ok(facilities);
        }

        /// <summary>
        /// Retrieves a specific facility by ID.
        /// </summary>
        /// <param name="facilityId">The ID of the facility.</param>
        /// <returns>The facility with the specified ID.</returns>
        [HttpGet("{facilityId}")]
        [SwaggerOperation(Summary = "Retrieves a facility by ID", Description = "Returns the facility details for the specified facility ID.")]
        [SwaggerResponse(200, "Facility retrieved successfully", typeof(Facility))]
        [SwaggerResponse(404, "Facility not found")]
        public async Task<IActionResult> GetFacilityById(string facilityId)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(facilityId);
            if (facility == null)
                return NotFound(new { Message = $"Facility with ID {facilityId} not found." });

            return Ok(facility);
        }

        /// <summary>
        /// Creates a new facility.
        /// </summary>
        /// <param name="facilityDto">The facility creation data.</param>
        /// <returns>The created facility record.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new facility", Description = "Creates a facility using the provided CreateFacilityDto.")]
        [SwaggerResponse(201, "Facility successfully created", typeof(Facility))]
        [SwaggerResponse(400, "Invalid facility data")]
        [SwaggerRequestExample(typeof(CreateFacilityDto), typeof(FacilityExample))]
        public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityDto facilityDto)
        {
            if (facilityDto == null)
                return BadRequest(new { Message = "Invalid facility data." });

            // Facility Type: Ensure null or empty values don't cause an error.
            FacilityType facilityType = FacilityType.Unknown;
            if (!string.IsNullOrWhiteSpace(facilityDto.Type))
            {
                if (!Enum.TryParse<FacilityType>(facilityDto.Type, ignoreCase: true, out facilityType))
                {
                    var validTypes = string.Join(", ", Enum.GetNames(typeof(FacilityType)));
                    return BadRequest(new { Message = $"Invalid facility type '{facilityDto.Type}'. Valid types: {validTypes}." });
                }
            }

            // Services Offered: Convert if provided, else set empty list.
            List<ServiceType> servicesOffered = new List<ServiceType>();
            if (facilityDto.ServicesOffered != null)
            {
                foreach (var service in facilityDto.ServicesOffered)
                {
                    if (!string.IsNullOrWhiteSpace(service) && Enum.TryParse<ServiceType>(service, ignoreCase: true, out var parsedService))
                    {
                        servicesOffered.Add(parsedService);
                    }
                    else
                    {
                        var validServices = string.Join(", ", Enum.GetNames(typeof(ServiceType)));
                        return BadRequest(new { Message = $"Invalid service '{service}'. Valid services: {validServices}." });
                    }
                }
            }

            // Capacity: Use default value if not provided.
            int capacity = facilityDto.Capacity ?? 0;

            // Contacts: Convert if provided, else set empty list.
            List<ContactInfo> contacts = facilityDto.Contacts != null
                ? facilityDto.Contacts.Select(c => new ContactInfo { Type = c.Type, Value = c.Value }).ToList()
                : new List<ContactInfo>();

            // Convert DTO to Facility model.
            var facility = new Facility
            {
                Id = Guid.NewGuid().ToString(),
                Name = facilityDto.Name,
                Type = facilityType,
                Capacity = capacity,
                ServicesOffered = servicesOffered,
                Contacts = contacts,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Active = true
            };

            await _facilityService.CreateFacilityAsync(facility);
            return CreatedAtAction(nameof(GetFacilityById), new { facilityId = facility.Id }, facility);
        }
        
        /// <summary>
        /// Updates an existing facility.
        /// </summary>
        /// <param name="facilityId">The ID of the facility to update.</param>
        /// <param name="updateDto">The facility update data.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{facilityId}")]
        [SwaggerOperation(Summary = "Updates an existing facility", Description = "Updates facility information using UpdateFacilityDto.")]
        [SwaggerResponse(204, "Facility updated successfully")]
        [SwaggerResponse(400, "Invalid data provided")]
        [SwaggerResponse(404, "Facility not found")]
        [SwaggerRequestExample(typeof(UpdateFacilityDto), typeof(FacilityExample))]
        public async Task<IActionResult> UpdateFacility(string facilityId, [FromBody] UpdateFacilityDto updateDto)
        {
            // Retrieve the existing facility from the service.
            var facility = await _facilityService.GetFacilityByIdAsync(facilityId);
            if (facility == null)
                return NotFound(new { Message = $"Facility with ID {facilityId} not found." });

            // Update the modification timestamp.
            facility.UpdatedAt = DateTime.UtcNow;

            // Update only provided fields.
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                facility.Name = updateDto.Name;

            // Validate and update the facility type.
            if (!string.IsNullOrEmpty(updateDto.Type))
            {
                if (!Enum.TryParse<FacilityType>(updateDto.Type, ignoreCase: true, out var parsedType))
                {
                    var validTypes = string.Join(", ", Enum.GetNames(typeof(FacilityType)));
                    return BadRequest(new { Message = $"Invalid facility type '{updateDto.Type}'. Valid types: {validTypes}." });
                }
                facility.Type = parsedType;
            }

            // Update capacity if provided.
            if (updateDto.Capacity.HasValue)
                facility.Capacity = updateDto.Capacity.Value;

            // Validate and update ServicesOffered.
            if (updateDto.ServicesOffered != null && updateDto.ServicesOffered.Any())
            {
                var services = new List<ServiceType>();
                foreach (var service in updateDto.ServicesOffered)
                {
                    if (!Enum.TryParse<ServiceType>(service, ignoreCase: true, out var parsedService))
                    {
                        var validServices = string.Join(", ", Enum.GetNames(typeof(ServiceType)));
                        return BadRequest(new { Message = $"Invalid service '{service}'. Valid services: {validServices}." });
                    }
                    services.Add(parsedService);
                }
                facility.ServicesOffered = services;
            }

            // Update Contacts if provided.
            if (updateDto.Contacts != null && updateDto.Contacts.Any())
            {
                facility.Contacts = updateDto.Contacts
                    .Select(c => new ContactInfo { Type = c.Type, Value = c.Value })
                    .ToList();
            }

            // Save the changes via the service layer.
            await _facilityService.UpdateFacilityAsync(facility);
            return NoContent();
        }

        /// <summary>
        /// Deletes a facility by flagging it as inactive.
        /// </summary>
        /// <param name="id">The facility ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a facility", Description = "Marks a facility as inactive instead of permanently deleting it.")]
        [SwaggerResponse(204, "Facility marked as inactive successfully")]
        public async Task<IActionResult> DeleteFacility(string id)
        {
            await _facilityService.DeleteFacilityAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all doctors working at a given facility.
        /// </summary>
        /// <param name="facilityId">The facility ID.</param>
        /// <returns>A list of doctors at the facility.</returns>
        [HttpGet("{facilityId}/doctors")]
        [SwaggerOperation(Summary = "Retrieves all doctors for a facility", Description = "Returns all doctors working at the specified facility.")]
        [SwaggerResponse(200, "Doctors retrieved successfully", typeof(IEnumerable<Doctor>))]
        [SwaggerResponse(404, "No doctors found at the facility")]
        public async Task<IActionResult> GetDoctorsByFacility(string facilityId)
        {
            var doctors = await _facilityService.GetDoctorsByFacilityAsync(facilityId);
            return doctors.Any() ? Ok(doctors) : NotFound(new { Message = $"No doctors found at facility {facilityId}." });
        }

        /// <summary>
        /// Retrieves all treatments available within a facility.
        /// </summary>
        /// <param name="facilityId">The facility ID.</param>
        /// <returns>A list of treatments available at the facility.</returns>
        [HttpGet("{facilityId}/treatments")]
        [SwaggerOperation(Summary = "Retrieves all treatments for a facility", Description = "Returns all treatments available within the specified facility.")]
        [SwaggerResponse(200, "Treatments retrieved successfully", typeof(IEnumerable<Treatment>))]
        [SwaggerResponse(404, "No treatments found for the facility")]
        public async Task<IActionResult> GetTreatmentsByFacility(string facilityId)
        {
            var treatments = await _facilityService.GetTreatmentsAvailableAtFacilityAsync(facilityId);
            return treatments.Any() ? Ok(treatments) : NotFound(new { Message = $"No treatments found for facility {facilityId}." });
        }

        // ---------------------------------------
        // 3. Manage Relationships (Doctor ↔ Facility, Treatment ↔ Facility)
        // ---------------------------------------

        /// <summary>
        /// Assigns an abstract treatment to a facility by creating the appropriate relationship.
        /// </summary>
        /// <param name="facilityId">The facility ID.</param>
        /// <param name="treatmentId">The treatment ID.</param>
        /// <returns>An HTTP 200 response if successful.</returns>
        [HttpPost("{facilityId}/treatments/{treatmentId}")]
        [SwaggerOperation(Summary = "Assigns a treatment to a facility", Description = "Creates a relationship linking a treatment to a facility.")]
        [SwaggerResponse(200, "Treatment successfully assigned to facility")]
        public async Task<IActionResult> AssignTreatmentToFacility(string facilityId, string treatmentId)
        {
            await _facilityService.AssignTreatmentToFacilityAsync(treatmentId, facilityId);
            return Ok();
        }

        /// <summary>
        /// Assigns a doctor to a facility by creating the appropriate relationship.
        /// </summary>
        /// <param name="facilityId">The facility ID.</param>
        /// <param name="doctorId">The doctor ID.</param>
        /// <returns>An HTTP 200 response if successful.</returns>
        [HttpPost("{facilityId}/doctors/{doctorId}")]
        [SwaggerOperation(Summary = "Assigns a doctor to a facility", Description = "Creates a relationship linking a doctor to a facility.")]
        [SwaggerResponse(200, "Doctor successfully assigned to facility")]
        public async Task<IActionResult> AssignDoctorToFacility(string facilityId, string doctorId)
        {
            await _facilityService.AssignDoctorToFacilityAsync(doctorId, facilityId);
            return Ok();
        }
    }
}
