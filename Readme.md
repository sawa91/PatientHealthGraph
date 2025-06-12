

# Patient Healthcare Graph System

## Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Overview
This project is a **starter framework** for a healthcare management system. The main goal is to provide a modular, scalable starting point that integrates significant components such as a Neo4j database for complex relationships and ChatGPT-4 (using a Retrieval Augmented Generation approach) for dynamic clinical insights. 
The Healthcare Management System is designed to manage doctors, patients, facilities, and treatments. This project implements best practices by leveraging a layered architecture that separates responsibilities into controllers, services, repositories, models, and utilities. 

**Note:** This code base is meant to be further refined with detailed business logic, constraints, and input validation as the project evolves.

## Architecture
The architecture is divided into multiple layers:
  
- **Controllers**  
  Entry point of the application that handles HTTP requests. For example:  
  - TreatmentController  
  - DoctorController  
  - PatientController  
  - FacilityController
  
- **Services**  
  Business logic resides in the service layer. Services coordinate data, perform validation, and call the necessary repositories. Key services include:  
  - TreatmentService  
  - DoctorService  
  - PatientService  
  - FacilityService  
  - ChatGptHealthInsightsService (LLM integration for generating clinical insights)
  
- **Repositories**  
  The repository layer is responsible for data access. It interacts with the Neo4j database to persist and retrieve entities.  
  - TreatmentRepository  
  - DoctorRepository  
  - PatientRepository  
  - FacilityRepository
  
- **Models and DTOs**  
  Models represent the domain entities (e.g., Treatment, Doctor, Patient). Data Transfer Objects (DTOs) isolate the API contract, ensuring input validation and transformation.
  
- **Utilities**  
  Helper methods, mappers, and logging utilities are organized to support the core functionalities.
  
- **Neo4j Database**  
  The database is used to model complex relationships (e.g., UNDERGOES, TREATED_BY,SPECIALIZED_IN_TREATMENT, WORKS_AT, TREATED_BY) and store immutable snapshots of patient treatments.

View the architecture diagram for an overview of the structure and communication between layers.

## Features
- **Immutable Treatment Snapshots:** Capture patient health changes over time, including timestamps and enriched clinical insights.
- **LLM Integration:** Uses ChatGPT‑4 to generate a clinical summary and treatment recommendation with a RAG approach.
- **Comprehensive Domain Modeling:** Supports doctors, patients, and facilities with clearly defined relationships.
- **Swagger/OpenAPI:** Easily explore and test API endpoints.
- **Neo4j Integration:** Efficient handling of complex and interconnected healthcare data.

## Technology Stack
- **Backend Language:** C#
- **Framework:** ASP.NET Core
- **Database:** Neo4j
- **LLM Integration:** ChatGPT‑4 via OpenAI API
- **Documentation:** Swagger/OpenAPI
- **Dependency Injection:** Built-in .NET DI container

## Getting Started
To run the project locally, follow these steps:

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/HealthcareGraphAPI.git
   cd HealthcareGraphAPI
   ```

2. **Install Dependencies:**
   Restore NuGet packages using Visual Studio or from the command line:
   ```bash
   dotnet restore
   ```

3. **Configure the Application:**
   - Update the *appsettings.json* with the necessary configuration details for Neo4j and OpenAI:
     ```json
     {
       "OpenAI": {
         "ApiKey": "sk-XXXXXXXXXXXXXXXXXXXXXXXXXXXX"
       },
       "Neo4j": {
         "Uri": "bolt://your-neo4j-uri:port",
         "Username": "your-username",
         "Password": "your-password"
       },
       "Logging": {
         "LogLevel": {
           "Default": "Information",
           "Microsoft": "Warning"
         }
       },
       "AllowedHosts": "*"
     }
     ```

4. **Run the Application:**
   From the command line or Visual Studio:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:5001` (or the specified port).

## Configuration
- **OpenAI API Key:**  
  The key is stored in the `appsettings.json` under the **OpenAI** section.  
- **Neo4j Connection:**  
  The connection details are also defined in `appsettings.json` under the **Neo4j** section.
- **Dependency Injection & Services:**  
  The `Program.cs` file registers controllers, services (including ChatGptHealthInsightsService), repositories, and the Neo4j driver.

## API Documentation
Swagger is integrated into the project. After running the application, navigate to the specified port.

This interface allows you to test the endpoints for treatments, patients, doctors, and facilities.

