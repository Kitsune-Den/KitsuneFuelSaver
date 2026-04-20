# KitsuneFuelSaver

A QoL Harmony mod for 7 Days to Die. Auto-shuts off forges, campfires, and other fueled workstations the instant their craft queue is empty — no more fuel wasted while you're off doing literally anything else.

## Install

1. Download the latest release zip.
2. Extract so that `Mods/KitsuneFuelSaver/` lands inside your 7D2D install.
3. Launch the game. You should see `[KitsuneFuelSaver] Loading Harmony patches` in the log.

Three files ship: `ModInfo.xml`, `KitsuneFuelSaver.dll`, and `0Harmony.dll` (V2.x no longer ships Harmony in `Managed/`, so the mod bundles its own copy).

## What it does

Hooks `TileEntityWorkstation.UpdateTick` with a Harmony postfix. After the game's normal tick:

- If the station is burning AND has the fuel module AND the craft queue is empty:
  - For forge-style stations (material-input module), also require raw input slots to be empty so in-flight smelting still completes.
  - Otherwise, flip `IsBurning` to `false`.

Non-fueled stations (chem bench, workbench, cement mixer) are untouched — the patch short-circuits for them.

Fuel already in the fuel slots is preserved. Just light it again next time.

## Repo layout

```
/                        <- dev root
  KitsuneFuelSaver.sln / .csproj
  Harmony/KitsuneFuelSaver.cs    <- source
  libs/                         <- vendored 7D2D DLLs (compile-time only)
  KitsuneFuelSaver/              <- THE MOD FOLDER (drop this into 7D2D/Mods/)
    ModInfo.xml
    KitsuneFuelSaver.dll         <- produced by build
    0Harmony.dll                <- copied in by build
```

The build outputs straight into `KitsuneFuelSaver/`, so after `dotnet build -c Release` that folder is the shippable mod.

## Build from source

Requires .NET SDK 8+. The build depends on a few 7 Days to Die DLLs that aren't redistributable, so they're not in the repo — you need to provide them yourself from your local install:

```
libs/
  0Harmony.dll                    (from another Harmony-using 7D2D mod, or download HarmonyX)
  Assembly-CSharp.dll
  Assembly-CSharp-firstpass.dll
  LogLibrary.dll
  UnityEngine.dll
  UnityEngine.CoreModule.dll
```

Copy those from `<7D2D install>/7DaysToDie_Data/Managed/`. Note: V2.x no longer ships `0Harmony.dll` there — any Harmony-based 7D2D mod already installed on your system will have a copy, or grab the latest HarmonyX release for .NET Framework 4.8.

Then:

```
dotnet build -c Release
```

Or run `01-CreateRelease.bat`.

## Compatibility

Built and runtime-verified against 7 Days to Die **V2.6.14**. Decompile confirms the `TileEntityWorkstation` shape used; single-player tested. The `setModified()` call inside `IsBurning`'s setter handles dedi state sync.

No load-order requirements — bundles its own Harmony, patches a vanilla class as a postfix, coexists with other Harmony mods.

## License

MIT. See [LICENSE](LICENSE).
