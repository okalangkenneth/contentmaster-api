# Project Rules — Inherited Codebase Edition

## Memory Architecture (3-Layer System)

This project uses a unified memory approach combining:

| Layer | Location | Purpose | Auto-Loaded |
|-------|----------|---------|-------------|
| **CLAUDE.md** | Project root | Rules, workflow, conventions | ✅ Always |
| **MEMORY.md** | `~/.claude/projects/<project>/memory/` | Session learnings, patterns Claude discovers | ✅ First 200 lines |
| **claude-mem** | `~/.claude-mem/` | Deep searchable history, AI-compressed | ✅ Via MCP injection |

### How They Work Together

```
Session Start
     │
     ├─► CLAUDE.md loaded (your rules)
     │
     ├─► MEMORY.md loaded (Claude's learnings from past sessions)
     │
     ├─► claude-mem context injected (relevant historical observations)
     │
     └─► You're immediately productive — no re-explaining
```

### Memory Commands

| Command | Purpose |
|---------|---------|
| `/memory` | View/toggle auto-memory, edit CLAUDE.md |
| `/remember` | Suggest patterns to save permanently |
| `/compact` | Instant (uses pre-written Session Memory) |
| `/dream` | Manually trigger memory consolidation |
| `Ctrl+O` | Expand "Recalled/Wrote memories" details |

### Auto-Dream (Memory Cleanup)

Auto-dream runs automatically after 24h + 5 sessions. It consolidates memory by:
- Converting relative dates to absolute ("yesterday" → "2026-03-27")
- Removing contradicted/stale entries
- Merging duplicate notes
- Keeping MEMORY.md under 200 lines

Use `/dream` to trigger manually if memory feels cluttered.

---

## CLAUDE.md File Hierarchy

CLAUDE.md loads automatically every session, but only the root file is always loaded. Use the hierarchy to keep the root lean:

```
~/.claude/CLAUDE.md          ← global, applies to ALL sessions
./CLAUDE.md                  ← project root (this file) — always loaded
./CLAUDE.local.md            ← personal overrides, gitignored
./src/Services/CLAUDE.md     ← loaded on-demand when working in that dir
./src/Data/CLAUDE.md         ← loaded on-demand for database work
```

**The subdirectory trick:** Instead of cramming every module's conventions into the root (and burning instruction budget), put them in subdirectory `CLAUDE.md` files. Claude loads them automatically when working in that area.

Common candidates for inherited .NET projects:
- `src/Services/CLAUDE.md` — service layer patterns, dependency rules
- `src/Data/CLAUDE.md` — EF Core patterns, migration conventions
- `src/Api/CLAUDE.md` — controller conventions, response shape rules

**Instruction budget:** CLAUDE.md has ~150 usable instruction slots (system prompt uses ~50). Every line you add that Claude doesn't genuinely need dilutes the lines that matter. Subdirectory files let you spend budget where it's relevant.

**Project-scoped slash commands:** Define them in `.claude/commands/*.md` at the repo root — committed alongside code, auto-discovered by CC.

### Portability (CC ↔ Codex)

Skills are the portable layer — identical markdown files read by both tools. Set up symlinks once per project so config stays in sync with zero maintenance:

```bash
# Run once at project root (elevated cmd or PowerShell)
mklink AGENTS.md CLAUDE.md
mklink /D .agents\skills .claude\skills
```

Codex looks for `AGENTS.md` and `.agents/skills/`; CC looks for `CLAUDE.md` and `.claude/skills/`. Symlinks mean one source of truth for both.

**What transfers:** skills, shared knowledge files, docs/references.
**What stays CC-only:** hooks, MCP config (`.claude/settings.json`), subagents (CC auto-invokes; Codex requires explicit calls).

---

## Discovery Phase — Do This EVERY Session Start

> **These steps are non-negotiable. Never skip discovery.**

### Step 1 — Understand Recent History
```bash
git log --oneline -20
```
Note: what changed recently, what's in flight, any half-finished work.

### Step 2 — Run the Build
```bash
dotnet build ContentMasterAPI.sln
```
Record ALL warnings and errors in **Build State** below before touching anything.

### Step 3 — Read the README
Note what's documented vs. what's actually in the code. Gaps = potential tech debt or missing context.

### Step 4 — List What's Broken
Before making any changes, capture the current broken state. This prevents accidentally "fixing" things that were intentionally left or introducing regressions.

---

## Build State (UPDATE EVERY SESSION)

**Claude Code: Update this section at session start AND end.**

| Field | Value |
|-------|-------|
| Last known clean build | 2026-06-24 |
| Build command | dotnet build ContentMasterAPI.sln |
| Last run by Claude | 2026-06-24 |

### Current Build Errors
```
None
```

### Current Warnings
```
None (pre-existing nullable warnings suppressed via <Nullable>disable</Nullable>)
```

### Services Verified Working
- [x] ContentMasterAPI.API (starts, Swagger visible at /)
- [ ] PostgreSQL (Phase 2)
- [ ] Ollama AI integration (Phase 3)

---

## What We Know vs. What We Assume

- **Never assume** a pattern is intentional — it might be a bug
- **Document every non-obvious decision** discovered in the codebase
- **Flag technical debt** separately from bugs (see section below)
- If something looks wrong: read the git history before changing it

```bash
# Before changing anything suspicious:
git log --follow -p -- path/to/file   # full history of a file
git blame path/to/file                 # who wrote each line and when
```

---

## Technical Debt Register

Track debt separately from bugs. Debt = known compromises, not errors.

| ID     | Location                                  | Description                                                      | Severity | Safe to touch? |
|--------|-------------------------------------------|------------------------------------------------------------------|----------|----------------|
| TD-001 | Controllers/AuthController.cs             | Fake auth (admin/admin123) — remove entirely in Phase 1B         | High     | Yes (Phase 1B) |
| TD-002 | Infrastructure/Data/ContentMasterDbContext| EF DbContext exists but never registered in DI                   | High     | Yes (Phase 2)  |
| TD-003 | Infrastructure/Services/OpenAiContentAnalysisService.cs | Stale OpenAI SDK — replace with ILlmService Phase 3 | High     | Yes (Phase 3)  |
| TD-004 | Middleware/RapidApiMiddleware.cs          | UsageTrackingService never called from middleware                 | Med      | Yes (Phase 4)  |
| TD-005 | Controllers/SubscriptionController.cs     | Domain models (Subscription, BillingDetails etc) defined in controller | Med | Yes (Phase 1B) |
| TD-006 | Controllers/PaymentController.cs          | Domain models (PaymentMethod, Invoice etc) defined in controller | Med      | Yes (Phase 1B) |
| TD-007 | Core/Interfaces/IContentRepository.cs    | Interface bloated with sync + duplicate GraphQL-specific methods  | Med      | Yes (Phase 2)  |
| TD-008 | ContentMasterAPI.Tests/                   | Tests project has no .csproj — may be incomplete scaffold         | Med      | Yes (Phase 7)  |
| TD-009 | Infrastructure/Services/OpenAiContentAnalysisService.cs | sync-over-async (.GetAwaiter().GetResult()) in GenerateTags, GenerateSummary, CategorizeContent | High | Yes (Phase 3) |
| TD-010 | Controllers/SubscriptionController.cs + PaymentController.cs | All marketplace endpoints return hardcoded fake data | Med | Yes (Phase 4) |

**Severity guide:**
- **High** — will cause data loss, security hole, or prod outage
- **Med** — degrades reliability or maintainability
- **Low** — cosmetic, style, or minor inefficiency

---

## Bug Register

| ID | Location | Symptom | Root Cause (if known) | Fixed? |
|----|----------|---------|----------------------|--------|
| BUG-001 | | | | |

---

## Build Progress (KEEP UPDATED)

**Claude Code: Update this section at the end of every session.**
**Native auto-memory handles patterns; this tracks deliverables.**

### ✅ COMPLETED

- Phase 0: Audit — 22 debt items identified across security, architecture, and quality
- Phase 1A: Security hardening + structural cleanup
  - Removed hardcoded JWT secret from appsettings.json
  - Fixed DateTime.Now → DateTime.UtcNow in AuthController
  - Replaced hardcoded RapidAPI demo keys with config-driven loading
  - Enabled Swagger in all environments
  - Moved IUsageTrackingService + UsageStatistics to Core
  - Removed Class1.cs placeholders
  - Removed AfterTargets publish and hardcoded linux-x64 from csproj
  - Deleted 15 stale root-level deployment artifacts
  - Removed committed publish/ directory
  - Excluded Phase 2/3 stub files (EfContentRepository, ContentMasterDbContext, OpenAiContentAnalysisService) from build until their phases add required packages

### 🔨 IN PROGRESS
<!-- Current work -->

### ❌ REMAINING

- Phase 1B: Auth simplification (remove JWT AuthController, RapidAPI keys as sole auth)
- Phase 2: PostgreSQL persistence (port 5435, EF migrations, EfContentRepository)
- Phase 3: AI service layer (ILlmService, OllamaLlmService, GroqLlmService)
- Phase 4: RapidAPI integration (wire UsageTrackingService into middleware)
- Phase 5: Containerization + observability (docker-compose, Serilog, health checks)
- Phase 6: Kubernetes + Helm chart
- Phase 7: CI/CD (GitHub Actions)
- Phase 8: RapidAPI listing (freemium launch)
- Phase 9: README + LinkedIn post

---

## Tech Stack

- **Runtime**: .NET 8
- **Framework**: ASP.NET Core (Program.cs minimal hosting), HotChocolate 15.x (GraphQL)
- **Database**: PostgreSQL 16 (Phase 2, port 5435), In-memory repository (current)
- **Testing**: xUnit, WebApplicationFactory
- **Package Manager**: NuGet
- **Infrastructure**: Docker, Kubernetes + Helm (Phase 5+), GitHub Actions (Phase 7)
- **AI**: Ollama (local dev, http://localhost:11434) / Groq (production, https://api.groq.com)
- **Memory**: Native auto-memory + claude-mem for deep history

---

## Claude Integration in .NET

If a project requires Claude/AI integration, prefer the **official Anthropic C# SDK** (`Anthropic` NuGet package, v10+) over ad-hoc `HttpClient` wrappers.

**Why:** The SDK provides typed access, automatic retries with exponential backoff, configurable timeouts, streaming via `IAsyncEnumerable`, and `IChatClient` integration for `Microsoft.Extensions.AI` — all the plumbing you'd otherwise own yourself.

```bash
dotnet add package Anthropic
```

**Key points for inherited codebases:**
- Prefer `IChatClient` abstraction so Claude can slot in as a standard provider alongside others
- Use environment variable `ANTHROPIC_API_KEY` — never hardcode keys
- SDK is currently **beta** — APIs may change between versions; pin the package version in `*.csproj`
- If the project already has a hand-rolled wrapper, flag it as tech debt (TD register) but don't refactor unless asked
- **ASP.NET Core DI registration:**
  ```csharp
  builder.Services.AddSingleton<AnthropicClient>(_ =>
      new AnthropicClient(apiKey: builder.Configuration["Anthropic:ApiKey"]));
  ```

**Do NOT:** reach for a bare `HttpClient` + manual JSON for Anthropic calls when this SDK is available.

---

## Installed Plugins

| Plugin | Source | Purpose |
|--------|--------|---------|
| `pr-review-toolkit` ✅ | Anthropic official | 6 specialist agents: code review, test gaps, silent failures, type design, comments, simplification — auto-trigger |
| `code-review` ✅ | Anthropic official | 4 parallel agents on PRs, confidence-scored (80+ threshold), CLAUDE.md compliance — invoke with `/code-review` |
| `claude-mem` ✅ | thedotmack | Deep searchable session history via MCP |

---

## Token Efficiency — code-review-graph

Inherited codebases are large and Claude will over-read without guidance. `code-review-graph` builds a persistent SQLite dependency graph (Tree-sitter supports C#) and hands Claude only the files relevant to your current change via MCP.

**Install once:**
```bash
claude plugin add tirth8205/code-review-graph
claude plugin install code-review-graph@code-review-graph
# Restart Claude Code
```

**One-time setup per project:**
```bash
cd your-project
code-review-graph build   # parses into .code-review-graph/
```

**During active development:**
```bash
code-review-graph watch   # auto-updates graph on every file save
code-review-graph status  # check coverage
```

After `build`, Claude narrows context automatically — no prompt changes needed.

---

## Custom Subagents

Subagents live in `.claude/agents/` and get a **fresh context window** — they do isolated work and return only the result. Essential for inherited codebases where exploration tasks would otherwise bloat your main session.

**Agents suited for inherited .NET projects:**
```
.claude/agents/
├── dead-code-finder.md       # Finds unused classes/methods via grep + git
├── dependency-auditor.md     # Reviews NuGet packages for outdated/vulnerable deps
└── test-writer.md            # Generates xUnit tests for a given class/method
```

**How to invoke:**
```
"Spin up a subagent to find dead code in src/Services/"
"Use the test-writer subagent to write xUnit tests for PaymentService.cs"
```

**Mental test:** *Will I need the intermediate output, or just the conclusion?*
If only the conclusion → subagent. Keep in main session if you'll need the detail.

---

## Workflow

```
1. Run discovery phase (git log, build, README, list broken)
2. Make only the explicitly requested change
3. Typecheck / build
4. Run tests
5. Verify nothing regressed
6. Commit: conventional commits (feat:, fix:, chore:)
```

### Before Every Change
- Only modify what was explicitly requested
- Ask if <90% confident about intent
- Offer 2-3 options for significant decisions
- If you find something suspicious — read git history before changing it

---

## Git Conventions
- **Branching**: `main` / `master` = production. Feature: `git checkout -b feat/name`
- **Commits**: `feat:`, `fix:`, `chore:`, `docs:`, `refactor:`, `test:`
- **Before commit**: run build + tests
- **Never commit**: `.env`, API keys, secrets, connection strings

---

## GitHub Repository

**Repo**: https://github.com/okalangkenneth/contentmaster-api

### Rules
- Commit and push at the end of **every phase**, not just when done
- Keep commit messages descriptive
- Never push `.env` or `.env.local`

---

## Critical Rules

### Code Quality
- NO placeholders (`YOUR_API_KEY`, `TODO`, `FIXME`) — unless already present in inherited code
- Environment variables for secrets
- Remove unused imports only in files you're actively modifying
- Add logging for API calls/errors

### Inherited Code Caution
- Do NOT refactor code you weren't asked to change
- Do NOT "improve" surrounding code when fixing a bug
- Do NOT rename things without checking all usages
- Do NOT delete code that looks unused — verify with `git grep` first

### Money Handling (NON-NEGOTIABLE)
- ALL money as INTEGER cents/öre
- NEVER `parseFloat()` for financial values
- 100% test coverage for money calculations

### ContentMasterAPI-Specific Rules
- Auth model: RapidAPI keys only (X-RapidAPI-Key header). No internal user management.
  The JWT AuthController is being removed in Phase 1B.
- AI provider switching: LLM__Provider env var controls "Ollama" vs "Groq".
  LLM__BaseUrl and LLM__ApiKey control the connection details.
- Port 5435 reserved for PostgreSQL (avoids conflict with system PostgreSQL on 5432,
  EconAdvisor on 5433, and FinTrak on 5434).
- NEVER use DateTime.Now — always DateTime.UtcNow.
- NEVER use .GetAwaiter().GetResult() — async/await throughout.
- Secrets go in appsettings.Development.json (gitignored) or environment variables only.

### Subscription Tiers (RapidAPI — target pricing)
| Tier  | Price     | Monthly Requests | AI Features                        |
|-------|-----------|------------------|------------------------------------|
| Basic | Free      | 500              | Content CRUD only                  |
| Pro   | $9.99/mo  | 10,000           | + Sentiment analysis, auto-tagging |
| Ultra | $29.99/mo | 50,000           | + Summarization, categorization    |

---

## Memory Commands

| Command | Purpose |
|---------|---------|
| `/memory` | View/toggle auto-memory, edit CLAUDE.md |
| `/remember` | Suggest patterns to save permanently |
| `/compact` | Instant (uses pre-written Session Memory) |
| `/dream` | Manually trigger memory consolidation |
| `Ctrl+O` | Expand "Recalled/Wrote memories" details |

---

## Session Management

### Starting
1. CLAUDE.md and MEMORY.md auto-load
2. **Run the discovery phase** (see above — never skip)
3. Check "Recalled X memories" for Session Memory
4. Update **Build State** section with current findings

### During
- "Wrote X memories" = Claude saved learnings
- Use `/remember` to promote patterns to permanent memory
- Update Bug Register / Tech Debt Register as you find things

### During /compact
**Compact proactively — don't wait for autocompact.** Autocompact fires at peak context rot, when the model is least capable of writing a good summary. Especially critical in 1–2 hr sessions on inherited codebases.

Always pass a hint:
```
/compact focus on <current task>, drop <stale discovery/dead ends>
```
Example: `/compact focus on the payment service refactor, drop the initial build error investigation`

Preserve on every compact:
- Modified files with paths
- Current branch and uncommitted changes
- Pending tasks
- Bug/debt register additions from this session
- Key decisions made about inherited patterns

### Rewind Strategy
**Prefer rewind over correction.** When Claude goes down a wrong path:
1. Press `Esc Esc` (or `/rewind`) — jump back to just after useful file reads
2. Re-prompt with what you learned: `"That approach breaks X — go straight to Y instead"`

**Rewind-as-handoff:** Ask Claude to summarize findings before rewinding, then paste the summary into the re-prompt.

**Two failed corrections rule:** If you've corrected Claude on the same problem twice and it's still wrong — stop. Run `/clear` and rewrite the prompt from scratch. Don't keep correcting into a polluted context.

### Session Handoff

Run this prompt **proactively every 30–45 minutes** — not when the limit hits, because by then CC is already locked. Always have a recent handoff ready to paste into Codex.

```
Summarize this session as a handoff:
- Active files (paths + what changed)
- Key decisions made
- Blockers encountered
- Exact next steps
Keep it under 20 lines.
```

Paste the output as the opening message of the next session — in CC or Codex.

### Effort Levels

| Command | When to use |
|---------|------------|
| `/effort low` | Boilerplate, simple edits, typo fixes |
| `/effort medium` | Default — most coding tasks |
| `/effort high` | Hard bugs, architecture decisions, complex debugging |
| `/effort auto` | Reset to default |

> **Rule of thumb:** When in doubt about effort, go one level higher — the cost is tokens, the benefit is catching mistakes before they're in production.

> **Plan mode:** Press `Shift+Tab` twice before any non-trivial change to get a read-only analysis. Especially valuable before architecture decisions or refactors touching multiple files.

### Ending
- Update Build Progress section
- Update Build State with any new errors/fixes
- Claude auto-saves to MEMORY.md — no manual export needed

### End-of-Session Prompt
```
Update the Build State, Build Progress, and any new Technical Debt or Bug Register entries in CLAUDE.md, then stop.
```

---

## Corrections Log

| Date | Mistake | Rule |
|------|---------|------|
| | Changed unrelated code | Only modify what's requested |
| | Assumed pattern was intentional | Check git history before assuming |
| | Deleted "unused" code | Verify with git grep before deleting |

---

## Verification Commands

```bash
# Background monitoring — runs on a timer while you work on something else
/loop 5m check if the CI pipeline on current branch passed and report back
/loop 10m check for new failing tests on main

# Quick in-context question (answer discarded, no history pollution)
/btw

# Post-implementation review (auto-triggers via pr-review-toolkit, or invoke manually)
/code-review

# Before committing
dotnet build ContentMasterAPI.sln

# Quick commit
git add -A && git commit -m "fix: description"

# Check memory status
/memory

# View claude-mem dashboard
curl http://localhost:37777/api/health
```
