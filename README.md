# BastionBuilder

> **Windows-only offline application** for creating complex bastions for tabletop RPG campaigns.  
> Built with .NET 8, WPF, MVVM, and Clean Architecture.

---

## Features

- **Nodes** – rooms, corridors, and areas with discoverability DCs (0–20) and height stubs
- **WallGroups / WallSegments** – shared walls with additive reinforcements, opening splits, and height-scaling stubs
- **Edges** – doors, windows, stairs, hatches, and archways with full **lock / trap / alarm** fields
- **Features** – points of interest with **secret visibility states** (Hidden → Discovered → Revealed) and **reveal propagation**
- **SQLite persistence** – offline, single-file database via Entity Framework Core
- **JSON export** – separate **Public** (player-visible) and **DM** (full) exports

---

## Solution Structure

```
BastionBuilder.sln
├─ src/
│  ├─ BastionBuilder.Domain          – Entities & enums (Node, WallGroup, Edge, Feature …)
│  ├─ BastionBuilder.Rules           – Pure business rules (WallSegmentRules, DiscoverabilityRules)
│  ├─ BastionBuilder.Application     – MVVM ViewModels, RelayCommands, repository/export interfaces
│  ├─ BastionBuilder.Persistence.Sqlite – EF Core + SQLite DbContext & repository
│  ├─ BastionBuilder.Export          – JSON export service (Public + DM DTOs)
│  └─ BastionBuilder.App             – WPF entry point (Windows-only, net8.0-windows)
└─ tests/
   ├─ BastionBuilder.Domain.Tests
   ├─ BastionBuilder.Rules.Tests
   ├─ BastionBuilder.Application.Tests
   └─ BastionBuilder.Export.Tests
```

---

## Prerequisites

| Requirement | Version |
|---|---|
| Windows | 10 22H2 or later (WPF requires Windows) |
| .NET SDK | **8.0** ([download](https://dotnet.microsoft.com/download/dotnet/8)) |

> The test and library projects are cross-platform (`net8.0`).  
> Only `BastionBuilder.App` is Windows-only (`net8.0-windows`).

---

## Build

```powershell
# Clone the repository
git clone https://github.com/CWitteVera/WorldBuilder_Bastion.git
cd WorldBuilder_Bastion

# Restore packages and build the whole solution
dotnet build BastionBuilder.slnx
```

---

## Run Tests

The test suite runs on any OS where .NET 8 is installed:

```powershell
dotnet test BastionBuilder.slnx --filter "FullyQualifiedName!~BastionBuilder.App"
```

Or run individual test projects:

```powershell
dotnet test tests/BastionBuilder.Domain.Tests
dotnet test tests/BastionBuilder.Rules.Tests
dotnet test tests/BastionBuilder.Application.Tests
dotnet test tests/BastionBuilder.Export.Tests
```

---

## Run the Application (Windows only)

```powershell
dotnet run --project src/BastionBuilder.App
```

The SQLite database is stored at:

```
%LOCALAPPDATA%\BastionBuilder\bastion.db
```

---

## Publish

### Self-contained single-file executable (Windows x64)

```powershell
dotnet publish src/BastionBuilder.App `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output ./publish `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

The output will be `publish/BastionBuilder.exe` – copy it anywhere on a Windows machine.

### Framework-dependent (requires .NET 8 installed on target)

```powershell
dotnet publish src/BastionBuilder.App `
  --configuration Release `
  --runtime win-x64 `
  --self-contained false `
  --output ./publish
```

---

## Export Formats

Both exports are accessible from the toolbar inside the application.

### Public JSON (`*_public_*.json`)

Omits hidden secrets. Suitable for sharing with players.

```json
{
  "id": "...",
  "name": "Fort Danger",
  "nodes": [
    {
      "name": "Entrance Hall",
      "publicDescription": "A grand vaulted entrance.",
      "visibleFeatures": []
    }
  ],
  "edges": [
    {
      "name": "Main Gate",
      "kind": "Door",
      "hasLock": true,
      "hasTrap": false,
      "hasAlarm": true
    }
  ]
}
```

### DM JSON (`*_dm_*.json`)

Includes everything: hidden secrets, lock/trap/alarm details, wall statistics, reveal chains.

```json
{
  "edges": [
    {
      "name": "Hidden Door",
      "isSecret": true,
      "visibilityState": "Hidden",
      "lockDC": 18,
      "trapKind": "poison needle",
      "trapDetectDC": 14,
      "alarmThreshold": 0
    }
  ],
  "wallGroups": [
    {
      "segments": [
        {
          "baseAC": 15, "effectiveAC": 18,
          "baseHP": 27, "effectiveHP": 42,
          "heightScaleFactor": 1.0,
          "solidFraction": 0.9
        }
      ]
    }
  ]
}
```

---

## Architecture Notes

| Concept | Implementation |
|---|---|
| Discoverability DC | 0 = always visible; 1–20 = Perception check required |
| Secret visibility | `Hidden → Discovered → Revealed`; reveal propagates to linked Features |
| Additive reinforcements | `EffectiveAC = BaseAC + Σ BonusAC`, `EffectiveHP = BaseHP + Σ BonusHP` |
| Opening splits | Each opening reduces the wall's solid fraction; `SolidFraction = clamp(1 − Σ SplitPercentage, 0, 1)` |
| Height scaling | `HeightScaleFactor = max(0.1, HeightFeet / 10.0)` – stub for future cover/damage formulas |
| SQLite | EF Core `EnsureCreated`; database path in `%LOCALAPPDATA%\BastionBuilder\` |

