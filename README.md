# Bonk Ultra, Alpha (MelonLoader)

Version: 0.4.3

This mod checks run conditions after a run loads. If none of the qualifying
conditions are met, it holds `R` for 3 seconds to restart. If any condition is
met, it presses `ESC` once.

Qualifying conditions (any one):
- Soul Harvester >= 1 and Green Credit Card >= 1
- Moai >= 6
- Legendary Vendors >= 2 and Epic Vendors >= 1
- Green Credit Cards >= 2, Microwaves >= 1, and Epic Microwaves >= 1

Legendary detection uses the `InteractableShadyGuy` vendor rarity tier (the
legendary hat color), aligned with what SeedInfoMod exposes.

## Pause menu controls

Open the pause menu or main menu and click the `Bonk Ultra` button (legendary
yellow) to open the settings panel. From there you can toggle auto-restart,
game sound, and the four condition toggles (Condition 1-4).

The settings panel also includes a `Game Sound` toggle to mute/unmute audio.

## Build (Windows)

1) Install the .NET SDK 6.0 or newer.
2) For real builds against the game, make sure the references point at your
   Megabonk install directory.
3) Build the mod:
   ```powershell
   dotnet build .\MelonLoader\BonkUltraAlpha.csproj -c Release
   ```
4) The output DLL will be in:
   `.\MelonLoader\bin\Release\net6.0\BonkUltraAlpha.dll`

If you want to build without game assemblies (CI or local sanity builds), use:
```powershell
dotnet build .\MelonLoader\BonkUltraAlpha.csproj -c Release /p:UseStubs=true
```

## Install

Copy `BonkUltraAlpha.dll` into your MelonLoader `Mods` folder:

- `Z:\SteamLibrary\steamapps\common\Megabonk\Mods`
- `C:\Program Files\Megabonk\Mods`
- `C:\Program Files (x86)\Megabonk\Mods`

The mod logs a summary when it checks vendors. Look for `[BonkUltra]` lines in
the MelonLoader log.

## Bonk Ultra Auto Banish v0.0.2

This companion mod watches the skip-chest flow and automatically banishes
priority items when the animation is skipped. Build it with the same .NET SDK
and drop the resulting DLL into the same MelonLoader `Mods` folder:

```powershell
dotnet build .\MelonLoader\BonkUltraAutoBanish.csproj -c Release /p:UseStubs=true
Copy-Item .\MelonLoader\bin\Release\net6.0\BonkUltraAutoBanish.dll "<Megabonk>/Mods"
```

The mod only runs while `Skip Chest Animation` is enabled (the default
preference). If you ever need to see its decisions, the log watcher script now
mirrors both `[BonkUltra]` and `[BonkUltraAutoBanish]` lines by default
(`scripts\watch-bonk-ultra-logs.ps1` filter defaults to
`"[BonkUltra],[BonkUltraAutoBanish]"`). You can still override the `-Filter`
parameter if you want to focus on one prefix.

## Log workflow

Use the helper script to mirror filtered log lines into the repo so they are
easy to share:

```powershell
.\scripts\watch-bonk-ultra-logs.ps1
```

It writes `latest-bonk-ultra.log` in the repo root and keeps it updated with the
latest `[BonkUltra]` entries.

## Hang dumps

If the game stops responding for ~10 seconds, the mod writes a hang dump to:
`Z:\SteamLibrary\steamapps\common\Megabonk\UserData\BonkUltraAlpha`

Files are named `hang-dump-YYYYMMDD-HHMMSS-*.dmp` with a matching `.txt` status file.
