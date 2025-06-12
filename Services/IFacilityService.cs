// Services/IPatientService.cs
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Services
{
    /// <summary>
    /// Service interface exposing facility operations including CRUD and custom queries.
    /// </summary>
    public interface IFacilityService
    {
        /// <summary>
        /// Retrieves all facility.
        /// </summary>
        Task<IEnumerable<Facility>> GetAllFacilitiesAsync();

        /// <summary>
        /// Retrieves a facility by their ID.
        /// </summary>
        Task<Facility> GetFacilityByIdAsync(string id);


        /// <summary>
        /// Creates a new facility.
        /// </summary>
        Task CreateFacilityAsync(Facility facility);

        /// <summary>
        /// Updates an existing facility.
        /// </summary>
        Task UpdateFacilityAsync(Facility facility);

        /// <summary>
        /// Deletes a facility by their ID.
        /// </summary>
        Task DeleteFacilityAsync(string id);

        Task<IEnumerable<Doctor>> GetDoctorsByFacilityAsync(string facilityId);
        Task<IEnumerable<AbstractTreatment>> GetTreatmentsAvailableAtFacilityAsync(string facilityId);

        Task AssignDoctorToFacilityAsync(string doctorId, string facilityId);
        Task AssignTreatmentToFacilityAsync(string treatmentId, string facilityId);










    }
}
