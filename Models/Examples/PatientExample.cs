using Swashbuckle.AspNetCore.Filters;
using HealthcareGraphAPI.Models.DTOs;

namespace HealthcareGraphAPI.Models.Examples
{
    public class PatientExample : IExamplesProvider<CreatePatientDto>
    {
        public CreatePatientDto GetExamples()
        {
            return new CreatePatientDto
            {
                FirstName = "John",
                LastName = "Doe",
                Gender = Gender.Male,
                HealthCardNumber = "123-400-008",
                DateOfBirth="2005-10-10",
                
            };
        }
    }
}