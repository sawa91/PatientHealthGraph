using System.Threading.Tasks;
using HealthcareGraphAPI.Models;
using HealthcareGraphAPI;

namespace HealthcareGraphAPI.Services
{
    public interface IHealthInsightsService
    {
        /// <summary>
        /// Génère un résumé de l'état de santé et une recommandation de traitement en se basant sur les informations du patient et du traitement.
        /// </summary>
        /// <param name="patientId">L'identifiant du patient.</param>
        /// <param name="treatment">L'objet Treatment en cours de création.</param>
        /// <param name="dto">Les informations complémentaires issues du DTO de création.</param>
        /// <returns>Un tuple contenant le résumé et la recommandation.</returns>
        Task<(string Summary, string Recommendation)> GenerateInsightsAsync(string patientId, Treatment treatment, CreateTreatmentDTO dto);
    }
}
