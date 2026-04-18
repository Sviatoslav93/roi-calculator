# ROI Calculator

This app allows you to build and publish your ROI form. Then your clients can use it.

## Architecture

```plaintext
├── web/          Vue 3 + Vite + TypeScript (frontend)
└── backend/      .NET 10 (backend)
    ├── RoiCalculator.Api             Minimal API · MediatR · Vertical Slices
    ├── RoiCalculator.Core            Domain layer (DDD) · Zero dependencies
    └── RoiCalculator.Infrastructure  EF Core · PostgreSQL · Repositories
```

## Prerequisites

- **Node.js** ≥ 20
- **.NET** 10 SDK
- **PostgreSQL** (local or remote)

## Getting Started

### Frontend

```bash
cd web
npm install
npm run dev
```

### Backend

```bash
cd backend
dotnet run --project RoiCalculator.Api
```
