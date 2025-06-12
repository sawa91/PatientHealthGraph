// Models/DTOs/CreatePatientDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// Represents the data required to create a new patient.
    /// Note: Excludes properties that are auto-generated (Id, CreatedAt, UpdatedAt).
    /// </summary>
    public class CreatePatientDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [SwaggerSchema(Format = "date", Description = "Date of birth in YYYY-MM-DD format")]
        public string DateOfBirth { get; set; } 
        
        [Required]
        public Gender Gender { get; set; }
        
        public string HealthCardNumber { get; set; } = string.Empty;
    }
}
