using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;

public interface ITreatmentRepository 
{
    // Retrieve detailed treatment information (including its issuing doctor and follow‑up actions)
    Task<Treatment> GetTreatmentByIdAsync(string treatmentId);
    Task<AbstractTreatment> GetAbstractTreatmentByIdAsync(string treatmentId);


    // Retrieve all treatments for a given patient
    Task<IEnumerable<Treatment>> GetTreatmentsByPatientIdAsync(string patientId);

    // Create a new treatment record given the patient and doctor identifiers along with treatment details
    Task<Treatment> CreateTreatmentAsync(string patientId, string doctorId, Treatment dto);

   
    // Retrieve the latest health snapshot for a given patient
    Task<HealthSnapshot> GetLatestHealthSnapshotByPatientIdAsync(string patientId);

   
    // Retrieve the timeline (full list) of health snapshots for a given patient
    Task<IEnumerable<HealthSnapshot>> GetHealthSnapshotTimelineByPatientIdAsync(string patientId);
    Task<AbstractTreatment> CreateTreatmentAsynchrone(AbstractTreatment treatment);
}
