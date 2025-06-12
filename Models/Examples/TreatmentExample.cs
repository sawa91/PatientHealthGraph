using HealthcareGraphAPI.Models.DTOs;

public static class TreatmentExample
{
    public static CreateTreatmentDTO Example { get; } = new CreateTreatmentDTO
    {
        Type = "Prescripton",
        PatientId = "ef1240563xgs",
        DoctorId = "doctor-456",
        FollowUpAction = "take Paracetamol 200 mg 3 times a day, PErform blood work after 2 weeks"
        };
    }
