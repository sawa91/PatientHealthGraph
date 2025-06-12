using HealthcareGraphAPI.Models;

public class Facility :BaseEntity
{


    /// <summary>
    /// The name of the facility.
    /// </summary>
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// The type of the facility (e.g., Hospital, Clinic).
    /// </summary>
    public FacilityType Type { get; set; }

    /// <summary>
    /// The overall capacity of the facility.
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// A list of services offered by the facility.
    /// </summary>
    public List<ServiceType> ServicesOffered { get; set; } = new List<ServiceType>();

    /// <summary>
    /// A list of contact details associated with the facility.
    /// </summary>
    public List<ContactInfo> Contacts { get; set; } = new List<ContactInfo>();
}


