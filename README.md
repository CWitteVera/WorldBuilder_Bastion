# BastionBuilder

## Overview

BastionBuilder is a stronghold application designed for Game Masters to create dynamic and intricate bastions using the following features:

- **Nodes**: Represent points of interest in your stronghold.
- **Edges**: Connect nodes to define pathways and relationships.
- **Walls**: Physical barriers that can be configured and modified.
- **Secrets**: Hidden elements that can be found and revealed.

## Database

The application uses **SQLite** for efficient data storage and retrieval.

## Exporting Data

Data can be exported in both **DM** and **Public JSON** formats, enhancing usability for different gaming scenarios.

## Building & Testing

```bash
dotnet build      # build all projects
dotnet test       # run all unit tests
```

## Publishing the App (Windows x64)

Run the PowerShell script to produce a self-contained single-file executable:

```powershell
./scripts/publish-win-x64.ps1
```

The published executable is written to:

```
src/BastionBuilder.App/bin/Release/net8.0-windows/win-x64/publish/BastionBuilder.exe
```

## Publishing Steps

When ready, follow these steps to publish your stronghold:
1. Verify all nodes, edges, and walls are configured correctly.
2. Test the application for any bugs or issues.
3. Export your stronghold in the desired format.
4. Share with your gaming group for an engaging experience!