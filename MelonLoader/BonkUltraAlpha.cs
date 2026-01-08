using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using Il2Cpp;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Interactables;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppAssets.Scripts.Managers;
using Il2CppAssets.Scripts.Saves___Serialization.Progression.Achievements;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#endif

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "0.4.3", "Strei")]
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
    private const float PostLoadDelaySeconds = 0.0f;
    private const float MaxLoadWaitSeconds = 30f;
    private const float RestartHudTimeoutSeconds = 10f;
    private const float VendorPollIntervalSeconds = 0.1f;
    private const float VendorPollTimeoutSeconds = 20f;
    private const float ItemWaitTimeoutSeconds = 2.0f;
    private const float RestartHoldSeconds = 3.0f;
    private const int RestartAttempts = 3;
    private const float RestartRetryDelaySeconds = 0.1f;
    private const float MinRestartIntervalSeconds = 0.4f;
    private const float RestartStreakResetSeconds = 5f;
    private const int RestartStreakPauseCount = 200;
    private const float RestartStreakPauseSeconds = 5f;
    private const float UiTransitionGuardSeconds = 1.0f;
    private const float SceneTransitionGuardSeconds = 1.5f;
    private const float RestartSafetyMaxWaitSeconds = 3.0f;
    private const float RestartSafetyPollSeconds = 0.1f;
    private const float EscTapSeconds = 0.05f;
    private const int ConditionOneMinSoulHarvesters = 1;
    private const int ConditionOneMinGreenCreditCards = 1;
    private const int ConditionTwoMinMoai = 6;
    private const int ConditionThreeMinLegendaryVendors = 2;
    private const int ConditionThreeMinEpicVendors = 1;
    private const int ConditionFourMinGreenCreditCards = 2;
    private const int ConditionFourMinMicrowaves = 1;
    private const int ConditionFourMinEpicMicrowaves = 1;
    private const int DefaultMainMenuButtonX = 20;
    private const int DefaultMainMenuButtonY = 80;
    private const int DefaultSettingsMenuX = 680;
    private const int DefaultSettingsMenuY = 270;
    private const float PauseUiPollSeconds = 0.5f;
    private const int BreadcrumbMaxLines = 50;
    private const float ScanHeartbeatIntervalSeconds = 2f;
    private const float ScanWatchdogTimeoutSeconds = 6f;
      private const float HangDumpTimeoutSeconds = 10f;
      private const int ConditionFourFailureThreshold = 5;
      private const float ConditionFourCooldownSeconds = 15f;

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<bool>? _prefSoundEnabled;
    private static MelonPreferences_Entry<bool>? _prefConditionOneEnabled;
    private static MelonPreferences_Entry<bool>? _prefConditionTwoEnabled;
    private static MelonPreferences_Entry<bool>? _prefConditionThreeEnabled;
    private static MelonPreferences_Entry<bool>? _prefConditionFourEnabled;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonX;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonY;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuX;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuY;
    private static MelonPreferences_Entry<string>? _prefLastBreadcrumb;
    private static float _soundOnVolume = -1f;
    private static string? _breadcrumbFilePath;
    private static int _breadcrumbSequence;
    private static int _restartSequence;
    private static int _restartStreakCount;
    private static float _restartStreakStartAt;
    private static float _restartStreakLastAt;
    private static bool _breadcrumbFailureLogged;
    private static string _lastBreadcrumbReason = "auto";
    private static VendorScanResult _lastBreadcrumbScan;
    private static float _lastRestartIssuedAt;
    private static bool _restartInProgress;
    private static float _restartStartTime;
    private static bool _restartFallbackAttempted;
    private static bool _restartDisabledForTimeout;
    private static float _restartHeartbeatAt;
    private static float _lastScanAt;
    private static float _scanHeartbeatAt;
    private static bool _scanWatchdogLogged;
    private static Thread? _hangWatchdogThread;
    private static bool _hangWatchdogRunning;
    private static bool _hangDumpTaken;
    private static long _mainThreadHeartbeatTicks;
    private static bool _settingsVisible;
    private static Rect _settingsRect = new Rect(20f, 120f, 320f, 360f);
    private static GUIStyle? _buttonStyle;
    private static GUIStyle? _windowStyle;
    private static GUIStyle? _headerStyle;
    private static Texture2D? _legendaryTexture;
    private static Texture2D? _panelTexture;
    private static GameObject? _pauseUiCached;
    private static float _pauseUiLastCheck;
    private static bool _lastPauseMenuState;
    private static float _pauseMenuStateAt;
    private static float _lastSceneTransitionAt;
    private static string? _lastSceneName;
    private static int _lastSceneCount;
    private static GameObject? _uiBlocker;
    private static float _lastHudReadyTime;
    private static int _conditionFourFailureCount;
    private static float _conditionFourCooldownUntil;

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAlpha", "Bonk Ultra");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-restart");
      _prefSoundEnabled = _prefs.CreateEntry("SoundEnabled", true, "Game sound on/off");
      _prefConditionOneEnabled = _prefs.CreateEntry("ConditionOneEnabled", true, "Enable condition one");
      _prefConditionTwoEnabled = _prefs.CreateEntry("ConditionTwoEnabled", true, "Enable condition two");
      _prefConditionThreeEnabled = _prefs.CreateEntry("ConditionThreeEnabled", true, "Enable condition three");
      _prefConditionFourEnabled = _prefs.CreateEntry("ConditionFourEnabled", true, "Enable condition four");
      _prefMainMenuButtonX = _prefs.CreateEntry("MainMenuButtonX", DefaultMainMenuButtonX, "Main menu button X");
      _prefMainMenuButtonY = _prefs.CreateEntry("MainMenuButtonY", DefaultMainMenuButtonY, "Main menu button Y");
      _prefSettingsMenuX = _prefs.CreateEntry("SettingsMenuX", DefaultSettingsMenuX, "Settings menu X");
      _prefSettingsMenuY = _prefs.CreateEntry("SettingsMenuY", DefaultSettingsMenuY, "Settings menu Y");
      _prefLastBreadcrumb = _prefs.CreateEntry("LastBreadcrumb", string.Empty, "Last crash breadcrumb");
      try
      {
        string? userDir = ResolveUserDataDirectory();
        if (!string.IsNullOrWhiteSpace(userDir))
        {
          string breadcrumbDir = Path.Combine(userDir, "BonkUltraAlpha");
          Directory.CreateDirectory(breadcrumbDir);
          _breadcrumbFilePath = Path.Combine(breadcrumbDir, "last-breadcrumb.txt");
        }
      }
      catch (Exception)
      {
        _breadcrumbFilePath = null;
      }
      _mainThreadHeartbeatTicks = Stopwatch.GetTimestamp();
      StartHangWatchdog();
      ApplySoundPreference();
      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnGUI()
    {
      bool pauseOpen = IsPauseMenuOpen();
      bool mainMenuOpen = GameApi.IsMainMenu();
      if (!pauseOpen && !mainMenuOpen)
      {
        _settingsVisible = false;
        SetUiBlockerActive(false);
        return;
      }

      EnsureGuiStyles();

      float buttonWidth = 200f;
      float buttonHeight = 32f;
      float buttonX = 20f;
      float buttonY = 80f;

      if (mainMenuOpen)
      {
        buttonX = MainMenuButtonX;
        buttonY = MainMenuButtonY;
      }

      ClampButtonToScreen(ref buttonX, ref buttonY, buttonWidth, buttonHeight);

      if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "Bonk Ultra", _buttonStyle))
      {
        _settingsVisible = !_settingsVisible;
      }

      SetUiBlockerActive(_settingsVisible);

      if (_settingsVisible)
      {
        _settingsRect.x = SettingsMenuX;
        _settingsRect.y = SettingsMenuY;
        _settingsRect = ClampWindowToScreen(_settingsRect);
        DrawSettingsPanel(_settingsRect);
      }
    }

    public override void OnUpdate()
    {
      _mainThreadHeartbeatTicks = Stopwatch.GetTimestamp();
      UpdateSceneTransitionState();
      if (!SettingsEnabled || GameApi.IsMainMenu() || IsPauseMenuOpen())
      {
        return;
      }

      if (_lastScanAt <= 0f)
      {
        return;
      }

      float now = Time.realtimeSinceStartup;
      float sinceScan = now - _lastScanAt;
      if (!_scanWatchdogLogged && sinceScan >= ScanWatchdogTimeoutSeconds)
      {
        _scanWatchdogLogged = true;
        MelonLogger.Msg($"{LogPrefix} Scan watchdog timeout after {sinceScan:0.0}s.");
        WriteBreadcrumb("scan-timeout", "watchdog", _lastBreadcrumbScan);
      }
    }

    public override void OnDeinitializeMelon()
    {
      _hangWatchdogRunning = false;
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

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      _runCheckToken++;
      ApplySoundPreference();
      MelonCoroutines.Start(EvaluateRun(_runCheckToken, "auto"));
    }

    private static bool SettingsEnabled => _prefEnabled?.Value ?? true;

    private static bool EvaluateConditions(VendorScanResult scan, out bool conditionOne, out bool conditionTwo,
      out bool conditionThree, out bool conditionFour)
    {
      bool conditionOneEnabled = ConditionOneEnabled;
      bool conditionTwoEnabled = ConditionTwoEnabled;
      bool conditionThreeEnabled = ConditionThreeEnabled;
      bool conditionFourEnabled = ConditionFourEnabled;
      conditionOne = conditionOneEnabled
        && scan.SoulHarvesterCount >= ConditionOneMinSoulHarvesters
        && scan.GreenCreditCardCount >= ConditionOneMinGreenCreditCards;
      conditionTwo = conditionTwoEnabled
        && scan.MoaiCount >= ConditionTwoMinMoai;
      conditionThree = conditionThreeEnabled
        && scan.LegendaryVendorTierCount >= ConditionThreeMinLegendaryVendors
        && scan.EpicVendorTierCount >= ConditionThreeMinEpicVendors;
      conditionFour = conditionFourEnabled
        && scan.GreenCreditCardCount >= ConditionFourMinGreenCreditCards
        && scan.MicrowaveCount >= ConditionFourMinMicrowaves
        && scan.EpicMicrowaveCount >= ConditionFourMinEpicMicrowaves;
      return conditionOne || conditionTwo || conditionThree || conditionFour;
    }

    private static bool ConditionFourItemsSatisfied(VendorScanResult scan)
    {
      return scan.GreenCreditCardCount >= ConditionFourMinGreenCreditCards
        && scan.MicrowaveCount >= ConditionFourMinMicrowaves
        && scan.EpicMicrowaveCount >= ConditionFourMinEpicMicrowaves;
    }

    private static bool TryDescribeConditionFourMissing(VendorScanResult scan, out string reason)
    {
      var parts = new List<string>();
      if (scan.GreenCreditCardCount < ConditionFourMinGreenCreditCards)
      {
        parts.Add($"greenCards<{ConditionFourMinGreenCreditCards}");
      }

      if (scan.MicrowaveCount < ConditionFourMinMicrowaves)
      {
        parts.Add($"microwaves<{ConditionFourMinMicrowaves}");
      }

      if (scan.EpicMicrowaveCount < ConditionFourMinEpicMicrowaves)
      {
        parts.Add($"epicMicrowaves<{ConditionFourMinEpicMicrowaves}");
      }

      reason = string.Join(",", parts);
      return parts.Count > 0;
    }

    private static int MainMenuButtonX
      => Mathf.Clamp(_prefMainMenuButtonX?.Value ?? DefaultMainMenuButtonX, 0, 10000);

    private static int MainMenuButtonY
      => Mathf.Clamp(_prefMainMenuButtonY?.Value ?? DefaultMainMenuButtonY, 0, 10000);

    private static int SettingsMenuX
      => Mathf.Clamp(_prefSettingsMenuX?.Value ?? DefaultSettingsMenuX, 0, 10000);

    private static int SettingsMenuY
      => Mathf.Clamp(_prefSettingsMenuY?.Value ?? DefaultSettingsMenuY, 0, 10000);

    private static bool ConditionOneEnabled => _prefConditionOneEnabled?.Value ?? true;
    private static bool ConditionTwoEnabled => _prefConditionTwoEnabled?.Value ?? true;
    private static bool ConditionThreeEnabled => _prefConditionThreeEnabled?.Value ?? true;
    private static bool ConditionFourEnabled => _prefConditionFourEnabled?.Value ?? true;

    private static bool SoundEnabled => _prefSoundEnabled?.Value ?? true;

    private static void ApplySoundPreference()
    {
      bool enabled = SoundEnabled;
      if (enabled)
      {
        if (_soundOnVolume > 0f)
        {
          AudioListener.volume = _soundOnVolume;
        }
        else
        {
          AudioListener.volume = 1f;
        }

        return;
      }

      if (_soundOnVolume < 0f)
      {
        float current = AudioListener.volume;
        _soundOnVolume = current > 0f ? current : 1f;
      }

      AudioListener.volume = 0f;
    }

    private static bool HasAnyScanData(VendorScanResult scan)
    {
      return scan.VendorCount > 0 || scan.MicrowaveCount > 0 || scan.MoaiCount > 0;
    }

    private static bool IsPauseMenuOpen()
    {
      float now = Time.realtimeSinceStartup;
      if (_pauseUiCached == null || now - _pauseUiLastCheck > PauseUiPollSeconds)
      {
        _pauseUiCached = FindPauseUi();
        _pauseUiLastCheck = now;
      }

      bool isOpen = _pauseUiCached != null && _pauseUiCached.activeInHierarchy;
      if (isOpen != _lastPauseMenuState)
      {
        _lastPauseMenuState = isOpen;
        _pauseMenuStateAt = now;
      }

      return isOpen;
    }

    private static void UpdateSceneTransitionState()
    {
      try
      {
        Scene active = SceneManager.GetActiveScene();
        string name = active.name ?? string.Empty;
        int count = SceneManager.sceneCount;
        if (count != _lastSceneCount || !string.Equals(name, _lastSceneName, StringComparison.Ordinal))
        {
          _lastSceneCount = count;
          _lastSceneName = name;
          _lastSceneTransitionAt = Time.realtimeSinceStartup;
        }
      }
      catch (Exception)
      {
      }
    }

    private static bool IsUiTransitioning()
    {
      if (_pauseMenuStateAt <= 0f)
      {
        return false;
      }

      return Time.realtimeSinceStartup - _pauseMenuStateAt < UiTransitionGuardSeconds;
    }

    private static bool IsSceneTransitioning()
    {
      if (_lastSceneTransitionAt <= 0f)
      {
        return false;
      }

      return Time.realtimeSinceStartup - _lastSceneTransitionAt < SceneTransitionGuardSeconds;
    }

    private static IEnumerator WaitForSafeRestartWindow(string reason)
    {
      float start = Time.realtimeSinceStartup;
      UpdateSceneTransitionState();
      IsPauseMenuOpen();
      while (Time.realtimeSinceStartup - start < RestartSafetyMaxWaitSeconds)
      {
        if (GameApi.IsMainMenu())
        {
          yield break;
        }

        UpdateSceneTransitionState();
        IsPauseMenuOpen();
        if (!IsUiTransitioning() && !IsSceneTransitioning())
        {
          yield break;
        }

        yield return TimerApi.WaitSeconds(RestartSafetyPollSeconds);
      }

      MelonLogger.Msg($"{LogPrefix} ({reason}) Restart safety window timed out; proceeding.");
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

    private static void SetUiBlockerActive(bool active)
    {
      EnsureUiBlocker();
      if (_uiBlocker == null)
      {
        return;
      }

      if (_uiBlocker.activeSelf != active)
      {
        _uiBlocker.SetActive(active);
      }
    }

    private static void EnsureUiBlocker()
    {
      if (_uiBlocker != null)
      {
        return;
      }

      var blocker = new GameObject("BonkUltraUiBlocker");
      blocker.hideFlags = HideFlags.DontSave;
      UnityEngine.Object.DontDestroyOnLoad(blocker);

      var canvas = blocker.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.ScreenSpaceOverlay;
      canvas.sortingOrder = 5000;
      blocker.AddComponent<GraphicRaycaster>();

      var group = blocker.AddComponent<CanvasGroup>();
      group.blocksRaycasts = true;
      group.interactable = true;
      group.ignoreParentGroups = true;

      var imageObj = new GameObject("BlockerImage");
      imageObj.hideFlags = HideFlags.DontSave;
      imageObj.transform.SetParent(blocker.transform, false);
      var image = imageObj.AddComponent<Image>();
      image.color = new Color(0f, 0f, 0f, 0f);
      image.raycastTarget = true;

      var rectTransform = imageObj.GetComponent<RectTransform>();
      rectTransform.anchorMin = Vector2.zero;
      rectTransform.anchorMax = Vector2.one;
      rectTransform.offsetMin = Vector2.zero;
      rectTransform.offsetMax = Vector2.zero;

      blocker.SetActive(false);
      _uiBlocker = blocker;
    }

    private static void LogItemSpawnTiming(string reason, float hudReadyTime)
    {
      float now = Time.realtimeSinceStartup;
      if (hudReadyTime <= 0f)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) Items detected (HUD time unknown).");
        return;
      }

      float elapsed = Mathf.Max(0f, now - hudReadyTime);
      MelonLogger.Msg($"{LogPrefix} ({reason}) Items detected after {elapsed:0.00}s from HUD.");
    }

    private static void RecordScanHeartbeat(string reason, VendorScanResult scan)
    {
      float now = Time.realtimeSinceStartup;
      _lastScanAt = now;
      _scanWatchdogLogged = false;

      if (now - _scanHeartbeatAt >= ScanHeartbeatIntervalSeconds)
      {
        _scanHeartbeatAt = now;
        MelonLogger.Msg($"{LogPrefix} ({reason}) Scan heartbeat: vendors={scan.VendorCount} items={scan.ItemCount}.");
      }
    }

    private static void StartHangWatchdog()
    {
      if (_hangWatchdogThread != null)
      {
        return;
      }

      _hangWatchdogRunning = true;
      _hangDumpTaken = false;
      _hangWatchdogThread = new Thread(HangWatchdogLoop)
      {
        IsBackground = true,
        Name = "BonkUltraHangWatchdog"
      };
      _hangWatchdogThread.Start();
    }

    private static void HangWatchdogLoop()
    {
      while (_hangWatchdogRunning)
      {
        Thread.Sleep(1000);
        long lastTick = Volatile.Read(ref _mainThreadHeartbeatTicks);
        if (lastTick <= 0)
        {
          continue;
        }

        double elapsed = (Stopwatch.GetTimestamp() - lastTick) / (double)Stopwatch.Frequency;
        if (!_hangDumpTaken && elapsed >= HangDumpTimeoutSeconds)
        {
          _hangDumpTaken = true;
          CreateHangDump($"hang-{elapsed:0}s");
        }
      }
    }

    private static void CreateHangDump(string reason)
    {
      try
      {
        string? userDir = ResolveUserDataDirectory();
        if (string.IsNullOrWhiteSpace(userDir))
        {
          return;
        }

        string dumpDir = Path.Combine(userDir, "BonkUltraAlpha");
        Directory.CreateDirectory(dumpDir);
        string stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        string fileName = $"hang-dump-{stamp}-{reason}.dmp";
        string path = Path.Combine(dumpDir, fileName);

        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var proc = Process.GetCurrentProcess())
        {
          MiniDumpType dumpType = MiniDumpType.MiniDumpWithThreadInfo
            | MiniDumpType.MiniDumpWithUnloadedModules
            | MiniDumpType.MiniDumpWithHandleData
            | MiniDumpType.MiniDumpWithFullMemoryInfo;
          bool success = MiniDumpWriteDump(proc.Handle, proc.Id, fs.SafeFileHandle, dumpType,
            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
          string statusPath = Path.Combine(dumpDir, $"hang-dump-{stamp}-{reason}.txt");
          if (success)
          {
            File.WriteAllText(statusPath, $"OK {path}");
          }
          else
          {
            int error = Marshal.GetLastWin32Error();
            File.WriteAllText(statusPath, $"FAILED {path} err={error}");
          }
        }
      }
      catch (Exception)
      {
      }
    }

    private enum MiniDumpType : uint
    {
      MiniDumpNormal = 0x00000000,
      MiniDumpWithDataSegs = 0x00000001,
      MiniDumpWithFullMemory = 0x00000002,
      MiniDumpWithHandleData = 0x00000004,
      MiniDumpWithUnloadedModules = 0x00000020,
      MiniDumpWithFullMemoryInfo = 0x00000800,
      MiniDumpWithThreadInfo = 0x00001000,
    }

    [DllImport("Dbghelp.dll", SetLastError = true)]
    private static extern bool MiniDumpWriteDump(
      IntPtr hProcess,
      int processId,
      Microsoft.Win32.SafeHandles.SafeFileHandle hFile,
      MiniDumpType dumpType,
      IntPtr expParam,
      IntPtr userStreamParam,
      IntPtr callbackParam);

    private static string? ResolveUserDataDirectory()
    {
      string? resolved = ResolveMelonPath("MelonLoader.MelonEnvironment", "UserDataDirectory");
      if (!string.IsNullOrWhiteSpace(resolved))
      {
        return resolved;
      }

      resolved = ResolveMelonPath("MelonLoader.MelonUtils", "UserDataDirectory");
      if (!string.IsNullOrWhiteSpace(resolved))
      {
        return resolved;
      }

      string? baseDir = ResolveMelonPath("MelonLoader.MelonUtils", "BaseDirectory");
      if (!string.IsNullOrWhiteSpace(baseDir))
      {
        return Path.Combine(baseDir, "UserData");
      }

      return null;
    }

    private static string? ResolveMelonPath(string typeName, string propertyName)
    {
      try
      {
        Type? type = Type.GetType($"{typeName}, MelonLoader") ?? Type.GetType(typeName);
        PropertyInfo? prop = type?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
        return prop?.GetValue(null) as string;
      }
      catch (Exception)
      {
        return null;
      }
    }

    private static void WriteBreadcrumb(string eventType, string reason, VendorScanResult scan)
    {
      try
      {
        _breadcrumbSequence++;
        _lastBreadcrumbReason = reason;
        _lastBreadcrumbScan = scan;

        string seedText = "n/a";
        int? seed = GameApi.GetMapSeed();
        if (seed.HasValue && seed.Value != 0)
        {
          seedText = seed.Value.ToString();
        }

        string hudText = "n/a";
        if (_lastHudReadyTime > 0f)
        {
          float hudAge = Mathf.Max(0f, Time.realtimeSinceStartup - _lastHudReadyTime);
          hudText = $"{hudAge:0.00}s";
        }

        long managedMemory = GC.GetTotalMemory(false);
        long allocMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
        long reservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
        long monoMemory = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();

        string snapshot =
          $"{DateTime.UtcNow:O}|event={eventType}|reason={reason}|seed={seedText}|hudAge={hudText}|" +
          $"restartSeq={_restartSequence}|breadcrumbSeq={_breadcrumbSequence}|" +
          $"vendors={scan.VendorCount} done={scan.DoneVendorCount} vendorLegendary={scan.LegendaryVendorTierCount} " +
          $"vendorEpic={scan.EpicVendorTierCount} moai={scan.MoaiCount} microwaves={scan.MicrowaveCount} " +
          $"epicMicrowaves={scan.EpicMicrowaveCount} soulHarvester={scan.SoulHarvesterCount} " +
          $"greenCards={scan.GreenCreditCardCount} items={scan.ItemCount} legendaryItems={scan.LegendaryItemCount} " +
          $"memManaged={managedMemory} memAlloc={allocMemory} memReserved={reservedMemory} memMono={monoMemory}";

        bool shouldSavePrefs = eventType.Contains("restart", StringComparison.OrdinalIgnoreCase);
        if (_prefLastBreadcrumb != null && shouldSavePrefs)
        {
          _prefLastBreadcrumb.Value = snapshot;
          MelonPreferences.Save();
        }

        if (!string.IsNullOrWhiteSpace(_breadcrumbFilePath))
        {
          AppendBreadcrumbLine(_breadcrumbFilePath, snapshot);
        }
      }
      catch (Exception ex)
      {
        if (!_breadcrumbFailureLogged)
        {
          MelonLogger.Msg($"{LogPrefix} Breadcrumb write failed: {ex.GetType().Name} {ex.Message}");
          _breadcrumbFailureLogged = true;
        }
      }
    }

    private static void AppendBreadcrumbLine(string path, string line)
    {
      List<string> lines = new List<string>();
      if (File.Exists(path))
      {
        lines.AddRange(File.ReadAllLines(path));
      }

      lines.Add(line);
      if (lines.Count > BreadcrumbMaxLines)
      {
        lines = lines.GetRange(lines.Count - BreadcrumbMaxLines, BreadcrumbMaxLines);
      }

      File.WriteAllLines(path, lines);
    }

    private static void DisableAutoRestart(string reason)
    {
      if (_prefEnabled == null || !_prefEnabled.Value)
      {
        return;
      }

      _prefEnabled.Value = false;
      MelonPreferences.Save();
      MelonLogger.Msg($"{LogPrefix} Auto-restart disabled ({reason}).");
    }

    private static void ResetScanWatchdog()
    {
      _lastScanAt = 0f;
      _scanHeartbeatAt = 0f;
      _scanWatchdogLogged = false;
    }

    private static void ClampButtonToScreen(ref float x, ref float y, float width, float height)
    {
      float maxX = Mathf.Max(0f, Screen.width - width);
      float maxY = Mathf.Max(0f, Screen.height - height);
      x = Mathf.Clamp(x, 0f, maxX);
      y = Mathf.Clamp(y, 0f, maxY);
    }

    private void DrawSettingsPanel(Rect rect)
    {
      if (_windowStyle == null)
      {
        return;
      }

      const float titleOffset = 28f;
      const float lineHeight = 20f;
      const float spacing = 8f;
      const float buttonHeight = 28f;
      const int toggleRows = 6;

      float requiredHeight = 28f;
      requiredHeight += lineHeight + spacing;
      requiredHeight += toggleRows * (buttonHeight + spacing);
      requiredHeight += spacing;
      if (rect.height < requiredHeight)
      {
        rect.height = requiredHeight;
        _settingsRect.height = requiredHeight;
      }

      GUI.Box(rect, "Bonk Ultra", _windowStyle);

      float x = rect.x + 12f;
      float y = rect.y + titleOffset;
      float width = rect.width - 24f;

      if (_headerStyle != null)
      {
        GUI.Label(new Rect(x, y, width, lineHeight), "Auto-restart controls", _headerStyle);
      }

      y += lineHeight + spacing;

      bool enabled = SettingsEnabled;
      string toggleLabel = enabled ? "Auto-Restart: ON" : "Auto-Restart: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), toggleLabel, _buttonStyle))
      {
        if (_prefEnabled != null)
        {
          _prefEnabled.Value = !enabled;
          MelonPreferences.Save();
        }
      }

      y += buttonHeight + spacing;

      bool soundEnabled = SoundEnabled;
      string soundLabel = soundEnabled ? "Game Sound: ON" : "Game Sound: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), soundLabel, _buttonStyle))
      {
        if (_prefSoundEnabled != null)
        {
          _prefSoundEnabled.Value = !soundEnabled;
          MelonPreferences.Save();
          ApplySoundPreference();
        }
      }

      y += buttonHeight + spacing;

      bool conditionOneEnabled = ConditionOneEnabled;
      string conditionOneLabel = conditionOneEnabled ? "Soul+Green Card: ON" : "Soul+Green Card: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), conditionOneLabel, _buttonStyle))
      {
        if (_prefConditionOneEnabled != null)
        {
          _prefConditionOneEnabled.Value = !conditionOneEnabled;
          MelonPreferences.Save();
        }
      }

      y += buttonHeight + spacing;

      bool conditionTwoEnabled = ConditionTwoEnabled;
      string conditionTwoLabel = conditionTwoEnabled ? "Moai 6+: ON" : "Moai 6+: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), conditionTwoLabel, _buttonStyle))
      {
        if (_prefConditionTwoEnabled != null)
        {
          _prefConditionTwoEnabled.Value = !conditionTwoEnabled;
          MelonPreferences.Save();
        }
      }

      y += buttonHeight + spacing;

      bool conditionThreeEnabled = ConditionThreeEnabled;
      string conditionThreeLabel = conditionThreeEnabled ? "2 Legend + 1 Epic Vendor: ON" : "2 Legend + 1 Epic Vendor: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), conditionThreeLabel, _buttonStyle))
      {
        if (_prefConditionThreeEnabled != null)
        {
          _prefConditionThreeEnabled.Value = !conditionThreeEnabled;
          MelonPreferences.Save();
        }
      }

      y += buttonHeight + spacing;

      bool conditionFourEnabled = ConditionFourEnabled;
      string conditionFourLabel = conditionFourEnabled ? "Green Card + Microwave + Epic Microwave: ON" : "Green Card + Microwave + Epic Microwave: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), conditionFourLabel, _buttonStyle))
      {
        if (_prefConditionFourEnabled != null)
        {
          _prefConditionFourEnabled.Value = !conditionFourEnabled;
          MelonPreferences.Save();
        }
      }

    }

    private IEnumerator EvaluateRun(int token, string reason)
    {
      yield return WaitForLoadReady();
      if (token != _runCheckToken)
      {
        yield break;
      }

      if (_restartDisabledForTimeout)
      {
        _restartDisabledForTimeout = false;
        ResetScanWatchdog();
        yield break;
      }

      if (GameApi.IsMainMenu())
      {
        ResetScanWatchdog();
        yield break;
      }

      GameApi.StageInfo stageInfo = GameApi.GetStageInfo();
      if (stageInfo.IsBossStage)
      {
        string stageName = string.IsNullOrWhiteSpace(stageInfo.StageName) ? "unknown" : stageInfo.StageName;
        MelonLogger.Msg($"{LogPrefix} ({reason}) Boss stage detected ({stageName}); skipping auto-restart.");
        ResetScanWatchdog();
        yield break;
      }

      if (stageInfo.StageTier.HasValue && stageInfo.StageTier.Value >= 2)
      {
        string source = string.IsNullOrWhiteSpace(stageInfo.StageSource) ? "unknown" : stageInfo.StageSource;
        MelonLogger.Msg($"{LogPrefix} ({reason}) Stage tier {stageInfo.StageTier.Value} ({source}); skipping auto-restart.");
        ResetScanWatchdog();
        yield break;
      }

      bool anyConditionsEnabled = ConditionOneEnabled || ConditionTwoEnabled || ConditionThreeEnabled || ConditionFourEnabled;
      if (!anyConditionsEnabled)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) No conditions enabled; skipping.");
        yield break;
      }

      bool requiresItems = ConditionOneEnabled || ConditionFourEnabled;
      VendorScanResult scan = default;
      bool sawItems = false;
      bool loggedItemTiming = false;
      float hudReadyTime = _lastHudReadyTime;
      float itemWaitStart = hudReadyTime > 0f ? hudReadyTime : Time.realtimeSinceStartup;
      bool itemWaitExceeded = false;
      bool conditionOne = false;
      bool conditionTwo = false;
      bool conditionThree = false;
      bool conditionFour = false;
      bool meetsAny = false;
      float waited = 0f;
      while (waited < VendorPollTimeoutSeconds)
      {
        if (token != _runCheckToken || GameApi.IsMainMenu())
        {
          yield break;
        }

        scan = VendorScanner.Scan();
        WriteBreadcrumb("scan", reason, scan);
        RecordScanHeartbeat(reason, scan);
        if (scan.ItemCount > 0)
        {
          sawItems = true;
          if (requiresItems && !loggedItemTiming)
          {
            LogItemSpawnTiming(reason, hudReadyTime);
            loggedItemTiming = true;
          }
        }

        meetsAny = EvaluateConditions(scan, out conditionOne, out conditionTwo, out conditionThree, out conditionFour);
        if (meetsAny)
        {
          break;
        }

        if (HasAnyScanData(scan) && sawItems)
        {
          break;
        }

        if (requiresItems && !sawItems)
        {
          float itemWait = Time.realtimeSinceStartup - itemWaitStart;
          if (itemWait >= ItemWaitTimeoutSeconds)
          {
            itemWaitExceeded = true;
            break;
          }
        }

        yield return TimerApi.WaitSeconds(VendorPollIntervalSeconds);
        waited += VendorPollIntervalSeconds;
      }

      if (!HasAnyScanData(scan) || (requiresItems && !sawItems))
      {
        scan = VendorScanner.Scan();
        WriteBreadcrumb("scan", reason, scan);
        RecordScanHeartbeat(reason, scan);
        if (scan.ItemCount > 0)
        {
          sawItems = true;
          if (requiresItems && !loggedItemTiming)
          {
            LogItemSpawnTiming(reason, hudReadyTime);
            loggedItemTiming = true;
          }
        }
        meetsAny = EvaluateConditions(scan, out conditionOne, out conditionTwo, out conditionThree, out conditionFour);
      }

      if (!meetsAny && scan.VendorCount == 0 && scan.MoaiCount < ConditionTwoMinMoai)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) No vendors detected; restarting run.");
        _lastBreadcrumbScan = scan;
        _lastBreadcrumbReason = $"{reason}:no-vendors";
        yield return RestartRun();
        yield break;
      }

      if (requiresItems && !sawItems && !meetsAny)
      {
        string reasonText = itemWaitExceeded ? "Items taking too long to load" : "No items detected after scan window";
        _lastBreadcrumbScan = scan;
        _lastBreadcrumbReason = itemWaitExceeded ? $"{reason}:items-timeout" : $"{reason}:no-items";
        MelonLogger.Msg($"{LogPrefix} ({reason}) {reasonText}; restarting run.");
        yield return RestartRun();
        yield break;
      }

      if (!SettingsEnabled)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) settings disabled; skipping.");
        yield break;
      }
      bool pauseOpen = IsPauseMenuOpen();

      MelonLogger.Msg(
        $"{LogPrefix} ({reason}) vendors={scan.VendorCount} done={scan.DoneVendorCount} " +
        $"vendorTierLegendary={scan.LegendaryVendorTierCount} vendorTierEpic={scan.EpicVendorTierCount} " +
        $"moai={scan.MoaiCount} microwaves={scan.MicrowaveCount} epicMicrowaves={scan.EpicMicrowaveCount} " +
        $"soulHarvester={scan.SoulHarvesterCount} greenCards={scan.GreenCreditCardCount} " +
        $"items={scan.ItemCount} legendaryItems={scan.LegendaryItemCount} " +
        $"cond1={conditionOne} cond2={conditionTwo} cond3={conditionThree} cond4={conditionFour}");

      if (meetsAny)
      {
        if (pauseOpen)
        {
          MelonLogger.Msg($"{LogPrefix} ({reason}) Conditions met; pause menu already open.");
          yield break;
        }

        if (GameApi.TryOpenPauseMenu(out string pauseSource))
        {
          MelonLogger.Msg($"{LogPrefix} Conditions met. Opened pause menu via {pauseSource}.");
        }
        else
        {
          MelonLogger.Msg($"{LogPrefix} Conditions met. Pressing ESC.");
          yield return InputApi.PressKey("ESC", EscTapSeconds);
        }
        yield break;
      }

      if (pauseOpen)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) Pause menu open; blocking auto-restart.");
        yield break;
      }

      bool otherConditionsMet = conditionOne || conditionTwo || conditionThree;
      bool conditionFourReady = ConditionFourItemsSatisfied(scan);
      if (ConditionFourEnabled)
      {
        if (conditionFourReady)
        {
          _conditionFourFailureCount = 0;
          _conditionFourCooldownUntil = 0f;
        }
        else if (!otherConditionsMet)
        {
          _conditionFourFailureCount++;
          if (_conditionFourFailureCount >= ConditionFourFailureThreshold
            && _conditionFourCooldownUntil <= Time.realtimeSinceStartup)
          {
            _conditionFourFailureCount = 0;
            _conditionFourCooldownUntil = Time.realtimeSinceStartup + ConditionFourCooldownSeconds;
            if (TryDescribeConditionFourMissing(scan, out string detail))
            {
              MelonLogger.Msg(
                $"{LogPrefix} Condition 4 missing {detail}; delaying restarts for {ConditionFourCooldownSeconds:0.0}s.");
              WriteBreadcrumb("condition4-delay", $"{reason}:{detail}", scan);
            }
            else
            {
              MelonLogger.Msg(
                $"{LogPrefix} Condition 4 missing required items; delaying restarts for {ConditionFourCooldownSeconds:0.0}s.");
              WriteBreadcrumb("condition4-delay", $"{reason}:condition4", scan);
            }
          }
        }
      }

      if (_conditionFourCooldownUntil > Time.realtimeSinceStartup)
      {
        float remaining = _conditionFourCooldownUntil - Time.realtimeSinceStartup;
        MelonLogger.Msg(
          $"{LogPrefix} ({reason}) Restart suppressed {remaining:0.0}s while waiting for Condition 4 items.");
        yield break;
      }

      MelonLogger.Msg($"{LogPrefix} Conditions not met. Restarting run.");
      _lastBreadcrumbScan = scan;
      _lastBreadcrumbReason = $"{reason}:conditions-not-met";
      yield return RestartRun();
    }

    private static IEnumerator WaitForLoadReady()
    {
      _lastHudReadyTime = 0f;
      float waited = 0f;
      while (waited < MaxLoadWaitSeconds)
      {
        if (GameApi.IsMainMenu())
        {
          _restartInProgress = false;
          _restartFallbackAttempted = false;
          _restartDisabledForTimeout = false;
          yield break;
        }

        if (_restartInProgress)
        {
          float restartAge = Mathf.Max(0f, Time.realtimeSinceStartup - _restartStartTime);
          if (Time.realtimeSinceStartup - _restartHeartbeatAt >= 2f)
          {
            _restartHeartbeatAt = Time.realtimeSinceStartup;
            MelonLogger.Msg($"{LogPrefix} Restart in progress ({restartAge:0.0}s).");
          }
          if (restartAge >= RestartHudTimeoutSeconds)
          {
            if (!_restartFallbackAttempted)
            {
              _restartFallbackAttempted = true;
              MelonLogger.Msg($"{LogPrefix} Restart HUD timeout after {restartAge:0.00}s; holding R fallback.");
              WriteBreadcrumb("restart-timeout", $"{_lastBreadcrumbReason}:fallback", _lastBreadcrumbScan);
              yield return InputApi.HoldKey("R", RestartHoldSeconds);
              _restartStartTime = Time.realtimeSinceStartup;
            }
            else
            {
              MelonLogger.Msg($"{LogPrefix} Restart HUD timeout after fallback; disabling auto-restart.");
              WriteBreadcrumb("restart-timeout", $"{_lastBreadcrumbReason}:disable", _lastBreadcrumbScan);
              DisableAutoRestart("restart-timeout");
              _restartDisabledForTimeout = true;
              _restartInProgress = false;
              yield break;
            }
          }
        }

        if (GameObject.Find("HUD") != null)
        {
          _lastHudReadyTime = Time.realtimeSinceStartup;
          _restartInProgress = false;
          _restartFallbackAttempted = false;
          _restartDisabledForTimeout = false;
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
      _restartSequence++;
      float now = Time.realtimeSinceStartup;
      if (_lastRestartIssuedAt > 0f)
      {
        float sinceLast = now - _lastRestartIssuedAt;
        if (sinceLast < MinRestartIntervalSeconds)
        {
          yield return TimerApi.WaitSeconds(MinRestartIntervalSeconds - sinceLast);
        }
      }

      if (IsPauseMenuOpen())
      {
        MelonLogger.Msg($"{LogPrefix} Pause menu open; deferring restart.");
        yield break;
      }

      yield return WaitForSafeRestartWindow(_lastBreadcrumbReason);
      _lastRestartIssuedAt = Time.realtimeSinceStartup;
      _restartInProgress = true;
      _restartStartTime = Time.realtimeSinceStartup;
      _restartHeartbeatAt = _restartStartTime;
      _restartFallbackAttempted = false;
      _restartDisabledForTimeout = false;
      float streakNow = _lastRestartIssuedAt;
      if (_restartStreakCount == 0 || streakNow - _restartStreakLastAt > RestartStreakResetSeconds)
      {
        _restartStreakCount = 0;
        _restartStreakStartAt = streakNow;
      }
      _restartStreakCount++;
      _restartStreakLastAt = streakNow;
      MelonLogger.Msg(
        $"{LogPrefix} Restart streak={_restartStreakCount} window={streakNow - _restartStreakStartAt:0.0}s " +
        $"reason={_lastBreadcrumbReason}.");
      WriteBreadcrumb("restart", _lastBreadcrumbReason, _lastBreadcrumbScan);
      if (RestartStreakPauseCount > 0 && _restartStreakCount % RestartStreakPauseCount == 0)
      {
        MelonLogger.Msg($"{LogPrefix} Restart streak reached {_restartStreakCount}; pausing {RestartStreakPauseSeconds:0.0}s.");
        yield return TimerApi.WaitSeconds(RestartStreakPauseSeconds);
      }
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
      private const string SoulHarvesterName = "Soul Harvester";
      private const string CreditCardName = "Credit Card";
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
            else if (vendor.rarity == EItemRarity.Epic)
            {
              result.EpicVendorTierCount++;
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

              if (TryGetItemName(item, out string itemName))
              {
                if (NameContains(itemName, SoulHarvesterName) || NameContains(itemName, "SoulHarvester"))
                {
                  result.SoulHarvesterCount++;
                }

                if ((NameContains(itemName, CreditCardName) || NameContains(itemName, "CreditCard"))
                  && NameContains(itemName, "Green"))
                {
                  result.GreenCreditCardCount++;
                }
              }
            }
          }

          ScanMicrowaves(ref result);
          ScanMoai(ref result);
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

      private static void ScanMicrowaves(ref VendorScanResult result)
      {
        try
        {
          Il2CppArrayBase<InteractableMicrowave> microwaves = UnityEngine.Object.FindObjectsOfType<InteractableMicrowave>();
          foreach (var microwave in microwaves)
          {
            if (microwave == null)
            {
              continue;
            }

            if (microwave.usesLeft <= 0)
            {
              continue;
            }

            result.MicrowaveCount++;
            if (microwave.rarity == EItemRarity.Epic)
            {
              result.EpicMicrowaveCount++;
            }
          }
        }
        catch (Exception)
        {
        }
      }

      private static void ScanMoai(ref VendorScanResult result)
      {
        result.MoaiCount = CountMoai();
      }

      private static bool TryGetItemName(ItemData item, out string name)
      {
        name = string.Empty;
        try
        {
          var unlockable = (UnlockableBase)item;
          name = unlockable.GetName();
          return !string.IsNullOrWhiteSpace(name);
        }
        catch (Exception)
        {
          return false;
        }
      }

      private static bool NameContains(string? name, string expected)
      {
        return !string.IsNullOrWhiteSpace(name)
          && name.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
      }

      private static int CountMoai()
      {
        int count = 0;
        var seen = new HashSet<int>();

        try
        {
          GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
          foreach (var obj in objects)
          {
            if (obj == null || !IsSceneObject(obj))
            {
              continue;
            }

            if (IsMoaiName(obj.name))
            {
              int id = obj.GetInstanceID();
              if (seen.Add(id))
              {
                count++;
              }
            }
          }

          if (count > 0)
          {
            return count;
          }

          Component[] components = Resources.FindObjectsOfTypeAll<Component>();
          foreach (var component in components)
          {
            if (component == null)
            {
              continue;
            }

            string typeName = component.GetType().Name;
            if (!IsMoaiName(typeName))
            {
              continue;
            }

            GameObject obj = component.gameObject;
            if (obj == null || !IsSceneObject(obj))
            {
              continue;
            }

            int id = obj.GetInstanceID();
            if (seen.Add(id))
            {
              count++;
            }
          }
        }
        catch (Exception)
        {
        }

        return count;
      }

      private static bool IsMoaiName(string name)
      {
        if (string.IsNullOrWhiteSpace(name))
        {
          return false;
        }

        string lower = name.ToLowerInvariant();
        return lower.Contains("moai")
          || lower.Contains("soulharvester")
          || lower.Contains("soul_harvester")
          || lower.Contains("soul harvester");
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
    }

    private struct VendorScanResult
    {
      public int VendorCount;
      public int DoneVendorCount;
      public int LegendaryVendorTierCount;
      public int EpicVendorTierCount;
      public int MoaiCount;
      public int MicrowaveCount;
      public int EpicMicrowaveCount;
      public int SoulHarvesterCount;
      public int GreenCreditCardCount;
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
    private static readonly string[] StageTierMemberHints =
      { "stagetier", "tier", "currenttier", "tierindex", "tieridx" };
    private static readonly string[] StageTierExcludeHints =
      { "maptier", "maptierindex", "runtier", "runconfig" };

    public readonly struct StageInfo
    {
      public StageInfo(int? stageTier, string? stageSource, string? stageName, bool isBossStage)
      {
        StageTier = stageTier;
        StageSource = stageSource;
        StageName = stageName;
        IsBossStage = isBossStage;
      }

      public int? StageTier { get; }
      public string? StageSource { get; }
      public string? StageName { get; }
      public bool IsBossStage { get; }
    }

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

    public static StageInfo GetStageInfo()
    {
      string? stageName = TryGetStageName();
      bool isBossStage = IsBossStageName(stageName);
      string? stageSource = null;
      int? stageTier = TryGetStageTierFromStageData(out stageSource);

      if (!stageTier.HasValue)
      {
        stageTier = TryGetStageTierFromStageName(stageName, out stageSource);
      }

      return new StageInfo(stageTier, stageSource, stageName, isBossStage);
    }

    public static int? GetRunTier()
    {
      try
      {
        var runConfig = MapController.runConfig;
        if (runConfig == null)
        {
          return null;
        }

        return runConfig.mapTierIndex + 1;
      }
      catch (Exception)
      {
        return null;
      }
    }

    private static int? TryGetStageTierFromStageData(out string? source)
    {
      source = null;
      try
      {
        var stage = MapController.currentStage;
        if (stage == null)
        {
          return null;
        }

        Type type = stage.GetType();
        if (TryGetTierCandidate(stage, type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          out int value, out string memberName))
        {
          source = $"{type.Name}.{memberName}";
          return NormalizeTier(memberName, value);
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    private static int? TryGetStageTierFromStageName(string? stageName, out string? source)
    {
      source = null;
      if (TryExtractTierFromName(stageName, out int tier))
      {
        source = string.IsNullOrWhiteSpace(stageName) ? "StageName" : $"StageName:{stageName}";
        return tier;
      }

      return null;
    }

    private static string? TryGetStageName()
    {
      try
      {
        var stage = MapController.currentStage;
        if (stage == null)
        {
          return null;
        }

        if (stage is UnityEngine.Object stageObject)
        {
          return stageObject.name;
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    private static bool IsBossStageName(string? stageName)
    {
      if (string.IsNullOrWhiteSpace(stageName))
      {
        return false;
      }

      string lower = stageName.ToLowerInvariant();
      return lower.Contains("boss") || lower.Contains("final");
    }

    private static bool TryGetTierCandidate(object instance, Type type, BindingFlags flags, out int value,
      out string memberName)
    {
      value = 0;
      memberName = string.Empty;

      FieldInfo[] fields = type.GetFields(flags);
      foreach (var field in fields)
      {
        if (!IsStageTierMemberName(field.Name))
        {
          continue;
        }

        try
        {
          if (TryConvertToInt(field.GetValue(instance), out value))
          {
            memberName = field.Name;
            return true;
          }
        }
        catch (Exception)
        {
        }
      }

      PropertyInfo[] properties = type.GetProperties(flags);
      foreach (var property in properties)
      {
        if (property.GetIndexParameters().Length != 0)
        {
          continue;
        }

        if (!IsStageTierMemberName(property.Name))
        {
          continue;
        }

        try
        {
          if (TryConvertToInt(property.GetValue(instance, null), out value))
          {
            memberName = property.Name;
            return true;
          }
        }
        catch (Exception)
        {
        }
      }

      return false;
    }

    private static bool IsStageTierMemberName(string name)
    {
      string normalized = NormalizeMemberName(name);
      if (string.IsNullOrEmpty(normalized))
      {
        return false;
      }

      foreach (string exclude in StageTierExcludeHints)
      {
        if (normalized.Contains(exclude, StringComparison.OrdinalIgnoreCase))
        {
          return false;
        }
      }

      foreach (string hint in StageTierMemberHints)
      {
        if (normalized.Contains(hint, StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }

    private static string NormalizeMemberName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return string.Empty;
      }

      char[] buffer = new char[name.Length];
      int count = 0;
      foreach (char c in name)
      {
        if (char.IsLetterOrDigit(c))
        {
          buffer[count++] = char.ToLowerInvariant(c);
        }
      }

      return new string(buffer, 0, count);
    }

    private static bool TryConvertToInt(object? value, out int result)
    {
      result = 0;
      if (value == null)
      {
        return false;
      }

      try
      {
        switch (value)
        {
          case int intValue:
            result = intValue;
            return true;
          case byte byteValue:
            result = byteValue;
            return true;
          case short shortValue:
            result = shortValue;
            return true;
          case long longValue:
            result = (int)longValue;
            return true;
        }

        result = Convert.ToInt32(value);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static int NormalizeTier(string memberName, int value)
    {
      string normalized = NormalizeMemberName(memberName);
      if (normalized.Contains("index", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("idx", StringComparison.OrdinalIgnoreCase))
      {
        return value + 1;
      }

      return value;
    }

    private static bool TryExtractTierFromName(string? name, out int tier)
    {
      tier = 0;
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      string lower = name.ToLowerInvariant();
      for (int candidate = 1; candidate <= 5; candidate++)
      {
        if (lower.Contains($"tier{candidate}")
          || lower.Contains($"tier_{candidate}")
          || lower.Contains($"tier {candidate}")
          || lower.Contains($"stage{candidate}")
          || lower.Contains($"stage {candidate}"))
        {
          tier = candidate;
          return true;
        }
      }

      if (TryExtractLastNumber(lower, out int parsed))
      {
        if (lower.Contains("tier") || lower.Contains("stage"))
        {
          tier = parsed;
          return true;
        }
      }

      return false;
    }

    private static bool TryExtractLastNumber(string text, out int value)
    {
      value = 0;
      if (string.IsNullOrWhiteSpace(text))
      {
        return false;
      }

      int end = -1;
      for (int i = text.Length - 1; i >= 0; i--)
      {
        if (char.IsDigit(text[i]))
        {
          end = i;
          break;
        }
      }

      if (end < 0)
      {
        return false;
      }

      int start = end;
      while (start >= 0 && char.IsDigit(text[start]))
      {
        start--;
      }

      string number = text.Substring(start + 1, end - start);
      return int.TryParse(number, out value);
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
