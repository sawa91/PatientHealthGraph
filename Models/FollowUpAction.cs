using HealthcareGraphAPI.Models;

public class FollowUpAction: BaseEntity
{

    // Broad classification (e.g., "Medication", "LabTest","Radiology")
    public string Category { get; set; } = string.Empty;
    
    // Specific follow-up item (e.g., "Ibuprofen", "Vitamin D")
    public string Name { get; set; }= string.Empty;
    
    // The measurement or outcome value (e.g., "200", "Low")
    public string Value { get; set; }= string.Empty;
    
    // Additional details or description (e.g., "dosage in mg", "results of tests")
    public string Remarks { get; set; }= string.Empty;
    
    // The date when this follow-up action was recorded or performed
    public DateTime Date { get; set; }
}
