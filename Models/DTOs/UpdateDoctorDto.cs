using System;
using HealthcareGraphAPI.Models;

namespace HealthcareGraphAPI.Models.DTOs
{
    /// <summary>
    /// Represents the data for updating an existing doctor.
    /// Fields are optional so that only provided values are updated.
    /// </summary>
    public class UpdateDoctorDto
    {
        // Make these optional (if property is null, it won't update that field)
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? StartYear { get; set; }
        public Gender? Gender { get; set; }
        public string? LicenseNumber { get; set; }

        public string? Specialization { get; set; }
    }
}
