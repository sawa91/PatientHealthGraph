using HealthcareGraphAPI.Models;
using Neo4j.Driver;

public static class NodeMapper
{
    /// <summary>
    /// Helper method to map a Neo4j INode to a Patient.
    /// </summary>
    public static Patient MapPatient(INode node)
    {
        return new Patient
        {
            Id = node.Properties["id"].As<string>(),
            FirstName = node.Properties["firstName"].As<string>(),
            LastName = node.Properties["lastName"].As<string>(),
            DateOfBirth = DateTime.Parse(node.Properties["dateOfBirth"].As<string>()),
            Gender = Enum.Parse<Gender>(node.Properties["gender"].As<string>()),
            HealthCardNumber = node.Properties["healthCardNumber"].As<string>(),
            Active = node.Properties["active"].As<bool>(),
            CreatedAt = DateTime.Parse(node.Properties["createdAt"].As<string>()),
            UpdatedAt = DateTime.Parse(node.Properties["updatedAt"].As<string>())
        };
    }

    public static Doctor MapDoctor(INode node)
    {
        return new Doctor
        {
            Id = node.Properties["id"].As<string>(),
            FirstName = node.Properties["firstName"].As<string>(),
            LastName = node.Properties["lastName"].As<string>(),
            StartYear = node.Properties["startYear"].As<string>(),
            Gender = Enum.Parse<Gender>(node.Properties["gender"].As<string>()),
            LicenseNumber = node.Properties["licenseNumber"].As<string>(),
            Active = bool.TryParse(node.Properties["active"].As<string>(), out var isActive) ? isActive : true,
            CreatedAt = DateTime.Parse(node.Properties["createdAt"].As<string>()),
            UpdatedAt = DateTime.Parse(node.Properties["updatedAt"].As<string>())
        };
    }

    public static Treatment MapTreatment(IRecord record)
{
    // Map the treatment node.
    var tNode = record["t"].As<INode>();
    var treatment = new Treatment
    {
        Id = tNode.Properties.ContainsKey("id") ? tNode.Properties["id"].As<string>() : string.Empty,
        Type = tNode.Properties.ContainsKey("type") ? tNode.Properties["type"].As<string>() : string.Empty,
        Active = tNode.Properties.ContainsKey("active") &&
                 bool.TryParse(tNode.Properties["active"].As<string>(), out var isActive) ? isActive : true,
        CreatedAt = tNode.Properties.ContainsKey("createdAt")
                        ? DateTime.Parse(tNode.Properties["createdAt"].As<string>())
                        : DateTime.MinValue,
        UpdatedAt = tNode.Properties.ContainsKey("updatedAt")
                        ? DateTime.Parse(tNode.Properties["updatedAt"].As<string>())
                        : DateTime.MinValue,
        Date = tNode.Properties.ContainsKey("date")
                        ? DateTime.Parse(tNode.Properties["date"].As<string>())
                        : DateTime.MinValue,
        // Directly map followUpAction, now a string field in Treatment.
        FollowUpAction = tNode.Properties.ContainsKey("followUpAction")
                        ? tNode.Properties["followUpAction"].As<string>()
                        : string.Empty,
        HealthSnapshot = new HealthSnapshot
        {
            HealthStateSummary = tNode.Properties.ContainsKey("healthStateSummary")
                                    ? tNode.Properties["healthStateSummary"].As<string>()
                                    : string.Empty,
            HealthRecommendation = tNode.Properties.ContainsKey("healthRecommendation")
                                    ? tNode.Properties["healthRecommendation"].As<string>()
                                    : string.Empty
        },
    };

    return treatment;
}

        
    public static Treatment MapTreatment(INode node)
    {
        return new Treatment
        {
            Id = node.Properties.ContainsKey("id") ? node.Properties["id"].As<string>() : string.Empty,
            Type = node.Properties.ContainsKey("type") ? node.Properties["type"].As<string>() : string.Empty,
            Active = node.Properties.ContainsKey("active") &&
                     bool.TryParse(node.Properties["active"].As<string>(), out var isActive) ? isActive : true,
            CreatedAt = node.Properties.ContainsKey("createdAt")
                            ? DateTime.Parse(node.Properties["createdAt"].As<string>())
                            : DateTime.MinValue,
            UpdatedAt = node.Properties.ContainsKey("updatedAt")
                            ? DateTime.Parse(node.Properties["updatedAt"].As<string>())
                            : DateTime.MinValue,
            Isabstract = node.Properties.ContainsKey("isabstract") &&
                     bool.TryParse(node.Properties["isabstract"].As<string>(), out var isabstract) ? isabstract : false,
            HealthSnapshot = new HealthSnapshot
        {
            HealthStateSummary = node.Properties.ContainsKey("healthStateSummary")
                                    ? node.Properties["healthStateSummary"].As<string>()
                                    : string.Empty,
            HealthRecommendation = node.Properties.ContainsKey("healthRecommendation")
                                    ? node.Properties["healthRecommendation"].As<string>()
                                    : string.Empty
        },


        };
    }
    
    public static AbstractTreatment MapAbstractTreatment(INode node)
    {
        return new AbstractTreatment
        {
            Id = node.Properties.ContainsKey("id") ? node.Properties["id"].As<string>() : string.Empty,
            Type = node.Properties.ContainsKey("type") ? node.Properties["type"].As<string>() : string.Empty,
            Active = node.Properties.ContainsKey("active") &&
                     bool.TryParse(node.Properties["active"].As<string>(), out var isActive) ? isActive : true,
            CreatedAt = node.Properties.ContainsKey("createdAt")
                            ? DateTime.Parse(node.Properties["createdAt"].As<string>())
                            : DateTime.MinValue,
            UpdatedAt = node.Properties.ContainsKey("updatedAt")
                            ? DateTime.Parse(node.Properties["updatedAt"].As<string>())
                            : DateTime.MinValue,
            Isabstract= node.Properties.ContainsKey("isabstract") &&
                     bool.TryParse(node.Properties["isabstract"].As<string>(), out var isabstract) && isabstract,
         

        };
    }

public static Treatment MapTreatment(INode treatmentNode, INode healthSnapshotNode)
{
    var treatment = new Treatment
    {
        Id = treatmentNode.Properties.ContainsKey("id") ? treatmentNode.Properties["id"].As<string>() : string.Empty,
        Type = treatmentNode.Properties.ContainsKey("type") ? treatmentNode.Properties["type"].As<string>() : string.Empty,
        Active = treatmentNode.Properties.ContainsKey("active") &&
                 bool.TryParse(treatmentNode.Properties["active"].As<string>(), out var isActive) ? isActive : true,
        CreatedAt = treatmentNode.Properties.ContainsKey("createdAt")
                        ? DateTime.Parse(treatmentNode.Properties["createdAt"].As<string>())
                        : DateTime.MinValue,
        UpdatedAt = treatmentNode.Properties.ContainsKey("updatedAt")
                        ? DateTime.Parse(treatmentNode.Properties["updatedAt"].As<string>())
                        : DateTime.MinValue,
        FollowUpAction = treatmentNode.Properties.ContainsKey("followUpAction") ? treatmentNode.Properties["followUpAction"].As<string>() : string.Empty,
        Isabstract = treatmentNode.Properties.ContainsKey("isabstract") &&
                     bool.TryParse(treatmentNode.Properties["isabstract"].As<string>(), out var isabstract) ? isabstract : false,
        HealthSnapshot = healthSnapshotNode != null ? new HealthSnapshot
        {
            Id = healthSnapshotNode.Properties.ContainsKey("id") ? healthSnapshotNode.Properties["id"].As<string>() : string.Empty,
            CreatedAt = healthSnapshotNode.Properties.ContainsKey("createdAt") ? DateTime.Parse(healthSnapshotNode.Properties["createdAt"].As<string>()) : DateTime.MinValue,
            Details = healthSnapshotNode.Properties.ContainsKey("details") ? healthSnapshotNode.Properties["details"].As<string>() : string.Empty,
            Immutable = healthSnapshotNode.Properties.ContainsKey("immutable") && healthSnapshotNode.Properties["immutable"].As<bool>(),
            HealthStateSummary = healthSnapshotNode.Properties.ContainsKey("healthStateSummary") ? healthSnapshotNode.Properties["healthStateSummary"].As<string>() : string.Empty,
            HealthRecommendation = healthSnapshotNode.Properties.ContainsKey("HealthRecommendation") ? healthSnapshotNode.Properties["HealthRecommendation"].As<string>() : string.Empty
        } : null
    };

    return treatment;
}

 


}
