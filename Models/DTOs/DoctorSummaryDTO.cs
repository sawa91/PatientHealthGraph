public class DoctorSummaryDto
{

    //Doctor data shown in Treatments.
    //we hide sensitive data: licenseNumber, and contact info if defined for the doctor.
    public string Id { get; set; }= string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}