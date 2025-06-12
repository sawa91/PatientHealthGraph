using HealthcareGraphAPI.Models;

public class HealthSnapshot : BaseEntity
{
    public DateTime DateSnapshot { get; set; } = DateTime.UtcNow;
    public string Details { get; set; } = string.Empty;
    // This flag indicates that the snapshot is immutable.
    public bool Immutable { get; set; } = true;

    public string HealthStateSummary { get; set; } = string.Empty;

    public string HealthRecommendation { get; set; } = string.Empty;

    //public string TreatmentId { get; set; } = string.Empty;
     
   // public string PatientId{ get; set; } = string.Empty;

}