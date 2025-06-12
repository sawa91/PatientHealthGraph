using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
public class CreateTreatmentDTO
{
    // PatientId and DoctorId are to be extracted from the authentication context and the URL but currently we fill them with the other data. 

    [Required]
    public string Type { get; set; } = string.Empty;
    [Required]
    public string PatientId { get; set; } = string.Empty;
    [Required]
    public string DoctorId { get; set; } = string.Empty;
    [Required]


    // For creation, we also define a DTO for follow-up details.
    public string FollowUpAction { get; set; }= string.Empty;
    
}