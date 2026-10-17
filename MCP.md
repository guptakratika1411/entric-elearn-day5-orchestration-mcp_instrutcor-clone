# MCP (Model Context Protocol)

## What it is

1. MCP is a plug that gives Claude new tools.
2. An MCP server is the thing you plug in. It can be a browser, GitHub or a database.
3. With the Playwright server, Claude can open a real browser, click buttons and take screenshots.

## Where the setup lives

1. Project file: `.mcp.json` at the repo root. It is shared with everyone who uses this repo. This repo already has one for Playwright.
2. Your own setup: run `claude mcp add` in a terminal. It saves the server for you only (local scope) or for all your projects (user scope).
3. Node.js must be installed. Playwright runs through `npx`.

## First run

1. Start Claude Code in this folder.
2. Claude asks if you trust the `playwright` server from `.mcp.json`. Approve it.
3. Type `/mcp` to see the list of servers and whether each one is connected.
4. If the browser does not open, run `npx playwright install chromium` once.

## Try these prompts

1. Start the app first (backend and frontend).
2. "Open http://localhost:5173 with the browser, enroll as Asha in the first course, and tell me what you see."
3. "Take a screenshot of the Certificates page."

## Other servers to try

1. GitHub: read issues and pull requests, open a PR.
2. Filesystem: let Claude read and write files in a folder you choose.
3. A database (for example Postgres or SQLite): ask questions about your data in plain words.
