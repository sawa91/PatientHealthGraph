using Swashbuckle.AspNetCore.Filters;
using HealthcareGraphAPI.Models.DTOs;

namespace HealthcareGraphAPI.Models.Examples
{
    public class FacilityExample : IExamplesProvider<CreateFacilityDto>
    {
        public CreateFacilityDto GetExamples()
        {
            return new CreateFacilityDto
            {
                Name = "St. Mary's Hospital",
                Type = "Hospital",
                Capacity = 200,
                ServicesOffered = new List<string>
                {
                    "Emergency",
                    "Radiology",
                    "Cardiology", 
                },
                Contacts = new List<ContactDto>
            {
                new ContactDto { Type = "Email", Value = "contact@stmaryshospital.org" },
                new ContactDto { Type = "Phone", Value = "+1234567890" }
            }
            };
        }
    } 
}