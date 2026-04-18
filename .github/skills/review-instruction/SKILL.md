---
name: review-instruction
description: Review copilot instruction `*.instructions.md` to follow best practices
applyTo: "**/*.instructions.md"
version: 1.0.0
---

# Review Copilot Instruction

## Purpose
Review instruction files for correctness, clarity, and value.

## Review Process
- Treat each bullet or statement in the instruction file as a **rule**.

### Format
- Ensure the instruction file has correct name and format.
- Ensure required fields exist: `name`, `description`, `applyTo`, `version`.
- Validate `applyTo` glob pattern matches intended files.

### Review instruction content
- Ensure each rule is short, clear, and actionable.
- Ensure each rule includes reasoning when the intent is not obvious.
- Ensure rules focus on non-obvious guidance (not enforced by linters or existing tools).
- Ensure instructions do not duplicate existing instructions or well-known conventions (e.g., standard language naming conventions).
- Ensure instructions do not mix different scopes (e.g., backend vs API).

### Important
- Review ONLY the provided diff.
- Differentiate between file-level instructions and global instructions. File-level instructions should be specific to the project, while global instructions should be applicable across multiple projects.
- Do not suggest new content that is not present in the diff. Focus on improving the existing instructions rather than adding new ones.

### Heuristics
- Is it the right scope (file-level vs global, project-specific vs cross-project)?
- Does the instruction provide real value?

## Output Format
- Summary
- Good
- Issues
- Suggestions how to fix issues
- Verdict (APPROVE / NEEDS CHANGES)
