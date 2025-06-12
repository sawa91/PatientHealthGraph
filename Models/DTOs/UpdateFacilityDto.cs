// Models/DTOs/CreatePatientDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// Represents the data required to create a new doctor.
    /// Note: Excludes properties that are auto-generated (Id, CreatedAt, UpdatedAt).
    /// </summary>
    public class UpdateFacilityDto
    {
        [Required]
        public string?  Name { get; set; } = string.Empty;

        public int? Capacity { get; set; }

        public string? Type { get; set; } = string.Empty;

        public List<string>? ServicesOffered { get; set; } = new List<string>();

        public List<ContactDto>? Contacts { get; set; }
}

        
    
}