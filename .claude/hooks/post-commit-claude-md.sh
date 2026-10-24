#!/usr/bin/env bash
# PostToolUse hook: after a successful `git commit`, ask Claude to review CLAUDE.md.
input=$(cat)

cmd=$(printf '%s' "$input" | node -e 'let s="";process.stdin.on("data",d=>s+=d).on("end",()=>{try{console.log((JSON.parse(s).tool_input||{}).command||"")}catch{}})')
case "$cmd" in
  *"git commit"*) ;;
  *) exit 0 ;;
esac

files=$(git show --name-only --pretty=format: HEAD 2>/dev/null | sed '/^$/d')
[ -z "$files" ] && exit 0
# Avoid loops: skip if the commit only touched CLAUDE.md
[ "$files" = "CLAUDE.md" ] && exit 0

msg="A commit was just made. Files changed: $(printf '%s' "$files" | tr '\n' ' '). Review whether CLAUDE.md is still accurate (commands, configuration, architecture) and update it if needed. Do not commit the CLAUDE.md change automatically."
MSG="$msg" node -e 'console.log(JSON.stringify({hookSpecificOutput:{hookEventName:"PostToolUse",additionalContext:process.env.MSG}}))'
