using System.Collections.Generic;
using System.Text.Json.Serialization;
 

namespace HealthcareGraphAPI.Models
{
    public class AbstractTreatment : BaseEntity
{
    //instances of Patient Treatment with date of treatment and follow up actions fields.
        public string Type { get; set; } = string.Empty;
 

        public bool Isabstract { get; set; } = true;

   
}    
}