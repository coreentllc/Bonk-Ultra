using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using Il2Cpp;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppAssets.Scripts.Managers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "3.1", "Strei")]
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
    private const int RestartAttempts = 3;
    private const float RestartRetryDelaySeconds = 1.0f;
    private const float EscTapSeconds = 0.05f;
    private const int DefaultMinLegendaryItems = 1;
    private const int MaxLegendaryItems = 9;
    private const float PauseUiPollSeconds = 0.5f;

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<int>? _prefMinLegendary;
    private static bool _settingsVisible;
    private static Rect _settingsRect = new Rect(20f, 120f, 280f, 200f);
    private static GUIStyle? _buttonStyle;
    private static GUIStyle? _smallButtonStyle;
    private static GUIStyle? _windowStyle;
    private static GUIStyle? _headerStyle;
    private static Texture2D? _legendaryTexture;
    private static Texture2D? _panelTexture;
    private static GameObject? _pauseUiCached;
    private static float _pauseUiLastCheck;

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAlpha", "Bonk Ultra");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-restart");
      _prefMinLegendary = _prefs.CreateEntry("MinLegendaryItems", DefaultMinLegendaryItems, "Minimum legendary items");
      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnGUI()
    {
      if (!IsPauseMenuOpen())
      {
        _settingsVisible = false;
        return;
      }

      EnsureGuiStyles();

      float buttonWidth = 200f;
      float buttonHeight = 32f;
      float buttonX = 20f;
      float buttonY = 80f;

      if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Bonk Ultra", _buttonStyle))
      {
        _settingsVisible = !_settingsVisible;
      }

      if (_settingsVisible)
      {
        _settingsRect = ClampWindowToScreen(_settingsRect);
        _settingsRect = GUI.Window(0xB0B0, _settingsRect, DrawSettingsWindow, "Bonk Ultra", _windowStyle);
      }
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      _runCheckToken++;
      MelonCoroutines.Start(EvaluateRun(_runCheckToken, "auto"));
    }

    private static bool SettingsEnabled => _prefEnabled?.Value ?? true;

    private static int MinLegendaryItems
      => Mathf.Clamp(_prefMinLegendary?.Value ?? DefaultMinLegendaryItems, 0, MaxLegendaryItems);

    private static bool IsPauseMenuOpen()
    {
      float now = Time.realtimeSinceStartup;
      if (_pauseUiCached == null || now - _pauseUiLastCheck > PauseUiPollSeconds)
      {
        _pauseUiCached = FindPauseUi();
        _pauseUiLastCheck = now;
      }

      return _pauseUiCached != null && _pauseUiCached.activeInHierarchy;
    }

    private static GameObject? FindPauseUi()
    {
      GameObject obj = GameObject.Find("PauseUI");
      if (obj != null)
      {
        return obj;
      }

      return GameObject.Find("PauseMenu");
    }

    private static void EnsureGuiStyles()
    {
      if (_legendaryTexture == null)
      {
        _legendaryTexture = CreateColorTexture(LegendaryYellow);
      }

      if (_panelTexture == null)
      {
        _panelTexture = CreateColorTexture(PanelBackground);
      }

      if (_buttonStyle == null)
      {
        _buttonStyle = new GUIStyle(GUI.skin.button)
        {
          alignment = TextAnchor.MiddleCenter,
          fontStyle = FontStyle.Bold
        };
        _buttonStyle.normal.background = _legendaryTexture;
        _buttonStyle.hover.background = _legendaryTexture;
        _buttonStyle.active.background = _legendaryTexture;
        _buttonStyle.focused.background = _legendaryTexture;
        _buttonStyle.normal.textColor = LegendaryText;
        _buttonStyle.hover.textColor = LegendaryText;
        _buttonStyle.active.textColor = LegendaryText;
        _buttonStyle.focused.textColor = LegendaryText;
      }

      if (_smallButtonStyle == null && _buttonStyle != null)
      {
        _smallButtonStyle = new GUIStyle(_buttonStyle)
        {
          fontSize = 14
        };
        _smallButtonStyle.margin = new RectOffset(2, 2, 2, 2);
        _smallButtonStyle.padding = new RectOffset(4, 4, 4, 4);
      }

      if (_windowStyle == null)
      {
        _windowStyle = new GUIStyle(GUI.skin.window);
        _windowStyle.normal.background = _panelTexture;
        _windowStyle.onNormal.background = _panelTexture;
        _windowStyle.normal.textColor = Color.white;
      }

      if (_headerStyle == null)
      {
        _headerStyle = new GUIStyle(GUI.skin.label)
        {
          fontStyle = FontStyle.Bold
        };
        _headerStyle.normal.textColor = Color.white;
      }
    }

    private static Texture2D CreateColorTexture(Color color)
    {
      var texture = new Texture2D(1, 1);
      texture.SetPixel(0, 0, color);
      texture.Apply();
      texture.hideFlags = HideFlags.DontSave;
      return texture;
    }

    private static Rect ClampWindowToScreen(Rect rect)
    {
      float maxX = Mathf.Max(0f, Screen.width - rect.width);
      float maxY = Mathf.Max(0f, Screen.height - rect.height);
      rect.x = Mathf.Clamp(rect.x, 0f, maxX);
      rect.y = Mathf.Clamp(rect.y, 0f, maxY);
      return rect;
    }

    private void DrawSettingsWindow(int id)
    {
      GUILayout.Space(4f);
      GUILayout.Label("Auto-restart conditions", _headerStyle);

      bool enabled = SettingsEnabled;
      string toggleLabel = enabled ? "Auto-Restart: ON" : "Auto-Restart: OFF";
      if (_buttonStyle != null && GUILayout.Button(toggleLabel, _buttonStyle))
      {
        if (_prefEnabled != null)
        {
          _prefEnabled.Value = !enabled;
          MelonPreferences.Save();
        }
      }

      GUILayout.Space(6f);
      int minLegendary = MinLegendaryItems;
      GUILayout.Label("Minimum legendary items", _headerStyle);
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      if (_smallButtonStyle != null && GUILayout.Button("-", _smallButtonStyle, GUILayout.Width(32f), GUILayout.Height(26f)))
      {
        int nextValue = Mathf.Clamp(minLegendary - 1, 0, MaxLegendaryItems);
        if (nextValue != minLegendary && _prefMinLegendary != null)
        {
          _prefMinLegendary.Value = nextValue;
          MelonPreferences.Save();
        }
      }

      GUILayout.Space(6f);
      GUILayout.Label(minLegendary.ToString(), _headerStyle);
      GUILayout.Space(6f);

      if (_smallButtonStyle != null && GUILayout.Button("+", _smallButtonStyle, GUILayout.Width(32f), GUILayout.Height(26f)))
      {
        int nextValue = Mathf.Clamp(minLegendary + 1, 0, MaxLegendaryItems);
        if (nextValue != minLegendary && _prefMinLegendary != null)
        {
          _prefMinLegendary.Value = nextValue;
          MelonPreferences.Save();
        }
      }
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      GUILayout.Space(4f);
      GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
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

      if (!SettingsEnabled)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) settings disabled; skipping.");
        yield break;
      }

      int minLegendary = MinLegendaryItems;
      bool hasLegendary = scan.LegendaryItemCount >= minLegendary;

      MelonLogger.Msg(
        $"{LogPrefix} ({reason}) vendors={scan.VendorCount} done={scan.DoneVendorCount} " +
        $"vendorTierLegendary={scan.LegendaryVendorTierCount} items={scan.ItemCount} legendaryItems={scan.LegendaryItemCount} " +
        $"minLegendary={minLegendary}");

      if (hasLegendary)
      {
        if (GameApi.TryOpenPauseMenu(out string pauseSource))
        {
          MelonLogger.Msg($"{LogPrefix} Legendary vendor item found. Opened pause menu via {pauseSource}.");
        }
        else
        {
          MelonLogger.Msg($"{LogPrefix} Legendary vendor item found. Pressing ESC.");
          yield return InputApi.PressKey("ESC", EscTapSeconds);
        }
        yield break;
      }

      MelonLogger.Msg($"{LogPrefix} No legendary vendor items found. Restarting run.");
      yield return RestartRun();
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

    private IEnumerator RestartRun()
    {
      int? seed = GameApi.GetMapSeed();
      bool canVerify = seed.HasValue && seed.Value != 0;
      if (!canVerify)
      {
        MelonLogger.Msg($"{LogPrefix} Seed unavailable; restart verification disabled.");
      }

      if (GameApi.TryInvokeRestartRun(out string invoked))
      {
        MelonLogger.Msg($"{LogPrefix} Invoked restart method {invoked}.");
        yield return TimerApi.WaitSeconds(RestartRetryDelaySeconds);
        if (GameApi.HasSeedChanged(seed))
        {
          MelonLogger.Msg($"{LogPrefix} Restart confirmed by seed change.");
          yield break;
        }
      }

      for (int attempt = 1; attempt <= RestartAttempts; attempt++)
      {
        if (GameApi.IsMainMenu())
        {
          yield break;
        }

        MelonLogger.Msg($"{LogPrefix} Restart attempt {attempt}/{RestartAttempts} (holding R).");
        yield return InputApi.HoldKey("R", RestartHoldSeconds);
        yield return TimerApi.WaitSeconds(RestartRetryDelaySeconds);

        if (GameApi.HasSeedChanged(seed))
        {
          MelonLogger.Msg($"{LogPrefix} Restart confirmed by seed change.");
          yield break;
        }
      }

      if (canVerify)
      {
        MelonLogger.Msg($"{LogPrefix} Restart attempts exhausted; seed did not change.");
      }
      else
      {
        MelonLogger.Msg($"{LogPrefix} Restart attempts exhausted; unable to verify seed.");
      }
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
    private static bool _loggedRestartMethod;
    private static bool _loggedPauseMethod;

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

    public static bool TryOpenPauseMenu(out string invoked)
    {
      string[] methodNames = { "OpenPauseMenu", "TogglePauseMenu", "OpenPause", "TogglePause", "Pause", "ShowPauseMenu" };

      if (TryInvokeMethod(typeof(MapController), null, methodNames, out invoked))
      {
        return true;
      }

      object? mapInstance = TryGetMapControllerInstance();
      if (mapInstance != null && TryInvokeMethod(mapInstance.GetType(), mapInstance, methodNames, out invoked))
      {
        return true;
      }

      if (TryActivatePauseUi(out invoked))
      {
        return true;
      }

      if (!_loggedPauseMethod)
      {
        MelonLogger.Msg("[BonkUltra] Pause menu not found; falling back to ESC input.");
        _loggedPauseMethod = true;
      }

      invoked = string.Empty;
      return false;
    }

    public static bool TryInvokeRestartRun(out string invoked)
    {
      string[] methodNames = { "RestartRun", "Restart", "RestartGame", "RestartMap", "RestartLevel", "ResetRun" };

      if (TryInvokeMethod(typeof(MapController), null, methodNames, out invoked))
      {
        return true;
      }

      object? instance = TryGetMapControllerInstance();
      if (instance != null && TryInvokeMethod(instance.GetType(), instance, methodNames, out invoked))
      {
        return true;
      }

      if (!_loggedRestartMethod)
      {
        MelonLogger.Msg("[BonkUltra] No restart method found on MapController; falling back to R input.");
        _loggedRestartMethod = true;
      }

      invoked = string.Empty;
      return false;
    }

    public static int? GetMapSeed()
    {
      try
      {
        int seed = MapGenerationController.mapSeed;
        if (seed != 0)
        {
          return seed;
        }
      }
      catch (Exception)
      {
      }

      try
      {
        int seed = MapGenerator.seed;
        if (seed != 0)
        {
          return seed;
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    public static bool HasSeedChanged(int? previousSeed)
    {
      if (!previousSeed.HasValue || previousSeed.Value == 0)
      {
        return false;
      }

      int? currentSeed = GetMapSeed();
      return currentSeed.HasValue && currentSeed.Value != 0 && currentSeed.Value != previousSeed.Value;
    }

    private static bool TryInvokeMethod(Type type, object? instance, string[] methodNames, out string invoked)
    {
      invoked = string.Empty;
      if (methodNames.Length == 0)
      {
        return false;
      }

      const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

      foreach (string name in methodNames)
      {
        var method = type.GetMethod(name, Flags);
        if (method == null || method.GetParameters().Length != 0)
        {
          continue;
        }

        if (!method.IsStatic && instance == null)
        {
          continue;
        }

        try
        {
          method.Invoke(method.IsStatic ? null : instance, null);
          invoked = $"{type.Name}.{method.Name}()";
          return true;
        }
        catch (Exception ex)
        {
          MelonLogger.Msg($"[BonkUltra] Method {type.Name}.{method.Name} failed: {ex.GetType().Name} {ex.Message}");
        }
      }

      return false;
    }

    private static bool TryActivatePauseUi(out string invoked)
    {
      invoked = string.Empty;

      try
      {
        string[] pauseNames = { "PauseUI", "PauseMenu" };
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
          if (obj == null)
          {
            continue;
          }

          if (!IsSceneObject(obj))
          {
            continue;
          }

          foreach (string name in pauseNames)
          {
            if (!obj.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
              continue;
            }

            if (!obj.activeSelf)
            {
              obj.SetActive(true);
            }

            invoked = $"GameObject.SetActive(true) {obj.name}";
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        MelonLogger.Msg($"[BonkUltra] Pause object scan failed: {ex.GetType().Name} {ex.Message}");
      }

      return false;
    }

    private static bool IsSceneObject(GameObject obj)
    {
      try
      {
        return obj.scene.IsValid() && obj.scene.isLoaded;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static object? TryGetMapControllerInstance()
    {
      try
      {
        var type = typeof(MapController);
        string[] memberNames = { "Instance", "instance", "_instance", "s_instance", "Singleton", "Current", "current" };

        foreach (string name in memberNames)
        {
          var property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
          if (property != null && type.IsAssignableFrom(property.PropertyType))
          {
            object? value = property.GetValue(null);
            if (value != null)
            {
              return value;
            }
          }
        }

        foreach (string name in memberNames)
        {
          var field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
          if (field != null && type.IsAssignableFrom(field.FieldType))
          {
            object? value = field.GetValue(null);
            if (value != null)
            {
              return value;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      return null;
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
