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
    /// Implements doctor operations by using the repository for CRUD and the Neo4j driver for custom queries.
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IDriver _driver;

        public DoctorService(IDoctorRepository doctorRepository, ITreatmentRepository treatmentRepository, IDriver driver)
        {
            _doctorRepository = doctorRepository;
            _treatmentRepository = treatmentRepository; ;
            _driver = driver;
        }

        public Task<IEnumerable<Doctor>> GetAllDoctorssAsync() => _doctorRepository.GetAllAsync();

        /// <inheritdoc/>
        public Task<Doctor> GetDoctorByIdAsync(string id) => _doctorRepository.GetByIdAsync(id);

        /// <inheritdoc/>
        public Task CreateDoctorAsync(Doctor doctor) => _doctorRepository.CreateAsync(doctor);

        /// <inheritdoc/>
        public Task UpdateDoctorAsync(Doctor doctor) => _doctorRepository.UpdateAsync(doctor);

        /// <inheritdoc/>
        public Task DeleteDoctorAsync(string id) => _doctorRepository.DeleteAsync(id);



        public async Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(string doctorId)
        {
            return await _doctorRepository.GetAllSourcesByCriteriaAsync<Patient>("TREATED_BY", "Doctor", doctorId, NodeMapper.MapPatient);
        }

        public async Task<IEnumerable<Treatment>> GetTreatmentsByDoctorAsync(string doctorId)
        {
            return await _doctorRepository.GetAllTargetsByCriteriaAsync<Treatment>("SPECIALIZES_IN_TREATMENT", "Doctor", doctorId, NodeMapper.MapTreatment);

        }

        /// <summary>
        /// Assigns a Treatment to a doctor after validating that both exist.
        /// </summary>
        /// <param name="treatmentId">ID of the treatment</param>
        /// <param name="doctorId">ID of the doctor</param>
        public async Task AssignTreatmentToFacilityAsync(string treatmentId, string doctorId)
        {
            // Inline test: Ensure input values are not null or whitespace.
            if (string.IsNullOrWhiteSpace(treatmentId))
                throw new ArgumentException("Treatment id must not be null or empty.", nameof(treatmentId));
            if (string.IsNullOrWhiteSpace(doctorId))
                throw new ArgumentException("doctor id must not be null or empty.", nameof(doctorId));

            // Validation Test: Check if the Doctor exists.
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(treatmentId);
            if (treatment == null)
            {
                throw new InvalidOperationException($"Treatment with id '{treatmentId}' was not found.");
            }

            // Validation Test: Check if the Facility exists.
            var facility = await _doctorRepository.GetByIdAsync(doctorId);
            if (facility == null)
            {
                throw new InvalidOperationException($"doctor with id '{doctorId}' was not found.");
            }

            // If validations pass, delegate to the repository to create the relationship.
            await _doctorRepository.AssignRelationshipAsync<Treatment, Doctor>(
               treatmentId, doctorId, "SPECIALIZES_IN_TREATMENT");
        }

        public async Task AssignTreatmentToDoctorAsync<Treatment, Doctor>(string treatmentId, string doctorId)
        {
            await _doctorRepository.AssignRelationshipAsync<Treatment, Doctor>(
                    treatmentId, doctorId, "SPECIALIZES_IN_TREATMENT");
        }
        

         /// <summary>
    /// Assigns a Treatment to a doctor after validating that both exist.
    /// </summary>
    /// <param name="treatmentId">ID of the treatment</param>
    /// <param name="doctorId">ID of the facility</param>
    public async Task AssignTreatmentToDoctorAsync(string doctorId, string treatmentId)
    {
        // Inline test: Ensure input values are not null or whitespace.
        if (string.IsNullOrWhiteSpace(treatmentId))
            throw new ArgumentException("Treatment id must not be null or empty.", nameof(treatmentId));
        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor id must not be null or empty.", nameof(doctorId));

        // Validation Test: Check if the Doctor exists.
        var treatment = await _treatmentRepository.GetAbstractTreatmentByIdAsync(treatmentId);
        if (treatment == null)
        {
        throw new InvalidOperationException($"Treatment with id '{treatmentId}' was not found.");        }

        // Validation Test: Check if the Facility exists.
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null)
        {
        throw new InvalidOperationException($"doctor with id '{doctorId}' was not found.");        }

        // If validations pass, delegate to the repository to create the relationship.
        await _doctorRepository.AssignRelationshipAsync<Doctor, Treatment>(
            doctorId,treatmentId,"SPECIALIZES_IN_TREATMENT");
    }


    }
}

