using System;
using System.ComponentModel.DataAnnotations;
namespace HealthcareGraphAPI.Models
{
    /// <summary>
    /// Represents a Doctor entity.
    /// </summary>
    public class Doctor : BaseEntity
    {
        /// <summary>
        /// Doctor's first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Doctor's last name.
        /// </summary>
        [Required]
        public string LastName { get; set; } = string.Empty;
        /// <summary>
        /// Doctor's start date of work
        /// </summary>
        [Required]
        public string StartYear { get; set; }= string.Empty;
        /// <summary>
        /// Doctor's gender.
        /// </summary>

        public Gender Gender { get; set; }
        /// <summary>
        /// Doctor's health card number.
        /// </summary>
        public string LicenseNumber { get; set; } = string.Empty;
          /// <summary>
        /// Doctor's Specialization. To simplify it is considered a single field.
        /// </summary>
        public string Specialization { get; set; }= string.Empty;
    }
}
