using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using HealthcareGraphAPI.Repositories;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Services
{
    /// <summary>
    /// Implements facility operations by using the repository for CRUD and the Neo4j driver for custom queries.
    /// </summary>
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _facilityRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITreatmentRepository _treatmentRepository;

        private readonly IDriver _driver;

        public FacilityService(IFacilityRepository facilityRepository, IDoctorRepository doctorRepository,ITreatmentRepository treatmentRepository, IDriver driver)
        {
            _facilityRepository = facilityRepository;
            _doctorRepository = doctorRepository;
            _treatmentRepository = treatmentRepository;
            _driver = driver;
        }

        public Task<IEnumerable<Facility>> GetAllFacilitiesAsync() => _facilityRepository.GetAllAsync();

        /// <inheritdoc/>
        public Task<Facility> GetFacilityByIdAsync(string id) => _facilityRepository.GetFacilityByIdAsync(id);

        /// <inheritdoc/>
        public async Task CreateFacilityAsync(Facility facility)
        {
            using var session = _driver.AsyncSession();

            await session.ExecuteWriteAsync(async tx =>
            {
                // Create Facility Node
                await tx.RunAsync(@"
                CREATE (f:Facility {id: $id, name: $name, type: $type, capacity: $capacity, active: $active, createdAt: $createdAt, updatedAt: $updatedAt})
                ", new
                {
                    id = facility.Id,
                    name = facility.Name,
                    type = facility.Type.ToString(),
                    capacity = facility.Capacity,
                    active = facility.Active,
                    createdAt = facility.CreatedAt.ToUniversalTime(),
                    updatedAt = facility.UpdatedAt.ToUniversalTime()
                });

                // Create Contact Nodes and Relationships
                foreach (var contact in facility.Contacts)
                {
                    await tx.RunAsync(@"
                    CREATE (c:ContactInfo {id: $contactId, type: $type, value: $value})
                    WITH c
                    MATCH (f:Facility {id: $facilityId})
                    CREATE (f)-[:HAS_CONTACT]->(c)
                    ", new
                    {
                        contactId = Guid.NewGuid().ToString(),
                        type = contact.Type,
                        value = contact.Value,
                        facilityId = facility.Id
                    });
                }
            });
        }

        /// <inheritdoc/>
        public Task UpdateFacilityAsync(Facility facility) => _facilityRepository.UpdateAsync(facility);

        /// <inheritdoc/>
        public Task DeleteFacilityAsync(string id) => _facilityRepository.DeleteAsync(id);

        public async Task<IEnumerable<Doctor>> GetDoctorsByFacilityAsync(string facilityId)
        {
            return await _facilityRepository.GetAllSourcesByCriteriaAsync<Doctor>("WORKS_AT", "Facility", facilityId, NodeMapper.MapDoctor);
        }

        public Task<IEnumerable<AbstractTreatment>> GetTreatmentsAvailableAtFacilityAsync(string facilityId)
        {
            return _facilityRepository.GetAllSourcesByCriteriaAsync<AbstractTreatment>("AVAILABLE_AT", "Facility", facilityId, NodeMapper.MapAbstractTreatment);
        }

        /// <summary>
        /// Assigns a doctor to a facility after validating that both exist.
        /// </summary>
        /// <param name="doctorId">ID of the doctor</param>
        /// <param name="facilityId">ID of the facility</param>
        public async Task AssignDoctorToFacilityAsync(string doctorId, string facilityId)
        {
            // Inline test: Ensure input values are not null or whitespace.
            if (string.IsNullOrWhiteSpace(doctorId))
                throw new ArgumentException("Doctor id must not be null or empty.", nameof(doctorId));
            if (string.IsNullOrWhiteSpace(facilityId))
                throw new ArgumentException("Facility id must not be null or empty.", nameof(facilityId));

            // Validation Test: Check if the Doctor exists.
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new InvalidOperationException($"Doctor with id '{doctorId}' was not found.");
            }

            // Validation Test: Check if the Facility exists.
            var facility = await _facilityRepository.GetFacilityByIdAsync(facilityId);
            if (facility == null)
            {
                throw new InvalidOperationException($"Facility with id '{facilityId}' was not found.");
            }

            // If validations pass, delegate to the repository to create the relationship.
            await _facilityRepository.AssignRelationshipAsync<Doctor, Facility>(
                doctorId, facilityId, "WORKS_AT");
        }


         /// <summary>
    /// Assigns a Treatment to a facility after validating that both exist.
    /// </summary>
    /// <param name="treatmentId">ID of the treatment</param>
    /// <param name="facilityId">ID of the facility</param>
    public async Task AssignTreatmentToFacilityAsync(string treatmentId, string facilityId)
    {
        // Inline test: Ensure input values are not null or whitespace.
        if (string.IsNullOrWhiteSpace(treatmentId))
            throw new ArgumentException("Treatment id must not be null or empty.", nameof(treatmentId));
        if (string.IsNullOrWhiteSpace(facilityId))
            throw new ArgumentException("Facility id must not be null or empty.", nameof(facilityId));

        // Validation Test: Check if the Doctor exists.
        var treatment = await _treatmentRepository.GetAbstractTreatmentByIdAsync(treatmentId);
        if (treatment == null)
        {
        throw new InvalidOperationException($"Treatment with id '{treatmentId}' was not found.");        }

        // Validation Test: Check if the Facility exists.
        var facility = await _facilityRepository.GetFacilityByIdAsync(facilityId);
        if (facility == null)
        {
        throw new InvalidOperationException($"Facility with id '{facilityId}' was not found.");        }

        // If validations pass, delegate to the repository to create the relationship.
        await _facilityRepository.AssignRelationshipAsync<Treatment, Facility>(
            treatmentId, facilityId,"AVAILABLE_AT");
    }




    }

}