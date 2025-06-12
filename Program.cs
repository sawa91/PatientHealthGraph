using Neo4j.Driver;
using HealthcareGraphAPI.Repositories;
using HealthcareGraphAPI.Models.Examples;
using HealthcareGraphAPI.Services;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add controllers with JSON options (including enum as strings)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to use XML documentation (ensure the XML file is generated and path is correct)
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.EnableAnnotations(); // Enables Swagger annotations
    options.ExampleFilters();    // ✅ Enables `[SwaggerRequestExample]`
}); 

// Configure the Neo4j driver.
var neo4jUri = builder.Configuration.GetConnectionString("Neo4jConnection");
var neo4jUser = builder.Configuration["Neo4j:Username"];
var neo4jPassword = builder.Configuration["Neo4j:Password"];
var neo4jDriver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword));
builder.Services.AddSingleton<IDriver>(neo4jDriver);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never; // ✅ Ensures inherited properties are serialized
});


// Register repositories and services.
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
// Register HttpClient for ChatGptHealthInsightsService
builder.Services.AddHttpClient<ChatGptHealthInsightsService>();

// Register custom services.
builder.Services.AddScoped<IHealthInsightsService, ChatGptHealthInsightsService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();

builder.Services.AddSwaggerExamplesFromAssemblyOf<DoctorExample>(); // Registers example providers

// Configure Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Healthcare API",
        Version = "v1",
        Description = "API for managing treatments with ChatGPT integration"
    });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
