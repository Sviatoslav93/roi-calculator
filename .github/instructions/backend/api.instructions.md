---
name: Api Code Style Guidelines
description: This file describes the code style guidelines for the API project.
applyTo: "backend/RoiCalculator.Api/**"
version: 1.0.0
---

# Instructions
- Use ASP.NET Core Minimal APIs (endpoint handlers registered via app.MapGet/MapPost/etc. in Program.cs) rather than MVC controllers.
- RESTful API design principles should be followed for all API endpoints.
- All error responses must use ASP.NET Core's built-in ProblemDetails format (RFC 7807). Use TypedResults.Problem() or Results.ValidationProblem() as appropriate.
- API versioning is not required for this project. Do not add versioning segments (e.g. /v1/) to routes unless explicitly instructed.
- Register a global exception handler via app.UseExceptionHandler() in Program.cs that returns a ProblemDetails response. Individual handlers should not catch and swallow unhandled exceptions.
- Vertical slice architecture should be used. Each feature slice must contain all related files in a single folder under Features/<FeatureName>/, including: the endpoint registration class, request/response record types, the handler, and a validator class (required when the endpoint accepts a request body or query parameters, omit otherwise).
- Each feature slice must expose endpoint registration via a static extension method on IEndpointRouteBuilder, e.g. public static IEndpointRouteBuilder MapCalculateRoiEndpoint(this IEndpointRouteBuilder app), and be called from Program.cs.
- Apply authorization using .RequireAuthorization("PolicyName") fluently on the endpoint registration rather than using [Authorize] attributes, since Minimal APIs are attribute-hostile.
