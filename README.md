# Interview Technical Test

This repository contains a full-stack technical test designed for interview candidates. It consists of a modern web application with a React frontend, a .NET backend, and a containerized MySQL database.

## Technologies Used

### Frontend

- **Framework:** [React](https://react.dev/) (v19)
- **Language:** [TypeScript](https://www.typescriptlang.org/)
- **Build Tool:** [Rsbuild](https://rsbuild.dev/)
- **UI Library:** [Material UI (MUI)](https://mui.com/)
- **Styling:** SCSS / SASS modules

### Backend

- **Framework:** [.NET](https://dotnet.microsoft.com/) (C#)
- **ORM:** Entity Framework Core
- **Database Provider:** Pomelo.EntityFrameworkCore.MySql

### Database

- **Database:** MySQL 8.0
- **Containerization:** Docker

## Recommended IDEs

You can use any IDE that supports React/TypeScript and C# development. The recommended options are:

- **[Visual Studio Code](https://code.visualstudio.com/)** (Recommended for full-stack)
- **[JetBrains Rider](https://www.jetbrains.com/rider/)** (Excellent for .NET backend)
- **[Visual Studio](https://visualstudio.microsoft.com/)**

## Getting Started

Follow these steps to get the application running locally.

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js](https://nodejs.org/) (LTS version recommended)
- [.NET SDK](https://dotnet.microsoft.com/download)

### 1. Start the Database

The database is configured via Docker Compose. Run the following command from the root of the repository to start the MySQL container:

```bash
docker compose up -d
```

or

```bash
docker-compose up -d
```

This will start a MySQL instance on port `3333` with the database `interview_db`.

### 2. Start the Backend API

Navigate to the backend directory and start the .NET application:

```bash
cd src/backend
dotnet restore
dotnet run
```

The backend API should now be running (check the console output for the port, should be `http://localhost:5008`).

### 3. Start the Frontend Application

Open a new terminal, navigate to the frontend directory, install dependencies, and start the development server:

```bash
cd src/frontend
npm install
npm run start
```

The command `npm run start` runs `rsbuild dev --open`, which will start the frontend and automatically open it in your default browser (usually at `http://localhost:3101`).

### Database architecture

![database diagram](image.png)

## Agentic RAG interview (branch `agentic-rag`)

This branch adds a small retrieval-augmented question-answering feature on top of the app, for a 45-minute live interview with an AI coding agent. Candidates start with [CANDIDATE.md](CANDIDATE.md).

- `corpus/` - 12 project documents (one contains a prompt-injection attempt)
- `src/backend/Rag/` - chunker, BM25 retriever, LLM client, `RagService`
- `src/eval` + `eval/questions.json` - score card (`dotnet run --project src/eval`)
- `src/tests` - unit tests (`dotnet test src/tests`)
- `./start.sh` - starts MySQL and the API in one command

The default LLM mode is `offline` (no network, no key). Set `Llm__Mode=http` with `Llm__BaseUrl`, `Llm__ApiKey` and `Llm__Model` to use any OpenAI-compatible endpoint.
