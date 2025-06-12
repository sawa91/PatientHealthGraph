using Swashbuckle.AspNetCore.Filters;
using HealthcareGraphAPI.Models.DTOs;

namespace HealthcareGraphAPI.Models.Examples
{
    public class DoctorExample : IExamplesProvider<CreateDoctorDto>
    {
        public CreateDoctorDto GetExamples()
        {
            return new CreateDoctorDto
            {
                FirstName = "John",
                LastName = "Doe",
                Specialization = "Cardiology",
                Gender = Gender.Male,
                LicenseNumber = "123-4008 (License number is unique to each Doctor)",
                StartYear="2005"
                
            };
        }
    }
}