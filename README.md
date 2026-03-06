# BastionBuilder (v0.1 scaffold)

Windows-only offline stronghold mapper + rules tracker for a DM. The design goal is **“download → run”** with **no extra installs** for end users.

## Goals (v1)
- Track a stronghold as a graph of **Nodes** (rooms/areas) connected by **Edges** (doors/windows/stairs/hatches).
- Track perimeter **Walls** as **WallGroups** containing **WallSegments** (material, HP, quality, reinforcements).
- Support **shared walls** between adjacent nodes (single segment objects, not duplicated).
- Support **openings split segments** (doors/windows replace wall segment spans).
- Support **secrets** (secret edges/features) with DM-only metadata until revealed.
- Provide DM-guided discovery support via **Discoverability (0–20)**.
- Provide **node-local reveal** with propagation to the far side only after:
  - physical entry, or
  - remote-view “enter” (scry/clairvoyance).
- Persist locally via **SQLite**.
- Export:
  - **DM export** (full data + discoverability + leak flags)
  - **Public export** (node callouts only, e.g. `Discovered — "False Painting Door"`)

## Tech stack
- **.NET 8**
- **WPF** UI (Windows-only)
- **MVVM** pattern
- **Clean Architecture** style layering

## Solution structure
- `src/BastionBuilder.Domain`  
  Pure domain entities/value objects (no UI, no persistence).
- `src/BastionBuilder.Rules`  
  Rules engine modules (depends only on Domain): shared walls, opening split, discoverability, reveal propagation, reinforcements (additive), height scaling (stub).
- `src/BastionBuilder.Application`  
  Use cases (commands/queries) + interfaces/ports. UI talks only to this layer.
- `src/BastionBuilder.Persistence.Sqlite`  
  SQLite implementation (schema/migrations + repositories).
- `src/BastionBuilder.Export`  
  JSON export DTOs + generators for DM/Public exports.
- `src/BastionBuilder.App`  
  WPF UI shell (MVVM).
- `tests/*`  
  Unit tests, especially for `Rules` modules to “lock down” behavior.

## Build & run (dev)
Prereqs:
- Visual Studio 2022 (or later) with .NET desktop workload **or** .NET 8 SDK

Commands:
- `dotnet build`
- `dotnet test`
- Run the WPF app from Visual Studio or:
  - `dotnet run --project src/BastionBuilder.App`

## Publish (single-file exe, no installs)
Target: produce a self-contained single-file `.exe` suitable for “unzip and run”.

Example (x64):
```powershell
dotnet publish src/BastionBuilder.App -c Release -r win-x64 `
  /p:PublishSingleFile=true `
  /p:SelfContained=true `
  /p:PublishReadyToRun=true
```

Output is under:
- `src/BastionBuilder.App/bin/Release/net8.0-windows/win-x64/publish/`

## Design docs
Place your living rules/docs under `docs/` (player guide, DM rules, checklists). The application should implement these as the source-of-truth behaviors.

## Development approach (“lock modules”)
1. Stabilize **Domain** types early.
2. Implement one Rules module at a time with unit tests.
3. Keep UI thin: no rule logic in WPF code-behind.
4. Treat passing tests as “locked” behavior; refactors must preserve tests.

## License
TBD.
