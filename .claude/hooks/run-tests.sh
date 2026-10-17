#!/usr/bin/env bash
# Runs when Claude stops. Reports test results. Never exits 2, so it cannot loop.
input=$(cat)
# If this stop was already caused by a hook, do nothing.
if echo "$input" | grep -q '"stop_hook_active"[[:space:]]*:[[:space:]]*true'; then
  exit 0
fi
cd "${CLAUDE_PROJECT_DIR:-.}/backend" || exit 0
dotnet test --nologo -v q 1>&2
exit 0
