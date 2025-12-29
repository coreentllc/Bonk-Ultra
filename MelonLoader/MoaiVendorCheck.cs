using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Megabonk.MoaiVendorCheckMod), "MoaiVendorCheck", "1.0.0", "OpenAI")]
[assembly: MelonGame(null, "Megabonk")]

namespace Megabonk
{
  public sealed class MoaiVendorCheckMod : MelonMod
  {
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      MelonCoroutines.Start(HandleInstanceLoaded());
    }

    private static IEnumerator HandleInstanceLoaded()
    {
      int? tier = GameApi.GetInstanceTier();
      if (tier != 2 && tier != 3)
      {
        yield break;
      }

      int moaiCount = GameApi.CountEntitiesByType("moai");
      int rareVendorCount = GameApi.CountEntitiesByType("rare_vendor");
      int legendaryVendorCount = GameApi.CountEntitiesByType("legendary_vendor");

      if (moaiCount < 3 && rareVendorCount < 2 && legendaryVendorCount < 1)
      {
        yield return InputApi.HoldKey("R", 4f);
        yield return TimerApi.WaitSeconds(2f);
        InputApi.KeyPress("Escape");
      }
    }
  }

  public static class GameApi
  {
    private static readonly string[] TierMemberNames = { "tier", "Tier", "currentTier", "CurrentTier" };
    private static readonly string[] TierManagerHints = { "Instance", "Run", "Tier", "GameManager" };

    public static int CountEntitiesByType(string entityType)
    {
      if (string.IsNullOrWhiteSpace(entityType))
      {
        return 0;
      }

      GameObject[] taggedEntities = Array.Empty<GameObject>();
      try
      {
        taggedEntities = GameObject.FindGameObjectsWithTag(entityType);
      }
      catch (UnityException)
      {
        taggedEntities = Array.Empty<GameObject>();
      }

      if (taggedEntities.Length > 0)
      {
        return taggedEntities.Length;
      }

      return UnityEngine.Object.FindObjectsOfType<GameObject>()
        .Count(obj => obj != null && obj.name.IndexOf(entityType, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public static int? GetInstanceTier()
    {
      foreach (var component in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
      {
        if (component == null)
        {
          continue;
        }

        var type = component.GetType();
        if (!TierManagerHints.Any(hint => type.Name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0))
        {
          continue;
        }

        int? tier = ReadTierValue(type, component);
        if (tier.HasValue)
        {
          return tier.Value;
        }
      }

      return null;
    }

    private static int? ReadTierValue(Type type, object instance)
    {
      foreach (string memberName in TierMemberNames)
      {
        var property = type.GetProperty(memberName);
        if (property != null && property.PropertyType == typeof(int))
        {
          return (int)property.GetValue(instance);
        }

        var field = type.GetField(memberName);
        if (field != null && field.FieldType == typeof(int))
        {
          return (int)field.GetValue(instance);
        }
      }

      return null;
    }
  }

  public static class TimerApi
  {
    public static IEnumerator WaitSeconds(float seconds)
    {
      yield return new WaitForSeconds(seconds);
    }
  }

  public static class InputApi
  {
    public static IEnumerator HoldKey(string key, float seconds)
    {
      KeyDown(key);
      yield return TimerApi.WaitSeconds(seconds);
      KeyUp(key);
    }

    public static void KeyPress(string key)
    {
      KeyDown(key);
      KeyUp(key);
    }

    public static void KeyDown(string key)
    {
      SendKey(key, true);
    }

    public static void KeyUp(string key)
    {
      SendKey(key, false);
    }

    private static void SendKey(string key, bool keyDown)
    {
      ushort virtualKey = key switch
      {
        "R" or "r" => 0x52,
        "Escape" or "Esc" or "ESC" => 0x1B,
        _ => throw new ArgumentException($"Unsupported key: {key}", nameof(key))
      };

      INPUT input = new INPUT
      {
        Type = InputTypeKeyboard,
        Data = new InputUnion
        {
          Keyboard = new KEYBDINPUT
          {
            Vk = virtualKey,
            Scan = 0,
            Flags = keyDown ? 0u : KeyEventKeyUp,
            Time = 0,
            ExtraInfo = IntPtr.Zero
          }
        }
      };

      SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
    }

    private const uint InputTypeKeyboard = 1;
    private const uint KeyEventKeyUp = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInput);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
      public uint Type;
      public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
      [FieldOffset(0)]
      public KEYBDINPUT Keyboard;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
      public ushort Vk;
      public ushort Scan;
      public uint Flags;
      public uint Time;
      public IntPtr ExtraInfo;
    }
  }
}
