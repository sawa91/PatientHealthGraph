namespace HealthcareGraphAPI.Models
{
    public class ContactInfo : BaseEntity
    {
        /// <summary>
        /// The type of contact information (e.g., Email, Phone).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The contact value (e.g., an email address or phone number).
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}