using System;
namespace HealthcareGraphAPI.Models
{
    /// <summary>
    /// Defines a domain entity with an Id property.
    /// </summary>
    public interface IEntity
    {

        /// <summary>
        /// Unique identifier for the entity.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated.
        /// </summary>
        DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Status of the entity. Inactive entities are maintained for record-keeping purposes, such as former patients or retired doctors.
        /// </summary>
        bool Active { get; set; }
    }
}
