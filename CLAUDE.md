# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

Database (MySQL 8 must be running; needs the `mysql` CLI on PATH):
```bash
bash db/reset.sh            # or ./db/reset.ps1 on Windows; drops and reloads `learnhub`, prints row counts
```

Backend (.NET 8). Run from `backend/`:
```bash
cd LearnHub.Api && dotnet run --urls http://localhost:5080
dotnet test                                                        # all tests, SQLite in-memory, no MySQL needed
dotnet test --filter "FullyQualifiedName~ShopTests"                # one test class
dotnet test --filter "FullyQualifiedName~ShopTests.Checkout_Works" # one test (replace with a real name)
```

Frontend (Vite, port 5173, proxies `/api` to 5080). Run from `frontend/`:
```bash
npm install
npm run dev
npm run build               # no test or lint script exists
```

MCP server (read-only DB tools for Claude Code, wired via `.mcp.json`). Run from `mcp-server/`:
```bash
npm install
npm run build               # tsc -> dist/; .mcp.json points at dist/index.js, so build before use
```

No linter or formatter is configured in any part of the repo.

Hooks: `.claude/settings.json` runs `.claude/hooks/post-commit-claude-md.sh` after each `git commit` to prompt a CLAUDE.md review. Versioned git hooks live in `.githooks/`; enable with `bash .githooks/install.sh`.

## Configuration

- The API connection string comes from `LEARNHUB_DB` if set, otherwise `ConnectionStrings:LearnHub` in `backend/LearnHub.Api/appsettings.json`. Both currently use the password `LearnHub!23`.
- `LEARNHUB_NOW` (ISO timestamp) pins the app clock via `SystemClock` (`Services/Infrastructure.cs`). Sample data ends 2026-10-01, so set it when running the app to keep deadlines and refund windows meaningful.
- The MCP server reads `LEARNHUB_DB_HOST/PORT/USER/PASSWORD` from the process env or the root `.env` (gitignored; template in `.env.example`). Its `.env.example` password (`change-me`) differs from the appsettings password, so check which one your DB uses.
- `db/reset.sh` takes the same `LEARNHUB_DB_*` variables.

## Architecture

**Backend is a Minimal API with a thin endpoint layer over services.** `Program.cs` registers every service as scoped and maps endpoints from `Endpoints/` (one file per feature area). Endpoints inject services directly and do no business logic. Services live in `Services/` and share one `LearnHubDbContext` (`Data/`, EF Core with Pomelo MySQL, target MySQL 8.4).

**Services return `(Result?, string? Error)` tuples instead of throwing.** Endpoints turn a null result into `BadRequest(new { error })`. Follow this pattern for new features. Error text is user-facing and the frontend shows it as-is.

**Identity is a student name, not a login.** Every student-scoped call passes `studentName` in the query string or body. `StudentDirectory` (`Services/Infrastructure.cs`) maps the name to a `Student` row, creating one on first use with a generated email. There is no auth. Don't add auth assumptions to new endpoints without asking.

**Time comes only from `IClock`.** Never call `DateTime.UtcNow` in services; use the injected clock. Tests replace it with `FakeClock` (`TestEnv.cs`, `CustomWebApplicationFactory.cs`), which is why date-dependent tests are deterministic.

**Shop rules are concentrated in `ShopService`.** Bundle (15% off every line at 3+ paid courses), then coupon (applied after the bundle), then checkout. Checkout enrolls the student in each course and marks the coupon used. Refunds are allowed within 7 days of the order and only if progress is under 30% on every course in the order. A refund removes enrollments and lesson progress, returns the coupon use, and records a `Refund`. Instructor revenue is 70% of net (`ShopService.GetInstructorRevenue`). The payout report is different: `Reports/PayoutReportBuilder` applies 70% to gross (`row.Gross * 0.7m`), and `InstructorService` has its own `ShareRate`. Check which base a figure uses before comparing numbers across these views; the MCP `instructor_payouts` tool uses net.

**Two copies of the data-access logic exist.** `CustomWebApplicationFactory` runs the real API on SQLite for integration tests, while `TestEnv` builds the services directly for unit-style tests. The EF model must work on both MySQL and SQLite, so avoid MySQL-only SQL in `LearnHubDbContext` and queries.

**The MCP server (`mcp-server/src/`) is a separate read-only path to the same MySQL database.** It doesn't call the API. `db.ts` runs every query in a `READ ONLY` transaction. `index.ts` registers tools such as `run_query` (SELECT/WITH only), `get_student`, `list_refunds`, and `instructor_payouts`. Those tools re-implement some business rules in SQL, so they can drift from `ShopService`. Check both if you change pricing or refund logic.

**Frontend is React + Vite, with no state library.** `src/api.js` is the only place that calls the backend (a `request` helper throws on non-2xx using the `error` field). Pages live in `src/pages/` and routes are declared in `src/App.jsx`. The student name is stored in `localStorage` and read by each page. Shared UI is in `src/components/`.

**Data model and seed.** `db/schema.sql` defines the tables and `db/seed.sql` the sample data. Row counts are documented in `README.md` and should match after a reset. Changing the schema means updating `schema.sql`, the EF entities in `Models/Entities.cs`, `LearnHubDbContext`, and the test seed in `TestData.cs`.

**Exercises.** `EXERCISES.md` contains investigation tasks that use the data and MCP tools. It's course material, not code.
