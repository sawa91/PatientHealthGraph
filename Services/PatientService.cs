// Services/PatientService.cs
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
    /// Implements patient operations by using the repository for CRUD and the Neo4j driver for custom queries.
    /// </summary>
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDriver _driver;

        public PatientService(IPatientRepository patientRepository, IDriver driver)
        {
            _patientRepository = patientRepository;
            _driver = driver;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Patient>> GetAllPatientsAsync() => _patientRepository.GetAllAsync();

        /// <inheritdoc/>
        public Task<Patient> GetPatientByIdAsync(string id) => _patientRepository.GetByIdAsync(id);

        /// <inheritdoc/>
        public Task CreatePatientAsync(Patient patient) => _patientRepository.CreateAsync(patient);

        /// <inheritdoc/>
        public Task UpdatePatientAsync(Patient patient) => _patientRepository.UpdateAsync(patient);

        /// <inheritdoc/>
        public Task DeletePatientAsync(string id) => _patientRepository.DeleteAsync(id);

        /// <inheritdoc/>
        public async Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(string doctorId)
        {
            var query = @"
                MATCH (d:Doctor {id: $doctorId})<-[:TREATED_BY]-(p:Patient)
                RETURN p";
            var patients = new List<Patient>();

            await using var session = _driver.AsyncSession();
            try
            {
                var result = await session.RunAsync(query, new { doctorId });
                while (await result.FetchAsync())
                {
                    var node = result.Current["p"].As<INode>();
                    patients.Add(NodeMapper.MapPatient(node));
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return patients;
        }

        /// <inheritdoc/>
        public async Task<object> GetPatientNetworkGraphAsync(string patientId)
        {
            var query = @"
                MATCH (p:Patient { id: $patientId })-[r*1..2]-(n)
                RETURN p, r, n";
            var results = new List<object>();

            await using var session = _driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query, new { patientId });
                while (await cursor.FetchAsync())
                {
                    results.Add(cursor.Current);
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return results;
        }


        /// <summary>
        /// Assigns a patient to a doctor .
        /// </summary>
        /// <param name="patientId">ID of the patient</param>
        /// <param name="doctorId">ID of the doctor</param>
        /// 

        public async Task AssignPatienttToDoctorAsync<Patient, Doctor>(string patientId, string doctorId)
        {
            await _patientRepository.AssignRelationshipAsync<Patient, Doctor>(
             patientId, doctorId, "TREATED_BY");
        }

    }
}



