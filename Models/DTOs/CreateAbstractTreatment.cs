using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
public class CreateAbstractTreatmentDTO
{

    [Required]
    public string Type { get; set; } = string.Empty;


    
}