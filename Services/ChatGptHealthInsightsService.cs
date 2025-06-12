using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HealthcareGraphAPI;
using HealthcareGraphAPI.Models;
using Microsoft.Extensions.Configuration;

public class OpenAIResponse
{
    public Choice[] Choices { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
}

namespace HealthcareGraphAPI.Services
{
    public class ChatGptHealthInsightsService : IHealthInsightsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ChatGptHealthInsightsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"]; //API KEY
        }

        public async Task<(string Summary, string Recommendation)> GenerateInsightsAsync(string patientId, Treatment treatment, CreateTreatmentDTO dto)
        {
            // Define the system message to guide the model
            var systemMessage = "You are an expert in clinical summarization. Generate a concise summary and a treatment recommendation based on the provided information.";

            // User Prompt construction
            var userPrompt = $"Patient ID: {patientId}\n" +
                             $"Treatment ID: {treatment.Id}\n" +
                             $"Date du traitement: {treatment.CreatedAt:u}\n" +
                             $"FollowUpAction: {dto.FollowUpAction}\n" +
                             "Please generate the result in the following format:\n" +
                            "Summary: <Your summary here>\n" +
                            "Recommendation: <Your recommendation here>";

            // Créer l'objet de requête pour l'API ChatGPT-4
            var requestBody = new
            {
                model = "gpt-4",
                messages = new object[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.7
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Ajout de l’en-tête d'authorization avec votre clé API
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // Appel de l'API ChatGPT-4
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new Exception($"L'appel à l'API OpenAI a échoué avec le statut {response.StatusCode}: {errorMsg}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var openAiResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (openAiResponse == null || openAiResponse.Choices == null || openAiResponse.Choices.Length == 0)
            {
                throw new Exception("Réponse invalide de l'API OpenAI.");
            }

            // Récupérer le texte généré par l'API
            var generatedText = openAiResponse.Choices[0].Message.Content;

            // Parser le texte généré pour extraire le résumé et la recommandation
            string summary = string.Empty;
            string recommendation = string.Empty;
            var summaryMarker = "Summary:";
            var recMarker = "Recommendation:";

            int summaryIndex = generatedText.IndexOf(summaryMarker, StringComparison.InvariantCultureIgnoreCase);
            int recIndex = generatedText.IndexOf(recMarker, StringComparison.InvariantCultureIgnoreCase);

            if (summaryIndex != -1 && recIndex != -1)
            {
                summary = generatedText.Substring(summaryIndex + summaryMarker.Length, recIndex - (summaryIndex + summaryMarker.Length)).Trim();
                recommendation = generatedText.Substring(recIndex + recMarker.Length).Trim();
            }
            else
            {
                // Si les marqueurs ne sont pas trouvés, renvoyer le texte complet comme résumé.
                summary = generatedText;
            }

            return (summary, recommendation);
        }
    }

   
}
