# LearnHub (Day 5)

LearnHub is a small e-learning website. You can browse courses, read lessons, take quizzes, track progress, enroll, bookmark courses and earn certificates. Courses can be free or paid: students use a cart, coupons and a bundle discount, and can ask for a refund. Instructors can add courses and see their revenue.
It has a backend (C#, .NET 8) and a frontend (React). You run both at the same time.

## What you need installed

1. .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Node.js 20 or newer: https://nodejs.org
   (Node.js is also used by the MCP server, Playwright, through `npx`.)
3. VS Code: https://code.visualstudio.com
4. Claude Code extension for VS Code: https://docs.claude.com/en/docs/claude-code/ide-integrations

## How to run it

You need two terminals. Keep both open.

**Terminal 1 (backend)**

1. `cd backend/LearnHub.Api`
2. `dotnet run`

Wait until you see `Now listening on: http://localhost:5080`.

**Terminal 2 (frontend)**

1. `cd frontend`
2. `npm install`
3. `npm run dev`

Now open http://localhost:5173 in your browser.

## Run the tests

1. `cd backend`
2. `dotnet test`

All tests should pass. They check enrollment, search, quiz scoring, progress, bookmarks, certificates, adding courses and the whole shop (cart, coupons, bundles, orders, refunds, revenue).

## Check the whole shop

With the backend running, run `bash scripts/e2e-shop.sh` from the repo root. It walks the full buying trip and prints PASS or FAIL for each step.

Note: Claude Code hooks in this repo run `dotnet build` after C# edits and `dotnet test` when Claude finishes. This happens by itself.

## Check the backend alone

Open http://localhost:5080/api/courses in your browser. You should see a list of courses as JSON.

## Folder map

| Folder | What it is for |
| --- | --- |
| `backend/LearnHub.Api/Models` | The shapes of our data: Course, Lesson, Enrollment, QuizQuestion, LessonProgress, Bookmark, CartItem, Coupon, Order |
| `backend/LearnHub.Api/Data` | The in-memory store with the sample courses |
| `backend/LearnHub.Tests` | The tests (run with `dotnet test` from `backend/`) |
| `backend/LearnHub.Api/Endpoints` | The API routes, one file per area |
| `frontend/src/pages` | The screens of the website (courses, quiz, progress) |
| `frontend/src/components` | Small shared pieces (top bar, course card) |
| `frontend/src/api.js` | All calls from the website to the backend |
| `.claude/skills` | Skills: step-by-step guides that Claude Code follows for common jobs (including the five shop skills) |
| `scripts` | `e2e-shop.sh`, the end-to-end shop check |
| `.claude/agents` | Specialist helper agents (backend-dev, frontend-dev, test-writer, code-reviewer, ui-designer) |
| `.claude/commands` | Saved prompts: /run-app, /review-changes, /add-test |
| `.claude/hooks` | Scripts that run automatically (see `.claude/settings.json`) |
| `.mcp.json` | MCP servers for this project (Playwright browser) |
| `FEATURE.md` | The spec of the paid-course shop (rules R1 to R20). The shop skills read it |
| `AGENT-MAP.md` | Agent map and sample prompt for Day 5 |
| `MCP.md` | What MCP is and how to use it |
| `HOOKS-COMMANDS-AGENTS.md` | Reference notes for hooks, commands and agents (Day 4) |

Data is kept in memory. When you stop the backend, enrollments, bookmarks and new courses are gone.

## If something goes wrong

1. **"Address already in use" (port busy).** Another program uses port 5080 or 5173. Close it, or stop the old `dotnet run` / `npm run dev` and start again.
2. **Wrong .NET version.** Run `dotnet --version`. It should start with `8.`. If not, install the .NET 8 SDK from the link above.
3. **`npm install` fails.** Run `node --version`. It should be 20 or higher. Then delete the `frontend/node_modules` folder and run `npm install` again.
4. **The browser does not open (Playwright).** If the browser does not open, run `npx playwright install chromium` once.

Windows: hooks use bash scripts. Run Claude Code from Git Bash or WSL.
