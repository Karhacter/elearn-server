# Worklog Guide

Use this file as a simple project log after each meaningful code change.

## Entry Template

### Date
- YYYY-MM-DD

### Requested
- What the user or teammate asked for.

### Implemented
- What was added, edited, removed, or refactored.

### Why
- Why the change was needed.

### Impact
- Which APIs, screens, models, or flows are affected.

### Verification
- Build/test commands that were run and their result.

### Follow-up
- Any next steps, known limitations, or risks.

## Example

### Date
- 2026-03-20

### Requested
- Add AI recommendation for which courses a learner should study first after choosing a roadmap.

### Implemented
- Added a recommendation endpoint and a dedicated service.
- Integrated OpenAI-based ranking with a local fallback strategy.
- Added request/response models for roadmap recommendations.

### Why
- The project needed smarter course ordering instead of returning raw course lists.

### Impact
- New backend API available at `POST /api/Recommendation/roadmap-courses`.
- Startup configuration now includes recommendation service registration.
- `appsettings.json` now includes an `OpenAI` section.

### Verification
- Ran `dotnet build`.
- Build passed with warnings only.

### Follow-up
- Connect the new endpoint to the frontend roadmap selection flow.
- Consider adding a dedicated roadmap entity if roadmap data becomes persistent.
- Improve API feedback when OpenAI rejects requests because of quota, auth, or rate limits.
- Install Ollama locally and pull the configured model before testing AI mode.
