using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using HealthcareGraphAPI.Models;
using Neo4j.Driver;

[ApiController]
[Route("api")]
public class TreatmentsController : ControllerBase
{
    private readonly ITreatmentService _treatmentService;
        private readonly IAbstractTreatmentService _abstreatmentService;

    public TreatmentsController(ITreatmentService treatmentService)
    {
        _treatmentService = treatmentService;
    }

    /// <summary>
    /// Retrieves a patient treatment by its unique identifier.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the patient treatment.</param>
    /// <returns>The treatment with the specified id.</returns>
    [HttpGet("treatments/{treatmentId}")]
    [SwaggerResponse(200, "Treatment retrieved successfully", typeof(Treatment))]
    [SwaggerResponse(404, "Treatment not found")]
    public async Task<IActionResult> GetTreatment(string treatmentId)
    {
        var treatment = await _treatmentService.GetTreatmentByIdAsync(treatmentId);
        if (treatment == null)
        {
            return NotFound();
        }
        return Ok(treatment);
    }

     /// <summary>
    /// Retrieves an abstract treatment by its unique identifier.
    /// </summary>
    /// <param name="treatmentAbstractId">The unique identifier of the abstract treatment.</param>
    /// <returns>The treatment with the specified id.</returns>
    [HttpGet("abstracttreatments/{treatmentId}")]
    [SwaggerResponse(200, "Treatment retrieved successfully", typeof(AbstractTreatment))]
    [SwaggerResponse(404, "Treatment not found")]
    public async Task<IActionResult> GetAbstractTreatment(string treatmentAbstractId)
    {
        var treatment = await _treatmentService.GetTreatmentByIdAsync(treatmentAbstractId);
        if (treatment == null)
        {
            return NotFound();
        }
        return Ok(treatment);
    }

      

    /// <summary>
    /// Retrieves all treatments for a given patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>A list of treatments for the specified patient.</returns>
    [HttpGet("patients/{patientId}/treatments")]
    [SwaggerResponse(200, "Treatments retrieved successfully", typeof(IEnumerable<Treatment>))]
    public async Task<IActionResult> GetTreatmentsForPatient(string patientId)
    {
        var treatments = await _treatmentService.GetTreatmentsByPatientIdAsync(patientId);
        return Ok(treatments);
    }
    /// <summary>
    /// Creates a new Treatment that may be available at a Facility or a doctor got specialized on it.
    /// </summary>
    /// <param name="treatment">The patient object to create.</param>
    /// <returns>The created patient object and location header.</returns>

    [HttpPost("abstractTreatments")]
public async Task<IActionResult> CreateAbstractTreatment([FromBody] CreateAbstractTreatmentDTO  createDto)
{
    if (createDto == null)
    {
        return BadRequest("Les données du traitement sont obligatoires.");
    }

    // Mapper le DTO en modèle de domaine
    var treatment = new AbstractTreatment
    {
        Id = "T-" + Guid.NewGuid().ToString(), // Génération automatique de l'ID
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Active = true,
        Isabstract = true,
        Type = createDto.Type,
        // Mapper d'autres champs si nécessaire
    };

    await _treatmentService.CreateTreatmentAsynchrone(treatment);

    // Retourne une réponse HTTP 201 Created avec l'URL du traitement créé.
    return CreatedAtAction(nameof(GetAbstractTreatment), new { treatmentId = treatment.Id }, treatment);
}


    /// <summary>
    /// Creates a patient Snapshot record.
    /// </summary>
    /// <remarks>
    /// both PatientId and DoctorId need to be known form context (authentification, route).
    /// Here to simplify, we fill them as for the other fields of the patient treatment record 
    /// Fields HealthStateSummary and Health Recommendatio are generated using an LLM
    /// FollowUpAction meant to be an object/Node with fields: "followUpActions": 
    /// {Category, name, value, remarks} for example {LabTest, Test of Vitamin D, result: Low, repeat test in 3 months} but to simplify here it is considered a string
    /// </remarks>
    /// <param name="dto">The treatment creation data transfer object.</param>
    /// <returns>The newly created treatment record.</returns>
    [HttpPost("treatments")]
    [SwaggerRequestExample(typeof(CreateTreatmentDTO), typeof(TreatmentExample))]
    [SwaggerResponse(201, "Snapshot created successfully", typeof(Treatment))]
    [SwaggerResponse(400, "Invalid input: PatientId and DoctorId are required")]
    public async Task<IActionResult> CreateTreatment([FromBody] CreateTreatmentDTO dto)
    {
        if (string.IsNullOrEmpty(dto.PatientId) || string.IsNullOrEmpty(dto.DoctorId))
        {
            return BadRequest("PatientId and DoctorId are required in the payload.");
        }

        var createdTreatment = await _treatmentService.CreateTreatmentAsync(dto.PatientId, dto.DoctorId, dto);
        return CreatedAtAction(nameof(GetTreatment), new { treatmentId = createdTreatment.Id }, createdTreatment);
    }

    
    /// <summary>
    /// Retrieves the latest health state (snapshot) of a patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>The most recent health snapshot of the patient.</returns>
    [HttpGet("patients/{patientId}/healthstate/latest")]
    [SwaggerResponse(200, "Latest health state retrieved successfully", typeof(HealthSnapshot))]
    [SwaggerResponse(404, "Latest health state not found")]
    public async Task<IActionResult> GetLatestHealthState(string patientId)
    {
        var snapshot = await _treatmentService.GetLatestHealthSnapshotAsync(patientId);
        if (snapshot == null)
        {
            return NotFound();
        }
        return Ok(snapshot);
    }

    

    /// <summary>
    /// Retrieves the full timeline of health snapshots for a given patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>A chronological list of health snapshots.</returns>
    [HttpGet("patients/{patientId}/healthstate/timeline")]
    [SwaggerResponse(200, "Health state timeline retrieved successfully", typeof(IEnumerable<Treatment>))]
    public async Task<IActionResult> GetHealthStateTimeline(string patientId)
    {
        var snapshots = await _treatmentService.GetHealthSnapshotTimelineAsync(patientId);
        return Ok(snapshots);
    }
}

internal interface IAbstractTreatmentService
{
}