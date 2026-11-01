# AiBot

AiBot is an ASP.NET Core application built for AI-powered chat or bot functionality. The project combines Identity, EF Core, and a custom AI service using HTTP calls to an external LLM provider.

## Overview

This project appears to be a bot application that allows users to interact with an AI system through a web interface. The backend uses ASP.NET Core MVC and connects to an external language model API through an HTTP client.

## Tech Stack

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- HttpClient for LLM integration
- HTML/CSS/JavaScript support for views

## Project Structure

```text
AiBot/
├── AiBot/
├── Application/
├── ChatBot/
├── Domain/
├── Infrastructure/
├── AiBot.slnx
├── .gitignore
├── .gitattributes
└── README.md
```

## Main Features

- user authentication and authorization
- AI chat service integration
- database-backed persistence
- external LLM configuration through appsettings
- MVC-based setup for web pages

## Backend Setup

1. Update the database connection string in `appsettings.json`.
2. Set the LLM configuration values, especially:
   - `Llm:BaseUrl`
   - `Llm:ApiKey`
3. Restore dependencies:

```bash
dotnet restore
```

4. Run the app:

```bash
dotnet run --project AiBot/AiBot.csproj
```

## Configuration Note

The `ChatAiService` is registered with an HttpClient using a base URL and API key from configuration. The app expects the LLM settings to be present before running.

## Notes

This project is a good example of combining standard ASP.NET Core infrastructure with AI service access. It uses a common layered design with application, domain, and infrastructure layers while integrating with an external AI-based API.

## Future Improvements

- better chat history storage
- admin and user role separation
- cleaner BOT UI
- API documentation
- more advanced prompt handling
- deployment setup
