---
description: Start the LearnHub backend and frontend and tell me the URLs
---

Start LearnHub so I can use it. Extra notes from me: $ARGUMENTS

1. Start the backend in the background: `cd backend/LearnHub.Api && dotnet run` (port 5080).
2. Start the frontend in the background: `cd frontend && npm install && npm run dev` (port 5173).
3. Wait until both answer, for example with `curl localhost:5080/api/courses`.
4. Tell me the two URLs: http://localhost:5173 (website) and http://localhost:5080/api/courses (API).
5. If a port is busy, say which program uses it. Do not kill anything without asking.
