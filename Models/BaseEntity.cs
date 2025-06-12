using System;
using System.ComponentModel.DataAnnotations;
namespace HealthcareGraphAPI.Models
{
    /// <summary>
    /// Base class for entities providing common historisation attributes.
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// Unique identifier for the entity.
        /// </summary>
        [Required]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the entity was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Status of the entity. Inactive entities are maintained for record-keeping purposes, such as former patients or retired doctors.
        /// </summary>
        public bool Active { get; set; } = true;
    }
}