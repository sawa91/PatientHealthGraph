using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI.Models.DTOs;
using HealthcareGraphAPI.Repositories;

public class TreatmentRepository : BaseRepository<Treatment>, ITreatmentRepository
{

    protected override string NodeLabel => throw new NotImplementedException();

    public TreatmentRepository(IDriver driver) : base(driver) { }


    // 1. Retrieve detailed treatment information by treatmentId.
 public async Task<Treatment> GetTreatmentByIdAsync(string treatmentId)
{
    await using var session = _driver.AsyncSession();
    var query = @"
        MATCH (t:Treatment {id: $treatmentId})
        OPTIONAL MATCH (d:Doctor)-[:ISSUES]->(t)
        OPTIONAL MATCH (t)-[:GENERATES]->(hs:HealthSnapshot)
        RETURN t, d, hs
    ";

    // Affectation du résultat de la lambda à la variable resultTreatment.
    var resultTreatment = await session.ExecuteReadAsync(async tx =>
    {
        var cursor = await tx.RunAsync(query, new { treatmentId });
        var record = await cursor.SingleAsync();

        // Map the treatment node.
        var tNode = record["t"].As<INode>();
        var mappedTreatment = new Treatment
        {
            Id = tNode.Properties["id"].As<string>(),
            Type = tNode.Properties["type"].As<string>(),
            Date = DateTime.Parse(tNode.Properties["date"].As<string>()),
            DoctorId = tNode.Properties["doctorId"].As<string>(),
            FollowUpAction = tNode.Properties["followUpAction"].As<string>()
        };

        // Map health snapshot, if exists.
        var hsNode = record["hs"]?.As<INode>();
        mappedTreatment.HealthSnapshot = hsNode != null ? new HealthSnapshot
        {
            Id = hsNode.Properties["id"].As<string>(),
            CreatedAt = DateTime.Parse(hsNode.Properties["createdAt"].As<string>()),
            Details = hsNode.Properties["details"].As<string>(),
            Immutable = hsNode.Properties["immutable"].As<bool>(),
            HealthStateSummary = hsNode.Properties.ContainsKey("healthStateSummary") 
                                ? hsNode.Properties["healthStateSummary"].As<string>() 
                                : string.Empty,
            HealthRecommendation = hsNode.Properties.ContainsKey("healthRecommendation") 
                                ? hsNode.Properties["healthRecommendation"].As<string>() 
                                : string.Empty
        } : null;

        return mappedTreatment;
    });

    // Retourne le Treatment mappé
    return resultTreatment;
}


    public async Task<AbstractTreatment> GetAbstractTreatmentByIdAsync(string treatmentId)
    {
        using var session = _driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(
                "MATCH (t:Treatment {id: $id}) RETURN t",
                new { id = treatmentId });

            if (await cursor.FetchAsync())
            {
                var record = cursor.Current;
                var node = record["t"].As<INode>();
                return new AbstractTreatment
                {
                    Id = node.Properties.ContainsKey("id") ? node.Properties["id"].As<string>() : string.Empty,
                    Type = node.Properties.ContainsKey("type") ? node.Properties["type"].As<string>() : string.Empty,
                    CreatedAt = node.Properties.ContainsKey("createdAt")
                                    ? DateTime.Parse(node.Properties["createdAt"].As<string>())
                                    : DateTime.MinValue,
                    UpdatedAt = node.Properties.ContainsKey("updatedAt")
                                    ? DateTime.Parse(node.Properties["updatedAt"].As<string>())
                                    : DateTime.MinValue,
                    Active = node.Properties.ContainsKey("active") && node.Properties["active"].As<bool>(),
                    Isabstract = node.Properties.ContainsKey("isabstract") && node.Properties["isabstract"].As<bool>()
                };
            }

            // Si aucun nœud n'est trouvé, on retourne null.
            return null;
        });
    }


    

    // 2. Retrieve all treatments for a given patient.

    public async Task<IEnumerable<Treatment>> GetTreatmentsByPatientIdAsync(string patientId)
{
    var treatments = new List<Treatment>();

    await using var session = _driver.AsyncSession();
    var query = @"
        MATCH (p:Patient {id: $patientId})-[:UNDERGOES]->(t:Treatment)
        OPTIONAL MATCH (d:Doctor)-[:ISSUES]->(t)
        OPTIONAL MATCH (t)-[:GENERATES]->(hs:HealthSnapshot)
        RETURN t, d, hs
    ";

    await session.ExecuteReadAsync(async tx =>
    {
        var cursor = await tx.RunAsync(query, new { patientId });
        while (await cursor.FetchAsync())
        {
            var tNode = cursor.Current["t"].As<INode>();
            var treatment = new Treatment
            {
                Id = tNode.Properties.ContainsKey("id") ? tNode.Properties["id"].As<string>() : string.Empty,
                Type = tNode.Properties.ContainsKey("type") ? tNode.Properties["type"].As<string>() : string.Empty,
                Date = tNode.Properties.ContainsKey("date")
                            ? DateTime.Parse(tNode.Properties["date"].As<string>())
                            : DateTime.MinValue,
                DoctorId = tNode.Properties.ContainsKey("doctorId") ? tNode.Properties["doctorId"].As<string>() : string.Empty,
                FollowUpAction = tNode.Properties.ContainsKey("followUpAction") ? tNode.Properties["followUpAction"].As<string>() : string.Empty
            };

            // Map the HealthSnapshot node if it exists.
            var hsNode = cursor.Current["hs"]?.As<INode>();
            if (hsNode != null)
            {
                treatment.HealthSnapshot = new HealthSnapshot
                {
                    Id = hsNode.Properties.ContainsKey("id") ? hsNode.Properties["id"].As<string>() : string.Empty,
                    CreatedAt = hsNode.Properties.ContainsKey("createdAt") ? DateTime.Parse(hsNode.Properties["createdAt"].As<string>()) : DateTime.MinValue,
                    Details = hsNode.Properties.ContainsKey("details") ? hsNode.Properties["details"].As<string>() : string.Empty,
                    Immutable = hsNode.Properties.ContainsKey("immutable") && hsNode.Properties["immutable"].As<bool>(),
                    HealthStateSummary = hsNode.Properties.ContainsKey("healthStateSummary") ? hsNode.Properties["healthStateSummary"].As<string>() : string.Empty,
                    HealthRecommendation = hsNode.Properties.ContainsKey("healthRecommendation") ? hsNode.Properties["healthRecommendation"].As<string>() : string.Empty
                };
            }
            else
            {
                treatment.HealthSnapshot = null;
            }

            treatments.Add(treatment);
        }
    });

    return treatments;
}


    public async Task<Treatment> CreateTreatmentAsync(string patientId, string doctorId, Treatment dto)
{
    await using var session = _driver.AsyncSession();
    var query = @"
        MATCH (p:Patient {id: $patientId})
        MATCH (d:Doctor {id: $doctorId})
        CREATE (t:Treatment {
            id: apoc.create.uuid(), 
            type: $type, 
            date: $date, 
            doctorId: $doctorId,
            followUpAction: $followUpAction,
            patientId: $patientId
        })
        CREATE (p)-[:UNDERGOES]->(t)
        CREATE (d)-[:ISSUES]->(t)

        CREATE (hs:HealthSnapshot {
            id: apoc.create.uuid(),
            createdAt: datetime(),
            details: $followUpAction,
            immutable: true,
            healthStateSummary: $healthStateSummary,
            HealthRecommendation: $HealthRecommendation
        })
        CREATE (t)-[:GENERATES]->(hs)
        RETURN t, hs
    ";

    var parameters = new
    {
        patientId = patientId,
        doctorId = doctorId,
        type = dto.Type,
        followUpAction = dto.FollowUpAction,
        date = DateTime.UtcNow,
        healthStateSummary = dto.HealthSnapshot?.HealthStateSummary ?? string.Empty,
        HealthRecommendation = dto.HealthSnapshot?.HealthRecommendation ?? string.Empty
    };

    Treatment createdTreatment = null;

    await session.ExecuteWriteAsync(async tx =>
    {
        var cursor = await tx.RunAsync(query, parameters);
        var record = await cursor.SingleAsync();

        var tNode = record["t"].As<INode>();
        var hsNode = record.ContainsKey("hs") ? record["hs"].As<INode>() : null;

        createdTreatment = NodeMapper.MapTreatment(tNode, hsNode);
    });

    return createdTreatment;
}






    public async Task<AbstractTreatment> CreateTreatmentAsynchrone(AbstractTreatment treatment)
{
    using var session = _driver.AsyncSession();
    await session.ExecuteWriteAsync(async tx =>
    {
        var cypherQuery = @"
            CREATE (t:Treatment {
                id: $id,
                type: $type,
                createdAt: $createdAt,
                updatedAt: $updatedAt,
                active: $active,
                isabstract: $isabstract
            })
        ";

        var parameters = new
        {
            id = treatment.Id,
            type = treatment.Type,
            createdAt = treatment.CreatedAt.ToUniversalTime(),
            updatedAt = treatment.UpdatedAt.ToUniversalTime(),
            active = treatment.Active,
            isabstract = treatment.Isabstract
        };

        await tx.RunAsync(cypherQuery, parameters);
    });

    return treatment;
}



   


    //  Retrieve the latest health snapshot for a given patient.
    public async Task<HealthSnapshot> GetLatestHealthSnapshotByPatientIdAsync(string patientId)
    {
        HealthSnapshot snapshot = null;
        await using var session = _driver.AsyncSession();
        var query = @"
        MATCH (p:Patient {id: $patientId})-[:UNDERGOES]->(t:Treatment)
        MATCH (t)-[:GENERATES]->(hs:HealthSnapshot)
        RETURN hs
        ORDER BY hs.createdAt DESC LIMIT 1
    ";

        await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { patientId });
            if (await cursor.FetchAsync())
            {
                var hsNode = cursor.Current["hs"].As<INode>();
                snapshot = new HealthSnapshot
                {
                    Id = hsNode.Properties["id"].As<string>(),
                    CreatedAt = DateTime.Parse(hsNode.Properties["createdAt"].As<string>()),
                    Details = hsNode.Properties["details"].As<string>(),
                    Immutable = hsNode.Properties["immutable"].As<bool>(),
                    HealthStateSummary = hsNode.Properties.ContainsKey("healthStateSummary") 
                                    ? hsNode.Properties["healthStateSummary"].As<string>() 
                                    : string.Empty,
            HealthRecommendation = hsNode.Properties.ContainsKey("healthRecommendation") 
                                    ? hsNode.Properties["healthRecommendation"].As<string>() 
                                    : string.Empty

                };
            }
        });
        return snapshot;
    }


   


    //  Retrieve the full timeline of health snapshots for a given patient.
    public async Task<IEnumerable<HealthSnapshot>> GetHealthSnapshotTimelineByPatientIdAsync(string patientId)

    {
        var snapshots = new List<HealthSnapshot>();
        await using var session = _driver.AsyncSession();
        var query = @"
        MATCH (p:Patient {id: $patientId})-[:UNDERGOES]->(t:Treatment)
        MATCH (t)-[:GENERATES]->(hs:HealthSnapshot)
        RETURN hs
        ORDER BY hs.createdAt ASC
    ";

        await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { patientId });
            while (await cursor.FetchAsync())
            {
                var hsNode = cursor.Current["hs"].As<INode>();
                var hs = new HealthSnapshot
                {
                    Id = hsNode.Properties["id"].As<string>(),
                    CreatedAt = DateTime.Parse(hsNode.Properties["createdAt"].As<string>()),
                    Details = hsNode.Properties["details"].As<string>(),
                    Immutable = hsNode.Properties["immutable"].As<bool>(),


     
            HealthStateSummary = hsNode.Properties.ContainsKey("healthStateSummary") 
                                    ? hsNode.Properties["healthStateSummary"].As<string>() 
                                    : string.Empty,
            HealthRecommendation = hsNode.Properties.ContainsKey("healthRecommendation") 
                                    ? hsNode.Properties["healthRecommendation"].As<string>() 
                                    : string.Empty

            
                };
                snapshots.Add(hs);
            }
        });

        return snapshots;
    }

    protected override Treatment Map(INode node)
    {
        throw new NotImplementedException();
    }

    protected override IDictionary<string, object> CreateParameters(Treatment entity)
    {
        throw new NotImplementedException();
    }

   
}

