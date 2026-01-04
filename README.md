# Bonk Ultra, Alpha (MelonLoader)

Version: 0.4.1

This mod checks run conditions after a run loads. If the minimum counts for
legendary vendors, epic vendors, moai, microwaves, or epic microwaves are not
met, it holds `R` for 3 seconds to restart. If all minimums are met, it presses
`ESC` once.

Legendary detection uses the `InteractableShadyGuy` vendor rarity tier (the
legendary hat color), aligned with what SeedInfoMod exposes.

## Pause menu controls

Open the pause menu or main menu and click the `Bonk Ultra` button (legendary
yellow) to open the settings panel. From there you can toggle auto-restart and
set the minimum counts for legendary vendors, epic vendors, moai, soul
harvesters, microwaves, epic microwaves, and green credit cards.

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
