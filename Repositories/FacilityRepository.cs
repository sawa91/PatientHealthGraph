using HealthcareGraphAPI.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareGraphAPI.Repositories
{
    public class FacilityRepository : BaseRepository<Facility>, IFacilityRepository
    {
        public FacilityRepository(IDriver driver) : base(driver) { }

        /// <summary>
        /// Node label used in Neo4j for Facility nodes.
        /// </summary>
        protected override string NodeLabel => "Facility";

        /// <summary>
        /// Maps a Neo4j INode to a Facility object.
        /// </summary>
        protected override Facility Map(INode node)
        {
            var facility = new Facility
            {
                Id = node.Properties["id"].As<string>(),
                Name = node.Properties["name"].As<string>(),
                Type = Enum.TryParse<FacilityType>(node.Properties["type"].As<string>(), ignoreCase: true, out var facilityType)
                            ? facilityType : FacilityType.Unknown,
                Capacity = node.Properties.ContainsKey("capacity") ? node.Properties["capacity"].As<int>() : 0,
                Active = node.Properties.ContainsKey("active") ? node.Properties["active"].As<bool>() : false,
                CreatedAt = node.Properties.ContainsKey("createdAt")
                            ? node.Properties["createdAt"].As<ZonedDateTime>().ToDateTimeOffset().UtcDateTime
                            : DateTime.MinValue,
                UpdatedAt = node.Properties.ContainsKey("updatedAt")
                            ? node.Properties["updatedAt"].As<ZonedDateTime>().ToDateTimeOffset().UtcDateTime
                            : DateTime.MinValue,
                ServicesOffered = node.Properties.ContainsKey("servicesOffered")
                            ? node.Properties["servicesOffered"].As<List<object>>().Select(s => Enum.Parse<ServiceType>(s.ToString())).ToList()
                            : new List<ServiceType>(),
                // Leave contacts empty for now.
                Contacts = new List<ContactInfo>()
            };
            return facility;
        }


        private async Task<List<ContactInfo>> FetchFacilityContactsAsync(string facilityId, IAsyncSession session)
        {
            var contacts = new List<ContactInfo>();

            // Open a transaction (or use one that's already in use if your query method supports it)
            await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync(@"
            MATCH (f:Facility {id: $id})-[:HAS_CONTACT]->(c:ContactInfo)
            RETURN c.id AS id, c.type AS type, c.value AS value
        ", new { id = facilityId });

                await result.ForEachAsync(record =>
                {
                    contacts.Add(new ContactInfo
                    {
                        Id = record["id"].As<string>(),
                        Type = record["type"].As<string>(),
                        Value = record["value"].As<string>()
                    });
                });
            });

            return contacts;
        }



        /// <summary>
        /// Creates a dictionary of properties for a Facility.
        /// </summary>
        protected override IDictionary<string, object> CreateParameters(Facility entity)
        {
            return new Dictionary<string, object>
            {
                {"id", entity.Id},
                {"name", entity.Name},
                // Store enum values as strings.
                {"type", entity.Type.ToString()},
                {"capacity", entity.Capacity},
                // Convert the list of ServiceType enums to a list of strings.
                {"servicesOffered", entity.ServicesOffered.Select(s => s.ToString()).ToList()},
                // Convert each contact to a dictionary with keys "type" and "value".
                {"contacts", entity.Contacts.Select(c => new Dictionary<string, object>
                    {
                        {"type", c.Type},
                        {"value", c.Value}
                    }).ToList()},
                {"active", entity.Active},
                {"createdAt", entity.CreatedAt.ToUniversalTime()},
                {"updatedAt", entity.UpdatedAt.ToUniversalTime()}
            };
        }


        public async Task<Facility> GetFacilityByIdAsync(string facilityId)
        {
            Facility facility = null;
            // Assume _driver is your Neo4j driver injection.
            await using var session = _driver.AsyncSession();

            // Run a read transaction to get the facility node.
            facility = await session.ExecuteReadAsync(async tx =>
            {
                var result = await tx.RunAsync("MATCH (f:Facility {id: $id}) RETURN f", new { id = facilityId });
                var record = await result.SingleAsync();
                // Use the base Map method to map core properties
                return Map(record["f"].As<INode>());
            });

            // Now that we have the facility, fetch its contacts:
            if (facility != null)
            {
                var contacts = await FetchFacilityContactsAsync(facility.Id, session);
                facility.Contacts = contacts;
            }

            return facility;
        }

    }
}
