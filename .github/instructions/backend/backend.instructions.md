---
name: Backend Code Style Guidelines
description: This file describes the code style guidelines for the backend project.
applyTo: "backend/**"
version: 1.0.0
---

# Code Styles
- Use records for commands, queries, and DTOs — they communicate immutability and value semantics.
- Use primary constructors for dependency injection in handlers and services.
- No inline comments or XML doc comments — method and variable names should be self-documenting. If a comment seems necessary, it's a signal to refactor the code instead.
- Use Async for I/O-bound operations, but omit the `Async` suffix — the project follows an async-by-default convention where the suffix is redundant noise.
```csharp
// Preferred
public async Task<User> GetUser(int id) { ... }
// Avoid
public async Task<User> GetUserAsync(int id) { ... }
```
