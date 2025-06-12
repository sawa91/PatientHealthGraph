// Models/DTOs/CreatePatientDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// Represents the data required to create a new doctor.
    /// Note: Excludes properties that are auto-generated (Id, CreatedAt, UpdatedAt).
    /// </summary>
    public class CreateDoctorDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string StartYear { get; set; }= string.Empty;

        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
         [Required]
        public string Specialization { get; set; } = string.Empty;
    }
}
