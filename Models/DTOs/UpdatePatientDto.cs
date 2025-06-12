// Models/DTOs/UpdatePatientDto.cs
using System;
using HealthcareGraphAPI.Models;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// Represents the data for updating an existing patient.
    /// Fields are optional so that only provided values are updated.
    /// </summary>
    public class UpdatePatientDto
    {
        // Make these optional (if property is null, it won't update that field)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; } 
        public Gender? Gender { get; set; }
        public string? HealthCardNumber { get; set; }
    }
}
