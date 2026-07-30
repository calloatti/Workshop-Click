Include ..\AGENTS.md

# Workshop Click — Mod-Specific Agent Instructions

## Identity
- **Assembly:** `workshopclick`
- **Namespace:** `Calloatti.WorkshopClick`
- **Framework:** Harmony, Bindito DI
- **ModId:** `Calloatti.WorkshopClick`
- **Min Game Version:** 1.0.12.5 — uses `timberborn-decompiled-1.0.*`

## What This Mod Does
Navigates directly to the Timberborn Steam Workshop page from the main menu with a single click. Supports maps and mods sections.

## Source Architecture (`Version-1.0/Source/`)

| File | Role |
|---|---|
| `WorkshopClick.cs` | `IModStarter` entry point + core navigation logic |
| `WorkshopClick.Mods.cs` | Mods section navigation |
| `WorkshopClick.Maps.cs` | Maps section navigation |
