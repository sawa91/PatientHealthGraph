using System;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// DTO to represent a patient's relative.
    /// </summary>
    public class RelativeDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Represents the relationship type (e.g., spouse, child, parent).
        public string Relationship { get; set; } = string.Empty;
         
         public Boolean isEmergencyContact { get; set; } = false;
    }
}
