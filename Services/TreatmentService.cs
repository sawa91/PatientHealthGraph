using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Repositories;
using HealthcareGraphAPI.Services;

public class TreatmentService : ITreatmentService
{
    private readonly ITreatmentRepository _repository;
    private readonly IHealthInsightsService _healthInsightsService;

    public TreatmentService(ITreatmentRepository repository, IHealthInsightsService healthInsightsService)
    {
        _repository = repository;
        _healthInsightsService = healthInsightsService;
    }

    public async Task<Treatment> GetTreatmentByIdAsync(string treatmentId)
    {
        return await _repository.GetTreatmentByIdAsync(treatmentId);
    }


    public async Task<AbstractTreatment> GetAbstractTreatmentByIdAsync(string treatmentId)
    {
        return await _repository.GetAbstractTreatmentByIdAsync(treatmentId);
    }






    public async Task<IEnumerable<Treatment>> GetTreatmentsByPatientIdAsync(string patientId)
    {
        return await _repository.GetTreatmentsByPatientIdAsync(patientId);
    }

    public async Task<Treatment> CreateTreatmentAsync(string patientId, string doctorId, CreateTreatmentDTO dto)
    {
        // 1. Création de l'objet Treatment avec les informations de base.
    var treatment = new Treatment
    {
        Id = "T-" + Guid.NewGuid().ToString(),
        PatientId = patientId,
        Type = dto.Type,
        DoctorId = doctorId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        FollowUpAction = dto.FollowUpAction,
        HealthSnapshot = new HealthSnapshot()
    };
// 2. Appel au service LLM pour générer les insights (résumé et recommandation).
        var insights = await _healthInsightsService.GenerateInsightsAsync(patientId, treatment, dto);
    
    // 3. Enrichissement de l'objet Treatment avec les résultats du LLM.
    treatment.HealthSnapshot.HealthStateSummary = insights.Summary;
    treatment.HealthSnapshot.HealthRecommendation = insights.Recommendation;

        
    // 4. Persistance du snapshot dans la base de données via le repository.

        return await _repository.CreateTreatmentAsync(patientId, doctorId, treatment);
    }



    public async Task<HealthSnapshot> GetLatestHealthSnapshotAsync(string patientId)
    {
        return await _repository.GetLatestHealthSnapshotByPatientIdAsync(patientId);
    }

   /* public async Task<HealthSnapshot> GetHealthSnapshotAsOfDateAsync(string patientId, DateTime date)
    {
        return await _repository.GetHealthSnapshotAsOfDateAsync(patientId, date);
    }*/

    public async Task<IEnumerable<HealthSnapshot>> GetHealthSnapshotTimelineAsync(string patientId)
    {
        return await _repository.GetHealthSnapshotTimelineByPatientIdAsync(patientId);
    }
    
    public  Task<AbstractTreatment> CreateTreatmentAsynchrone(AbstractTreatment treatment)
        {
            
            return  _repository.CreateTreatmentAsynchrone(treatment);
        }

   
}


