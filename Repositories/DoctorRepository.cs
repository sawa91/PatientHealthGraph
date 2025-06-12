// Repositories/PatientRepository.cs
using HealthcareGraphAPI.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;

namespace HealthcareGraphAPI.Repositories
{
    /// <summary>
    /// Concrete repository implementation for Dcotor-specific operations.
    /// </summary>
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(IDriver driver) : base(driver) { }

        /// <summary>
        /// Node label used in Neo4j for Doctor nodes.
        /// </summary>
        protected override string NodeLabel => "Doctor";

        /// <summary>
        /// Maps a Neo4j INode to a Doctor object.
        /// </summary>
        protected override Doctor Map(INode node)
        {
            return new Doctor
            {
                Id = node.Properties["id"].As<string>(),
                FirstName = node.Properties["firstName"].As<string>(),
                LastName = node.Properties["lastName"].As<string>(),
                StartYear = node.Properties["startYear"].As<string>(),
                Gender = Enum.Parse<Gender>(node.Properties["gender"].As<string>()),
                Specialization = node.Properties["specialization"].As<string>(),
                LicenseNumber = node.Properties["licenseNumber"].As<string>(),
                Active = bool.TryParse(node.Properties["active"].As<string>(), out var isActive) ? isActive : false,
                CreatedAt = node.Properties.ContainsKey("createdAt")
            ? node.Properties["createdAt"].As<ZonedDateTime>().ToDateTimeOffset().UtcDateTime
            : DateTime.MinValue,
            UpdatedAt = node.Properties.ContainsKey("updatedAt") 
            ? node.Properties["updatedAt"].As<ZonedDateTime>().ToDateTimeOffset().UtcDateTime  
            : DateTime.MinValue 
 
 

            };
        }

        /// <summary>
        /// Creates a dictionary of properties for a Patient.
        /// </summary>
        protected override IDictionary<string, object> CreateParameters(Doctor entity)
        {
            return new Dictionary<string, object>
            {
                {"id", entity.Id},
                {"firstName", entity.FirstName},
                {"lastName", entity.LastName},
                {"startYear", entity.StartYear.ToString()},
                {"gender", entity.Gender.ToString()},
                {"licenseNumber", entity.LicenseNumber},
                {"specialization", entity.Specialization},
                {"createdAt", entity.CreatedAt.ToUniversalTime()},
                {"active", entity.Active}, 
               {"updatedAt", entity.UpdatedAt.ToUniversalTime()} 
  
            };
        }
    }
}
