# LearnHub

LearnHub is a small e-learning app. Students browse courses, read lessons and enroll.
Data lives in memory only. It resets when the backend stops.

## How to run

1. Backend: `cd backend/LearnHub.Api && dotnet run` (http://localhost:5080)
2. Frontend: `cd frontend && npm install && npm run dev` (http://localhost:5173)
3. Check backend build: `cd backend && dotnet build`
4. Run backend tests: `cd backend && dotnet test`
5. Check frontend build: `cd frontend && npm run build`

## Folder map

- `backend/LearnHub.Api/Models` - data classes (Course, Lesson, Enrollment, QuizQuestion, LessonProgress, Bookmark)
- `backend/LearnHub.Api/Data/InMemoryStore.cs` - all data and the business logic (enroll, quiz scoring, progress, bookmarks, certificates, add course), one singleton
- `backend/LearnHub.Api/Endpoints` - one static class per area
- `backend/LearnHub.Api/Program.cs` - registers the store and endpoints
- `backend/LearnHub.Tests` - xUnit tests for the store logic (`cd backend && dotnet test`)
- `frontend/src/pages` - one file per screen
- `frontend/src/components` - small shared pieces
- `frontend/src/api.js` - every backend call
- `frontend/src/index.css` - all styles
- `.claude/skills` - step-by-step guides (add-api-endpoint, add-react-page, seed-sample-data, write-api-tests)
- `.claude/agents` - specialist helpers: `backend-dev` (backend only), `frontend-dev` (frontend only), `test-writer`, `code-reviewer`, `ui-designer`
- `.claude/commands` - saved prompts: `/run-app`, `/review-changes`, `/add-test`
- `.claude/hooks` and `.claude/settings.json` - automatic checks: build after a `.cs` edit, block edits to secret files, run tests when Claude stops
- `.mcp.json` - project MCP servers (Playwright browser). See `MCP.md`
- `AGENT-MAP.md` - how the main agent hands work to the agents

## Coding rules

### Backend (.NET 8 Minimal API)
1. Endpoints live in `Endpoints/XxxEndpoints.cs` as `public static class XxxEndpoints`.
2. Each class has `MapXxxEndpoints(this WebApplication app)`. Register it in `Program.cs`.
3. Routes start with `/api/`. Take `InMemoryStore store` as a handler parameter.
4. Return anonymous objects, not the raw model, when the shape differs.
5. Errors: `Results.BadRequest(new { error = "..." })` for bad input,
   `Results.NotFound()` for a missing item, `Results.Conflict(new { error = "..." })` for duplicates.
6. Compare names with `StringComparison.OrdinalIgnoreCase`.
7. Put logic in `InMemoryStore` methods, not in endpoint lambdas, so tests can reach it.

### Store
1. Models are plain classes in `Models/` with default values (`= ""`, `= new()`).
2. All data goes in `InMemoryStore` as a `List<T>`. No database, no other static data.
3. Ids are unique ints. Never reuse an id.

### Frontend (React + Vite, plain JS)
1. One page per file in `src/pages`, default export a function component.
2. Routes are in `App.jsx`. Nav links are in `components/NavBar.jsx`.
3. Every backend call is a function in `src/api.js` that uses `request()`.
4. Pages show errors with `<p className="message error">`.
5. Reuse existing CSS classes (`panel`, `grid`, `row`, `muted`, `tag`) before adding new ones.

### General
1. Do not add new libraries or packages without asking first.
2. Keep code small and simple. Match the style of nearby files.
3. After backend changes run `dotnet build` and `dotnet test`. After frontend changes run `npm run build`.

### Working with agents
1. Plan first. Agree on an API contract (routes and shapes) before backend and frontend work starts.
2. `backend-dev` edits only `backend/LearnHub.Api`. `frontend-dev` edits only `frontend/`. `test-writer` edits only `backend/LearnHub.Tests`. `code-reviewer` edits nothing.
3. Hooks run `dotnet build` after `.cs` edits and `dotnet test` when Claude stops. Do not edit `.env` or `appsettings.*.json` (a hook blocks it).
