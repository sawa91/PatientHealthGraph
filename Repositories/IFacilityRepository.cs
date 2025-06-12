using HealthcareGraphAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Repositories
{
    public interface IFacilityRepository :IRepository<Facility>
    {
        public Task<Facility> GetFacilityByIdAsync(string id);
    }
}
