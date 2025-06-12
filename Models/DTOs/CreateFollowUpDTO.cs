public class CreateFollowUpDTO
{
    // The broad classification for the follow-up (e.g., "Medication" or "LabTest")
    public string Category { get; set; } = string.Empty;
    
    // The specific item name (e.g., "Ibuprofen", "Vitamin D")
    public string Name { get; set; }= string.Empty;
    
    // The measurement or outcome (e.g., "200" for dosage or "Low" for a test result)
    public string Value { get; set; }= string.Empty;
    
    // Additional context like "dosage en mg" or "results of tests"
    public string Remarks { get; set; }= string.Empty;
    
    // The date when this follow-up entry was recorded
    public string Date { get; set; }= string.Empty;
}
