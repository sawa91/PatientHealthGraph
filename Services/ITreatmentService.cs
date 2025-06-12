using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareGraphAPI.Models;


public interface ITreatmentService
{
    Task<Treatment> GetTreatmentByIdAsync(string treatmentId);

    Task<AbstractTreatment> CreateTreatmentAsynchrone(AbstractTreatment treatment);
    Task<Treatment> CreateTreatmentAsync(string patientId, string doctorId, CreateTreatmentDTO dto);

    Task<IEnumerable<Treatment>> GetTreatmentsByPatientIdAsync(string patientId);

    Task<HealthSnapshot> GetLatestHealthSnapshotAsync(string patientId);
    Task<IEnumerable<HealthSnapshot>> GetHealthSnapshotTimelineAsync(string patientId);

}
