# HarmonyPatchFixer

A universal fixer mod for **Slay the Spire 2** that repairs Harmony mods broken by calling `PatchAll()` without specifying their assembly.

## The Problem

Some STS2 mods call `Harmony.PatchAll()` without passing an explicit assembly parameter:

```csharp
_harmony.PatchAll(); // BUG: uses Assembly.GetCallingAssembly()
```

When the game invokes mod initializers via reflection, `Assembly.GetCallingAssembly()` returns the **game's assembly** instead of the mod's. This causes Harmony to scan the wrong assembly and never find the mod's `[HarmonyPatch]` classes.

This bug is system-dependent — it may work on some machines but fail on others depending on how the runtime handles reflection calls.

## How It Works

After a 2-second delay (to let all mods initialize), HarmonyPatchFixer scans all loaded assemblies, identifies mod assemblies by their load path (`mods` folder), and calls `PatchAll(asm)` with the correct assembly for each:

```csharp
fixHarmony.PatchAll(asm); // Correctly passes the mod's own assembly
```

## Installation

1. Download `HarmonyPatchFixer.dll` and `HarmonyPatchFixer.json` from [Releases](https://github.com/LiuZhanMingFpy/HarmonyPatchFixer/releases)
2. Create a folder `mods/HarmonyPatchFixer` in your STS2 game directory
3. Place both files inside that folder
4. Launch the game

## Building from Source

Requires:
- .NET 9 SDK
- A copy of Slay the Spire 2

1. Clone this repo
2. Update the `HintPath` values in `HarmonyPatchFixer.csproj` to point to your STS2 installation's `data_sts2_windows_x86_64` folder
3. Run `dotnet build -c Release`
4. Copy the output `HarmonyPatchFixer.dll` + `HarmonyPatchFixer.json` to your `mods/HarmonyPatchFixer` folder
