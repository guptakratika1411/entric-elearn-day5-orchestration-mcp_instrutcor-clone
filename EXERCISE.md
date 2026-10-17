# Day 5 Exercise: Orchestration, Agent Map and MCP

An agent map is a plan of which helper does which job, and in what order.
Orchestration means one main Claude hands jobs to helper agents and then checks their work.
MCP is a plug that gives Claude new tools, such as a real browser.

## Steps

0. Make a commit before you start (`git add . && git commit -m start`) so Claude can see what changed with `git diff`.
1. Run the app and the tests (see README.md). Look inside `.claude/agents/`. Read AGENT-MAP.md.
2. Type `/mcp` and approve the Playwright server. Ask Claude to open the app in the browser and enroll in a course.
3. Draw your own agent map (on paper or in mermaid, a way to draw a diagram using text; see the example in AGENT-MAP.md) for "Assignment submission and grading".
   - Students submit a text answer for a course.
   - An instructor sees the submissions and gives a grade and feedback.
   - The student sees the grade.
4. Start the build with one prompt to the main agent. Fill in the blanks:
   ```
   Build "Assignment submission and grading": ______________________.
   First write a plan and an API contract (routes and shapes).
   Use backend-dev and frontend-dev in parallel.
   Then use test-writer for ______________________.
   Then use code-reviewer. Fix problems it finds and summarize.
   ```
5. Watch the agents work. Note what each one reported back.
6. Use Playwright MCP to check the new pages work in the browser.
7. Write 3 lines: what went well, what went wrong, what you would change in your agent map.

## What to bring to Day 6

Your agent map and the working feature.
