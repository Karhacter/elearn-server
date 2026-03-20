# Changelog

All notable backend changes for `elearn-server` should be recorded here.

## 2026-03-20

### Added
- AI-backed roadmap course recommendation flow.
- New endpoint `POST /api/Recommendation/roadmap-courses`.
- Request model for roadmap-based recommendation input.
- Response DTOs for ordered course recommendation output.
- `OpenAI` configuration section for API key, base URL, and model.

### Changed
- Registered recommendation services and `HttpClient` in application startup.
- Added fallback ranking so recommendations still work without an AI key.
- Improved AI failure reporting so the API now exposes clearer reasons such as quota or authentication issues.
- Switched the recommendation provider from OpenAI API to local Ollama.

### Removed
- OpenAI API key based configuration from appsettings.

### Notes
- Current implementation accepts `RoadmapName` from the client because the project does not yet have a dedicated `Roadmap` entity/table.
- If `OPENAI_API_KEY` or `OpenAI:ApiKey` is missing, the system falls back to heuristic ordering.
- `dotnet build` succeeded after the change. Existing nullability warnings remain in unrelated parts of the project.

### Main Files
- `Controllers/RecommendationController.cs`
- `Services/CourseRecommendationService.cs`
- `Services/ICourseRecommendationService.cs`
- `Services/OpenAIOptions.cs`
- `Request/CourseRecommendationRequest.cs`
- `DTO/CourseRecommendationDTO.cs`
- `Program.cs`
- `appsettings.json`
