#!/usr/bin/env bash
# Runs dotnet build only when the edited file is a .cs file.
input=$(cat)
file=$(echo "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -n 1)
case "$file" in
  *.cs) cd "${CLAUDE_PROJECT_DIR:-.}/backend" && dotnet build --nologo -v q 1>&2 || exit 2 ;;
esac
exit 0
