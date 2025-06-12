// Repositories/PatientRepository.cs
using HealthcareGraphAPI.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;

namespace HealthcareGraphAPI.Repositories
{
    /// <summary>
    /// Concrete repository implementation for Patient-specific operations.
    /// </summary>
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(IDriver driver) : base(driver) { }

        /// <summary>
        /// Node label used in Neo4j for Patient nodes.
        /// </summary>
        protected override string NodeLabel => "Patient";

        /// <summary>
        /// Maps a Neo4j INode to a Patient object.
        /// </summary>
        protected override Patient Map(INode node)
        {
            return new Patient
            {
                Id = node.Properties["id"].As<string>(),
                FirstName = node.Properties["firstName"].As<string>(),
                LastName = node.Properties["lastName"].As<string>(),
                DateOfBirth = node.Properties.ContainsKey("dateOfBirth")
            ? new DateTime(
                node.Properties["dateOfBirth"].As<LocalDate>().Year,
                node.Properties["dateOfBirth"].As<LocalDate>().Month,
                node.Properties["dateOfBirth"].As<LocalDate>().Day) : DateTime.MinValue,
                Gender = Enum.Parse<Gender>(node.Properties["gender"].As<string>()),
                HealthCardNumber = node.Properties["healthCardNumber"].As<string>(),
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
        protected override IDictionary<string, object> CreateParameters(Patient entity)
        {
            return new Dictionary<string, object>
            {
                {"id", entity.Id},
                {"firstName", entity.FirstName},
                {"lastName", entity.LastName},
                {"dateOfBirth", new LocalDate(entity.DateOfBirth.Year, entity.DateOfBirth.Month, entity.DateOfBirth.Day)},
                { "gender", entity.Gender.ToString()},
                {"healthCardNumber", entity.HealthCardNumber},
                {"createdAt", entity.CreatedAt.ToUniversalTime()},
                {"active", entity.Active}, 
                {"updatedAt", entity.UpdatedAt.ToUniversalTime()} 
  
  
            };
        }
    }
}
