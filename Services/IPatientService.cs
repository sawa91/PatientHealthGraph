// Services/IPatientService.cs
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Services
{
    /// <summary>
    /// Service interface exposing patient operations including CRUD and custom queries.
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        Task<IEnumerable<Patient>> GetAllPatientsAsync();

        /// <summary>
        /// Retrieves a patient by their ID.
        /// </summary>
        Task<Patient> GetPatientByIdAsync(string id);

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        Task CreatePatientAsync(Patient patient);

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        Task UpdatePatientAsync(Patient patient);

        /// <summary>
        /// Deletes a patient by their ID.
        /// </summary>
        Task DeletePatientAsync(string id);

        /// <summary>
        /// Retrieves all patients treated by a specific doctor.
        /// </summary>
        Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(string doctorId);

        /// <summary>
        /// Retrieves the network of relationships for a given patient.
        /// </summary>
        Task<object> GetPatientNetworkGraphAsync(string patientId);



        Task AssignPatienttToDoctorAsync<Patient,Doctor>(string patientId, string doctorId);
        
        
    


    }
}
