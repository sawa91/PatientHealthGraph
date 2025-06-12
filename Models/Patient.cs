using System;
using System.ComponentModel.DataAnnotations;
namespace HealthcareGraphAPI.Models
{
    /// <summary>
    /// Represents a patient entity.
    /// </summary>
    public class Patient : BaseEntity
    {
        /// <summary>
        /// Patient's first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }= string.Empty;
        /// <summary>
        /// Patient's last name.
        /// </summary>
        [Required]
        public string LastName { get; set; }= string.Empty;
        /// <summary>
        /// Patient's date of birth
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Patient's gender.
        /// </summary>
        
        public Gender Gender { get; set; }
        /// <summary>
        /// Patient's health card number.
        /// </summary>
        public string HealthCardNumber { get; set; }= string.Empty;
    }
}
