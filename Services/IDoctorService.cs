// Services/IPatientService.cs
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Services
{
    /// <summary>
    /// Service interface exposing doctor operations including CRUD and custom queries.
    /// </summary>
    public interface IDoctorService
    {
        /// <summary>
        /// Retrieves all doctors.
        /// </summary>
        Task<IEnumerable<Doctor>> GetAllDoctorssAsync();
        // <summary>
        // Retrieves all doctors by facility.
        // </summary>
        // Task<IEnumerable<Doctor>> GetAllDoctorsByFacilityAsync();

        /// <summary>
        /// Retrieves a doctors by their ID.
        /// </summary>
        Task<Doctor> GetDoctorByIdAsync(string id);

        /// <summary>
        /// Creates a new doctor.
        /// </summary>
        Task CreateDoctorAsync(Doctor doctor);

        /// <summary>
        /// Updates an existing doctor.
        /// </summary>
        Task UpdateDoctorAsync(Doctor doctor);

        /// <summary>
        /// Deletes a doctor by their ID.
        /// </summary>
        Task DeleteDoctorAsync(string id);

        Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(string doctorId);
        Task<IEnumerable<Treatment>> GetTreatmentsByDoctorAsync(string doctorId);

        Task AssignTreatmentToDoctorAsync<Treatment, Doctor>(string treatmentId, string doctorId);

        }







    
}
