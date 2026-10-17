#!/usr/bin/env bash
# Blocks edits to .env and appsettings.*.json files.
input=$(cat)
file=$(echo "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -n 1)
name=$(basename "$file")
case "$name" in
  .env|.env.*|appsettings.*.json)
    echo "Blocked: $name may hold secrets. Ask the user to edit it." 1>&2
    exit 2 ;;
esac
exit 0
