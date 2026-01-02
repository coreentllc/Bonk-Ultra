using System;
using System.Collections;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using Il2Cpp;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppAssets.Scripts.Managers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "0.1.0", "Strei")]
[assembly: MelonGame(null, "Megabonk")]

namespace BonkUltraAlpha
{
  public sealed class BonkUltraAlphaMod : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private const string LogPrefix = "[BonkUltra]";
    private const float LoadPollIntervalSeconds = 0.5f;
    private const float PostLoadDelaySeconds = 0.5f;
    private const float MaxLoadWaitSeconds = 30f;
    private const float VendorPollIntervalSeconds = 0.5f;
    private const float VendorPollTimeoutSeconds = 20f;
    private const float RestartHoldSeconds = 3.0f;
    private const float EscTapSeconds = 0.05f;

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      _runCheckToken++;
      MelonCoroutines.Start(EvaluateRun(_runCheckToken, "auto"));
    }

    private IEnumerator EvaluateRun(int token, string reason)
    {
      yield return WaitForLoadReady();
      if (token != _runCheckToken)
      {
        yield break;
      }

      if (GameApi.IsMainMenu())
      {
        yield break;
      }

      VendorScanResult scan = default;
      float waited = 0f;
      while (waited < VendorPollTimeoutSeconds)
      {
        if (token != _runCheckToken || GameApi.IsMainMenu())
        {
          yield break;
        }

        scan = VendorScanner.Scan();
        if (scan.VendorCount > 0 && scan.ItemCount > 0)
        {
          break;
        }

        yield return TimerApi.WaitSeconds(VendorPollIntervalSeconds);
        waited += VendorPollIntervalSeconds;
      }

      if (scan.VendorCount == 0)
      {
        scan = VendorScanner.Scan();
      }

      bool hasLegendary = scan.LegendaryItemCount > 0;

      MelonLogger.Msg(
        $"{LogPrefix} ({reason}) vendors={scan.VendorCount} done={scan.DoneVendorCount} " +
        $"vendorTierLegendary={scan.LegendaryVendorTierCount} items={scan.ItemCount} legendaryItems={scan.LegendaryItemCount}");

      if (hasLegendary)
      {
        MelonLogger.Msg($"{LogPrefix} Legendary vendor item found. Pressing ESC.");
        yield return InputApi.PressKey("ESC", EscTapSeconds);
        yield break;
      }

      MelonLogger.Msg($"{LogPrefix} No legendary vendor items found. Holding R to restart.");
      yield return InputApi.HoldKey("R", RestartHoldSeconds);
    }

    private static IEnumerator WaitForLoadReady()
    {
      float waited = 0f;
      while (waited < MaxLoadWaitSeconds)
      {
        if (GameApi.IsMainMenu())
        {
          yield break;
        }

        if (GameObject.Find("HUD") != null)
        {
          yield return TimerApi.WaitSeconds(PostLoadDelaySeconds);
          yield break;
        }

        yield return TimerApi.WaitSeconds(LoadPollIntervalSeconds);
        waited += LoadPollIntervalSeconds;
      }

      MelonLogger.Msg($"{LogPrefix} Load wait timed out; continuing.");
    }

    private static class VendorScanner
    {
      private static bool _loggedFailure;

      public static VendorScanResult Scan()
      {
        var result = new VendorScanResult();

        try
        {
          Il2CppArrayBase<InteractableShadyGuy> vendors = UnityEngine.Object.FindObjectsOfType<InteractableShadyGuy>();
          foreach (var vendor in vendors)
          {
            if (vendor == null)
            {
              continue;
            }

            result.VendorCount++;

            if (vendor.done)
            {
              result.DoneVendorCount++;
              continue;
            }

            if (vendor.rarity == EItemRarity.Legendary)
            {
              result.LegendaryVendorTierCount++;
            }

            if (vendor.items == null)
            {
              continue;
            }

            var enumerator = vendor.items.GetEnumerator();
            while (enumerator.MoveNext())
            {
              ItemData item = enumerator.Current;
              if (item == null)
              {
                continue;
              }

              result.ItemCount++;
              if (item.rarity == EItemRarity.Legendary)
              {
                result.LegendaryItemCount++;
              }
            }
          }
        }
        catch (Exception ex)
        {
          if (!_loggedFailure)
          {
            MelonLogger.Msg($"{LogPrefix} Vendor scan failed: {ex.GetType().Name} {ex.Message}");
            _loggedFailure = true;
          }
        }

        return result;
      }

    }

    private struct VendorScanResult
    {
      public int VendorCount;
      public int DoneVendorCount;
      public int LegendaryVendorTierCount;
      public int ItemCount;
      public int LegendaryItemCount;
    }
#endif
  }

#if !MELONLOADER_STUBS
  internal static class GameApi
  {
    public static bool IsMainMenu()
    {
      try
      {
        return MapController.IsMainMenu();
      }
      catch (Exception)
      {
        return false;
      }
    }
  }

  public static class InputApi
  {
    private const uint InputKeyboard = 1;
    private const uint KeyEventKeyUp = 0x0002;

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
      public uint type;
      public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
      [FieldOffset(0)]
      public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
      public ushort wVk;
      public ushort wScan;
      public uint dwFlags;
      public uint time;
      public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    public static IEnumerator HoldKey(string keyName, float seconds)
    {
      if (!TryGetVirtualKey(keyName, out ushort vk))
      {
        MelonLogger.Msg("[BonkUltra] Unable to map key '" + keyName + "'.");
        yield break;
      }

      SendKey(vk, false);
      yield return TimerApi.WaitSeconds(seconds);
      SendKey(vk, true);
    }

    public static IEnumerator PressKey(string keyName, float seconds)
    {
      if (!TryGetVirtualKey(keyName, out ushort vk))
      {
        MelonLogger.Msg("[BonkUltra] Unable to map key '" + keyName + "'.");
        yield break;
      }

      SendKey(vk, false);
      yield return TimerApi.WaitSeconds(seconds);
      SendKey(vk, true);
    }

    private static void SendKey(ushort vk, bool keyUp)
    {
      var input = new INPUT
      {
        type = InputKeyboard,
        U = new InputUnion
        {
          ki = new KEYBDINPUT
          {
            wVk = vk,
            wScan = 0,
            dwFlags = keyUp ? KeyEventKeyUp : 0,
            time = 0,
            dwExtraInfo = IntPtr.Zero
          }
        }
      };

      SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
    }

    private static bool TryGetVirtualKey(string keyName, out ushort vk)
    {
      vk = 0;
      if (string.IsNullOrWhiteSpace(keyName))
      {
        return false;
      }

      string trimmed = keyName.Trim();
      if (trimmed.Length == 1)
      {
        char keyChar = char.ToUpperInvariant(trimmed[0]);
        if ((keyChar >= 'A' && keyChar <= 'Z') || (keyChar >= '0' && keyChar <= '9'))
        {
          vk = keyChar;
          return true;
        }
      }

      switch (trimmed.ToUpperInvariant())
      {
        case "ENTER":
          vk = 0x0D;
          return true;
        case "SPACE":
          vk = 0x20;
          return true;
        case "TAB":
          vk = 0x09;
          return true;
        case "ESC":
        case "ESCAPE":
          vk = 0x1B;
          return true;
        default:
          return false;
      }
    }
  }
#endif

  public static class TimerApi
  {
    public static IEnumerator WaitSeconds(float seconds)
    {
      yield return new WaitForSeconds(seconds);
    }
  }
}
