# Megabonk MoaiVendorCheck Mod (MelonLoader)

This MelonLoader mod checks the current instance for moai, rare vendor, and legendary vendor counts when a scene loads. If the instance has fewer than 3 moai, fewer than 2 rare vendors, and fewer than 1 legendary vendor, it holds `R` for 4 seconds and then presses `Escape` after a 2-second wait. The logic only runs in tier 2 or tier 3 instances.

## Windows setup & execution (MelonLoader)

> These steps assume Megabonk is a Unity title compatible with MelonLoader. Adjust paths to match your installation.

1. **Install MelonLoader**
   - Download MelonLoader for your game version from https://melonwiki.xyz/.
   - Run the installer and point it at your Megabonk install folder.
   - Launch the game once to let MelonLoader generate the `Mods` folder.

2. **Build the mod DLL**
   - Install the .NET SDK (6.0 or newer).
   - The project includes stubbed MelonLoader/UnityEngine types for CI builds. For real builds against the game:
     - Remove `MELONLOADER_STUBS` from `MelonLoader/MoaiVendorCheck.csproj`.
     - Add references to your MelonLoader and UnityEngine assemblies.
   - Build the project:
     ```powershell
     dotnet build .\MelonLoader\MoaiVendorCheck.csproj -c Release
     ```
   - The output DLL will be in:
     `.\MelonLoader\bin\Release\net6.0\MoaiVendorCheck.dll`

3. **Copy the mod DLL**
   - Copy `MoaiVendorCheck.dll` into the MelonLoader `Mods` directory:
     - `C:\Program Files\Megabonk\Mods`
     - `C:\Program Files (x86)\Megabonk\Mods`

4. **Start (or restart) Megabonk**
   - The mod should load automatically on game start.

5. **Verify the mod is active**
   - Load into a tier 2 or tier 3 instance and watch for the mod behavior:
     - If the instance has fewer than 3 moai, fewer than 2 rare vendors, and fewer than 1 legendary vendor, the mod will hold `R` for 4 seconds and then press `Escape` after a 2-second delay.

## Troubleshooting

- **Mod not loading**: confirm the DLL is in the `Mods` folder and that MelonLoader is installed correctly.
- **No effect in-game**: update the entity identifiers in `MelonLoader/MoaiVendorCheck.cs` if the game uses different tags or names.
- **Tier detection not working**: update `GameApi.TierManagerHints` and `GameApi.TierMemberNames` in `MelonLoader/MoaiVendorCheck.cs` to match the game's tier manager.
