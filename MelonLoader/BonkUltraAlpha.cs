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
#endif

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "0.4.1", "Strei")]
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
    private const float PortalScanIntervalSeconds = 0.2f;
    private const float PortalScanTimeoutSeconds = 10f;
    private const float PortalAnimatorSpeedMultiplier = 2.5f;
    private const float PortalTimeScale = 0.35f;
    private const int PortalDiagnosticMaxObjects = 6;
    private const int PortalDiagnosticMaxComponents = 12;
    private const int PortalDiagnosticMaxFields = 20;
    private const int PortalDiagnosticMaxClips = 8;
    private const int PortalDiagnosticMaxAnimators = 12;
    private const int PortalDiagnosticMaxLegacyAnimations = 8;
    private const int PortalDiagnosticMaxAnimationClips = 8;
    private const float EscTapSeconds = 0.05f;
    private const int DefaultMinLegendaryVendors = 1;
    private const int DefaultMinEpicVendors = 0;
    private const int DefaultMinMoai = 0;
    private const int DefaultMinMicrowaves = 0;
    private const int DefaultMinEpicMicrowaves = 0;
    private const int DefaultMinGreenCreditCards = 0;
    private const int DefaultMinSoulHarvesters = 0;
    private const int DefaultMainMenuButtonX = 20;
    private const int DefaultMainMenuButtonY = 80;
    private const int DefaultSettingsMenuX = 680;
    private const int DefaultSettingsMenuY = 270;
    private const int MaxConditionCount = 20;
    private const float PauseUiPollSeconds = 0.5f;
    private const int BreadcrumbMaxLines = 50;
    private const float ScanHeartbeatIntervalSeconds = 2f;
    private const float ScanWatchdogTimeoutSeconds = 6f;
    private const float HangDumpTimeoutSeconds = 10f;

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<bool>? _prefSoundEnabled;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonX;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonY;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuX;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuY;
    private static MelonPreferences_Entry<int>? _prefMinLegendaryVendors;
    private static MelonPreferences_Entry<int>? _prefMinEpicVendors;
    private static MelonPreferences_Entry<int>? _prefMinMoai;
    private static MelonPreferences_Entry<int>? _prefMinMicrowaves;
    private static MelonPreferences_Entry<int>? _prefMinEpicMicrowaves;
    private static MelonPreferences_Entry<int>? _prefMinGreenCreditCards;
    private static MelonPreferences_Entry<int>? _prefMinSoulHarvesters;
    private static MelonPreferences_Entry<string>? _prefLastBreadcrumb;
    private static float _soundOnVolume = -1f;
    private static string? _breadcrumbFilePath;
    private static string? _portalDiagnosticPath;
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
    private static GUIStyle? _smallButtonStyle;
    private static GUIStyle? _windowStyle;
    private static GUIStyle? _headerStyle;
    private static Texture2D? _legendaryTexture;
    private static Texture2D? _panelTexture;
    private static GameObject? _pauseUiCached;
    private static float _pauseUiLastCheck;
    private static GameObject? _uiBlocker;
    private static float _lastHudReadyTime;
    private static int _portalTweakedToken;
    private static int _portalDiagnosticToken;

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAlpha", "Bonk Ultra");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-restart");
      _prefSoundEnabled = _prefs.CreateEntry("SoundEnabled", true, "Game sound on/off");
      MelonPreferences_Entry<bool>? legacyRequireSoulHarvester = _prefs.CreateEntry("RequireSoulHarvester", false, "Require Soul Harvester");
      _prefMainMenuButtonX = _prefs.CreateEntry("MainMenuButtonX", DefaultMainMenuButtonX, "Main menu button X");
      _prefMainMenuButtonY = _prefs.CreateEntry("MainMenuButtonY", DefaultMainMenuButtonY, "Main menu button Y");
      _prefSettingsMenuX = _prefs.CreateEntry("SettingsMenuX", DefaultSettingsMenuX, "Settings menu X");
      _prefSettingsMenuY = _prefs.CreateEntry("SettingsMenuY", DefaultSettingsMenuY, "Settings menu Y");
      _prefMinLegendaryVendors = _prefs.CreateEntry("MinLegendaryItems", DefaultMinLegendaryVendors, "Minimum legendary vendors");
      _prefMinEpicVendors = _prefs.CreateEntry("MinEpicVendors", DefaultMinEpicVendors, "Minimum epic vendors");
      _prefMinMoai = _prefs.CreateEntry("MinMoai", DefaultMinMoai, "Minimum moai");
      _prefMinMicrowaves = _prefs.CreateEntry("MinMicrowaves", DefaultMinMicrowaves, "Minimum microwaves");
      _prefMinEpicMicrowaves = _prefs.CreateEntry("MinEpicMicrowaves", DefaultMinEpicMicrowaves, "Minimum epic microwaves");
      _prefMinGreenCreditCards = _prefs.CreateEntry("MinGreenCreditCards", DefaultMinGreenCreditCards, "Minimum green credit cards");
      _prefMinSoulHarvesters = _prefs.CreateEntry("MinSoulHarvesters", DefaultMinSoulHarvesters, "Minimum soul harvesters");
      _prefLastBreadcrumb = _prefs.CreateEntry("LastBreadcrumb", string.Empty, "Last crash breadcrumb");
      if (legacyRequireSoulHarvester != null && _prefMinSoulHarvesters != null
        && legacyRequireSoulHarvester.Value && _prefMinSoulHarvesters.Value == 0)
      {
        _prefMinSoulHarvesters.Value = 1;
        MelonPreferences.Save();
      }
      try
      {
        string? userDir = ResolveUserDataDirectory();
        if (!string.IsNullOrWhiteSpace(userDir))
        {
          string breadcrumbDir = Path.Combine(userDir, "BonkUltraAlpha");
          Directory.CreateDirectory(breadcrumbDir);
          _breadcrumbFilePath = Path.Combine(breadcrumbDir, "last-breadcrumb.txt");
          _portalDiagnosticPath = Path.Combine(breadcrumbDir, "last-portal-diagnostic.txt");
        }
      }
      catch (Exception)
      {
        _breadcrumbFilePath = null;
        _portalDiagnosticPath = null;
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
      MelonCoroutines.Start(TrySpeedPortalAnimation(_runCheckToken));
    }

    private IEnumerator TrySpeedPortalAnimation(int token)
    {
      float start = Time.realtimeSinceStartup;
      int? lastRunTier = null;
      GameApi.StageInfo lastStageInfo = default;
      bool sawTargetStage = false;
      while (Time.realtimeSinceStartup - start < PortalScanTimeoutSeconds)
      {
        if (token != _runCheckToken || GameApi.IsMainMenu())
        {
          yield break;
        }

        GameApi.StageInfo stageInfo = GameApi.GetStageInfo();
        int? runTier = GameApi.GetRunTier();
        lastRunTier = runTier;
        lastStageInfo = stageInfo;
        if (runTier.HasValue && runTier.Value == 3
          && stageInfo.StageTier.HasValue && stageInfo.StageTier.Value == 1
          && !stageInfo.IsBossStage)
        {
          sawTargetStage = true;
          if (_portalTweakedToken != token && TryApplyPortalSpeedup())
          {
            _portalTweakedToken = token;
            yield break;
          }
        }

        yield return TimerApi.WaitSeconds(PortalScanIntervalSeconds);
      }

      if (token == _runCheckToken && !GameApi.IsMainMenu())
      {
        string runTierText = lastRunTier.HasValue ? lastRunTier.Value.ToString() : "unknown";
        string stageTierText = lastStageInfo.StageTier.HasValue ? lastStageInfo.StageTier.Value.ToString() : "unknown";
        string stageName = string.IsNullOrWhiteSpace(lastStageInfo.StageName) ? "unknown" : lastStageInfo.StageName;
        MelonLogger.Msg(
          $"{LogPrefix} Portal speedup not applied (runTier={runTierText} stageTier={stageTierText} " +
          $"boss={lastStageInfo.IsBossStage} stage={stageName}).");
        string reason = sawTargetStage ? "portal-timeout-target" : "portal-timeout-no-target";
        DumpPortalDiagnostics(token, lastRunTier, lastStageInfo, reason);
      }
    }

    private static void DumpPortalDiagnostics(int token, int? runTier, GameApi.StageInfo stageInfo, string reason)
    {
      if (_portalDiagnosticToken == token)
      {
        return;
      }

      _portalDiagnosticToken = token;
      List<string> lines = new List<string>();
      string runTierText = runTier.HasValue ? runTier.Value.ToString() : "unknown";
      string stageTierText = stageInfo.StageTier.HasValue ? stageInfo.StageTier.Value.ToString() : "unknown";
      string stageSource = string.IsNullOrWhiteSpace(stageInfo.StageSource) ? "unknown" : stageInfo.StageSource;
      string stageName = string.IsNullOrWhiteSpace(stageInfo.StageName) ? "unknown" : stageInfo.StageName;
      lines.Add($"{DateTime.UtcNow:O} reason={reason} token={token} runTier={runTierText} stageTier={stageTierText} " +
        $"stageSource={stageSource} stageName={stageName} boss={stageInfo.IsBossStage}");

      int portalObjects = 0;
      int loggedObjects = 0;

      try
      {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
          if (obj == null || !IsSceneObject(obj))
          {
            continue;
          }

          if (!LooksLikePortal(obj.name) && !HasPortalComponent(obj))
          {
            continue;
          }

          portalObjects++;
          if (loggedObjects >= PortalDiagnosticMaxObjects)
          {
            continue;
          }

          loggedObjects++;
          string path = GetHierarchyPath(obj);
          lines.Add($"object[{loggedObjects}] name={obj.name} activeSelf={obj.activeSelf} " +
            $"activeInHierarchy={obj.activeInHierarchy} layer={obj.layer} path={path}");

          Component[] components = obj.GetComponentsInChildren<Component>(true);
          int loggedComponents = 0;
          foreach (var component in components)
          {
            if (component == null)
            {
              continue;
            }

            if (loggedComponents >= PortalDiagnosticMaxComponents)
            {
              lines.Add("  component[...] (truncated)");
              break;
            }

            loggedComponents++;
            Type type = component.GetType();
            string typeName = ResolveIl2CppTypeName(component);
            lines.Add($"  component[{loggedComponents}]={typeName}");
            AppendAnimatorDiagnostics(lines, component);
            if (LooksLikePortal(type.Name) || LooksLikePortal(component.gameObject.name))
            {
              AppendPortalFieldDiagnostics(lines, component);
            }
          }
        }
      }
      catch (Exception ex)
      {
        lines.Add($"scanFailed={ex.GetType().Name} {ex.Message}");
      }

      AppendAnimatorSummary(lines);
      AppendLegacyAnimationSummary(lines);
      lines.Add($"portalObjects={portalObjects} loggedObjects={loggedObjects}");

      if (!string.IsNullOrWhiteSpace(_portalDiagnosticPath))
      {
        try
        {
          File.WriteAllLines(_portalDiagnosticPath, lines);
          MelonLogger.Msg($"{LogPrefix} Portal diagnostic written to {_portalDiagnosticPath}.");
        }
        catch (Exception ex)
        {
          MelonLogger.Msg($"{LogPrefix} Portal diagnostic write failed: {ex.GetType().Name} {ex.Message}");
        }
      }
      else
      {
        foreach (string line in lines)
        {
          MelonLogger.Msg($"{LogPrefix} {line}");
        }
      }
    }

    private static string GetHierarchyPath(GameObject obj)
    {
      if (obj == null)
      {
        return "unknown";
      }

      try
      {
        string path = obj.name;
        Transform current = obj.transform;
        while (current != null && current.parent != null)
        {
          current = current.parent;
          path = $"{current.name}/{path}";
        }

        return path;
      }
      catch (Exception)
      {
        return obj.name;
      }
    }

    private static void AppendAnimatorDiagnostics(List<string> lines, Component component)
    {
      Type type = component.GetType();
      if (!string.Equals(type.Name, "Animator", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      try
      {
        PropertyInfo? speedProperty = type.GetProperty("speed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (speedProperty != null && speedProperty.PropertyType == typeof(float))
        {
          float speed = (float)speedProperty.GetValue(component, null);
          lines.Add($"    animatorSpeed={speed:0.###}");
        }
      }
      catch (Exception)
      {
      }

      try
      {
        PropertyInfo? controllerProperty = type.GetProperty("runtimeAnimatorController",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        object? controller = controllerProperty?.GetValue(component, null);
        if (controller == null)
        {
          return;
        }

        string controllerName = ResolveObjectName(controller);
        lines.Add($"    animatorController={controllerName}");

        PropertyInfo? clipsProperty = controller.GetType().GetProperty("animationClips",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (clipsProperty == null)
        {
          return;
        }

        object? clipsObj = clipsProperty.GetValue(controller, null);
        if (clipsObj is Array clips)
        {
          int logged = 0;
          foreach (object clip in clips)
          {
            if (clip == null || logged >= PortalDiagnosticMaxClips)
            {
              break;
            }

            logged++;
            string clipName = ResolveObjectName(clip);
            float length = ResolveFloatProperty(clip, "length");
            lines.Add($"    clip[{logged}]={clipName} len={length:0.###}");
          }
        }
      }
      catch (Exception)
      {
      }
    }

    private static void AppendAnimatorSummary(List<string> lines)
    {
      try
      {
        Animator[] animators = Resources.FindObjectsOfTypeAll<Animator>();
        if (animators == null || animators.Length == 0)
        {
          lines.Add("animators=0");
          return;
        }

        int logged = 0;
        foreach (Animator animator in animators)
        {
          if (animator == null)
          {
            continue;
          }

          string path = GetHierarchyPath(animator.gameObject);
          string controllerName = animator.runtimeAnimatorController != null
            ? ResolveObjectName(animator.runtimeAnimatorController)
            : "null";

          if (!IsDiagnosticMatch(animator.gameObject.name, path, controllerName))
          {
            continue;
          }

          logged++;
          lines.Add($"animator[{logged}] name={animator.gameObject.name} active={animator.gameObject.activeInHierarchy} " +
            $"enabled={animator.enabled} speed={animator.speed:0.###} controller={controllerName} path={path}");

          if (animator.runtimeAnimatorController != null)
          {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            int clipLogged = 0;
            foreach (AnimationClip clip in clips)
            {
              if (clip == null)
              {
                continue;
              }

              clipLogged++;
              lines.Add($"  clip[{clipLogged}]={clip.name} len={clip.length:0.###}");
              if (clipLogged >= PortalDiagnosticMaxAnimationClips)
              {
                break;
              }
            }
          }

          if (logged >= PortalDiagnosticMaxAnimators)
          {
            lines.Add("animator[...] (truncated)");
            break;
          }
        }

        if (logged == 0)
        {
          lines.Add($"animators={animators.Length} (no keyword matches)");
        }
      }
      catch (Exception ex)
      {
        lines.Add($"animatorsScanFailed={ex.GetType().Name} {ex.Message}");
      }
    }

    private static void AppendLegacyAnimationSummary(List<string> lines)
    {
      try
      {
        Animation[] animations = Resources.FindObjectsOfTypeAll<Animation>();
        if (animations == null || animations.Length == 0)
        {
          return;
        }

        int logged = 0;
        foreach (Animation animation in animations)
        {
          if (animation == null)
          {
            continue;
          }

          string path = GetHierarchyPath(animation.gameObject);
          if (!IsDiagnosticMatch(animation.gameObject.name, path, null))
          {
            continue;
          }

          logged++;
          lines.Add($"animation[{logged}] name={animation.gameObject.name} active={animation.gameObject.activeInHierarchy} " +
            $"enabled={animation.enabled} path={path}");

          int clipLogged = 0;
          foreach (AnimationState state in animation)
          {
            if (state == null)
            {
              continue;
            }

            clipLogged++;
            lines.Add($"  state[{clipLogged}]={state.name} len={state.length:0.###} speed={state.speed:0.###}");
            if (clipLogged >= PortalDiagnosticMaxAnimationClips)
            {
              break;
            }
          }

          if (logged >= PortalDiagnosticMaxLegacyAnimations)
          {
            lines.Add("animation[...] (truncated)");
            break;
          }
        }

        if (logged == 0)
        {
          lines.Add($"animations={animations.Length} (no keyword matches)");
        }
      }
      catch (Exception ex)
      {
        lines.Add($"animationsScanFailed={ex.GetType().Name} {ex.Message}");
      }
    }

    private static string ResolveIl2CppTypeName(Component component)
    {
      if (component == null)
      {
        return "null";
      }

      try
      {
        Type? extensionType = Type.GetType("Il2CppInterop.Runtime.Il2CppObjectBaseExtensions, Il2CppInterop.Runtime");
        MethodInfo? method = extensionType?.GetMethod("GetIl2CppType", BindingFlags.Public | BindingFlags.Static);
        if (method != null)
        {
          object? il2cppType = method.Invoke(null, new object[] { component });
          if (il2cppType != null)
          {
            PropertyInfo? fullNameProperty = il2cppType.GetType().GetProperty("FullName",
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string? fullName = fullNameProperty?.GetValue(il2cppType, null) as string;
            if (!string.IsNullOrWhiteSpace(fullName))
            {
              return fullName;
            }

            PropertyInfo? nameProperty = il2cppType.GetType().GetProperty("Name",
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string? name = nameProperty?.GetValue(il2cppType, null) as string;
            if (!string.IsNullOrWhiteSpace(name))
            {
              return name;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      Type type = component.GetType();
      return string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName;
    }

    private static bool IsDiagnosticMatch(string? name, string? path, string? extra)
    {
      return ContainsDiagnosticKeyword(name)
        || ContainsDiagnosticKeyword(path)
        || ContainsDiagnosticKeyword(extra);
    }

    private static bool ContainsDiagnosticKeyword(string? value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        return false;
      }

      string lower = value.ToLowerInvariant();
      return lower.Contains("portal")
        || lower.Contains("transition")
        || lower.Contains("warp")
        || lower.Contains("teleport")
        || lower.Contains("spawn")
        || lower.Contains("load")
        || lower.Contains("fade")
        || lower.Contains("enter")
        || lower.Contains("vfx");
    }

    private static void AppendPortalFieldDiagnostics(List<string> lines, Component component)
    {
      Type type = component.GetType();
      BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      int logged = 0;

      try
      {
        FieldInfo[] fields = type.GetFields(flags);
        foreach (var field in fields)
        {
          if (field == null || field.FieldType != typeof(float))
          {
            continue;
          }

          if (!ShouldLogPortalField(field.Name))
          {
            continue;
          }

          float value = (float)field.GetValue(component);
          lines.Add($"    field {field.Name}={value:0.###}");
          logged++;
          if (logged >= PortalDiagnosticMaxFields)
          {
            return;
          }
        }
      }
      catch (Exception)
      {
      }

      try
      {
        PropertyInfo[] properties = type.GetProperties(flags);
        foreach (var property in properties)
        {
          if (property == null || property.PropertyType != typeof(float))
          {
            continue;
          }

          if (!property.CanRead || property.GetIndexParameters().Length != 0)
          {
            continue;
          }

          if (!ShouldLogPortalField(property.Name))
          {
            continue;
          }

          float value = (float)property.GetValue(component, null);
          lines.Add($"    property {property.Name}={value:0.###}");
          logged++;
          if (logged >= PortalDiagnosticMaxFields)
          {
            return;
          }
        }
      }
      catch (Exception)
      {
      }
    }

    private static string ResolveObjectName(object obj)
    {
      if (obj == null)
      {
        return "null";
      }

      try
      {
        PropertyInfo? nameProperty = obj.GetType().GetProperty("name",
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        string? name = nameProperty?.GetValue(obj, null) as string;
        if (!string.IsNullOrWhiteSpace(name))
        {
          return name;
        }
      }
      catch (Exception)
      {
      }

      return obj.GetType().Name;
    }

    private static float ResolveFloatProperty(object obj, string name)
    {
      if (obj == null)
      {
        return 0f;
      }

      try
      {
        PropertyInfo? property = obj.GetType().GetProperty(name,
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null && property.PropertyType == typeof(float))
        {
          return (float)property.GetValue(obj, null);
        }
      }
      catch (Exception)
      {
      }

      return 0f;
    }

    private static bool ShouldLogPortalField(string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      string normalized = name.Replace("_", string.Empty).ToLowerInvariant();
      return normalized.Contains("time")
        || normalized.Contains("duration")
        || normalized.Contains("delay")
        || normalized.Contains("speed")
        || normalized.Contains("anim");
    }

    private static bool TryApplyPortalSpeedup()
    {
      int portalObjects = 0;
      int animatorsAdjusted = 0;
      int fieldsAdjusted = 0;
      int skipFlagsAdjusted = 0;
      bool changed = false;
      var seenObjects = new HashSet<int>();

      try
      {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
          if (obj == null || !IsSceneObject(obj))
          {
            continue;
          }

          if (!LooksLikePortal(obj.name) && !HasPortalComponent(obj))
          {
            continue;
          }

          if (!seenObjects.Add(obj.GetInstanceID()))
          {
            continue;
          }

          portalObjects++;

          Component[] components = obj.GetComponentsInChildren<Component>(true);
          foreach (var component in components)
          {
            if (component == null)
            {
              continue;
            }

            if (TrySetAnimatorSpeed(component, PortalAnimatorSpeedMultiplier))
            {
              animatorsAdjusted++;
              changed = true;
            }

            if (!LooksLikePortal(component.GetType().Name))
            {
              continue;
            }

            if (TrySetPortalSkipFlag(component))
            {
              skipFlagsAdjusted++;
              changed = true;
            }

            fieldsAdjusted += ScalePortalTimings(component, PortalTimeScale);
          }
        }
      }
      catch (Exception ex)
      {
        MelonLogger.Msg($"{LogPrefix} Portal speedup scan failed: {ex.GetType().Name} {ex.Message}");
      }

      if (animatorsAdjusted > 0 || fieldsAdjusted > 0 || skipFlagsAdjusted > 0)
      {
        MelonLogger.Msg(
          $"{LogPrefix} Portal speedup applied: portals={portalObjects} animators={animatorsAdjusted} " +
          $"fieldsScaled={fieldsAdjusted} skipFlags={skipFlagsAdjusted}.");
        return true;
      }

      if (changed)
      {
        return true;
      }

      return false;
    }

    private static bool TrySetAnimatorSpeed(Component component, float speed)
    {
      if (component == null)
      {
        return false;
      }

      Type type = component.GetType();
      if (!string.Equals(type.Name, "Animator", StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      try
      {
        PropertyInfo? speedProperty = type.GetProperty("speed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (speedProperty != null && speedProperty.PropertyType == typeof(float) && speedProperty.CanWrite)
        {
          float current = (float)speedProperty.GetValue(component, null);
          if (current < speed)
          {
            speedProperty.SetValue(component, speed, null);
            return true;
          }
        }
      }
      catch (Exception)
      {
      }

      try
      {
        FieldInfo? speedField = type.GetField("speed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (speedField != null && speedField.FieldType == typeof(float))
        {
          float current = (float)speedField.GetValue(component);
          if (current < speed)
          {
            speedField.SetValue(component, speed);
            return true;
          }
        }
      }
      catch (Exception)
      {
      }

      return false;
    }

    private static bool HasPortalComponent(GameObject obj)
    {
      try
      {
        Component[] components = obj.GetComponents<Component>();
        foreach (var component in components)
        {
          if (component == null)
          {
            continue;
          }

          if (LooksLikePortal(component.GetType().Name))
          {
            return true;
          }
        }
      }
      catch (Exception)
      {
      }

      return false;
    }

    private static int ScalePortalTimings(Component component, float scale)
    {
      int adjusted = 0;
      Type type = component.GetType();
      BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      try
      {
        FieldInfo[] fields = type.GetFields(flags);
        foreach (var field in fields)
        {
          if (field == null || field.FieldType != typeof(float))
          {
            continue;
          }

          if (!ShouldScalePortalField(field.Name))
          {
            continue;
          }

          float value = (float)field.GetValue(component);
          float scaled = Mathf.Max(0.05f, value * scale);
          if (Mathf.Abs(scaled - value) < 0.0001f)
          {
            continue;
          }

          field.SetValue(component, scaled);
          adjusted++;
        }
      }
      catch (Exception)
      {
      }

      try
      {
        PropertyInfo[] properties = type.GetProperties(flags);
        foreach (var property in properties)
        {
          if (property == null || property.PropertyType != typeof(float))
          {
            continue;
          }

          if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length != 0)
          {
            continue;
          }

          if (!ShouldScalePortalField(property.Name))
          {
            continue;
          }

          float value = (float)property.GetValue(component, null);
          float scaled = Mathf.Max(0.05f, value * scale);
          if (Mathf.Abs(scaled - value) < 0.0001f)
          {
            continue;
          }

          property.SetValue(component, scaled, null);
          adjusted++;
        }
      }
      catch (Exception)
      {
      }

      return adjusted;
    }

    private static bool TrySetPortalSkipFlag(Component component)
    {
      if (component == null)
      {
        return false;
      }

      Type type = component.GetType();
      BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      bool changed = false;

      try
      {
        FieldInfo[] fields = type.GetFields(flags);
        foreach (var field in fields)
        {
          if (field == null || field.FieldType != typeof(bool))
          {
            continue;
          }

          if (!ShouldSetPortalSkipFlag(field.Name))
          {
            continue;
          }

          object? boxed = field.GetValue(component);
          if (boxed is bool current && !current)
          {
            field.SetValue(component, true);
            changed = true;
          }
        }
      }
      catch (Exception)
      {
      }

      try
      {
        PropertyInfo[] properties = type.GetProperties(flags);
        foreach (var property in properties)
        {
          if (property == null || property.PropertyType != typeof(bool))
          {
            continue;
          }

          if (!property.CanRead || !property.CanWrite || property.GetIndexParameters().Length != 0)
          {
            continue;
          }

          if (!ShouldSetPortalSkipFlag(property.Name))
          {
            continue;
          }

          object? boxed = property.GetValue(component, null);
          if (boxed is bool current && !current)
          {
            property.SetValue(component, true, null);
            changed = true;
          }
        }
      }
      catch (Exception)
      {
      }

      return changed;
    }

    private static bool ShouldScalePortalField(string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      string normalized = name.Replace("_", string.Empty).ToLowerInvariant();
      return normalized.Contains("opentime")
        || normalized.Contains("closetime")
        || normalized.Contains("movetime")
        || normalized.Contains("scaletime")
        || normalized.Contains("animationtime")
        || normalized.Contains("animtime")
        || normalized.Contains("timebetweentiers");
    }

    private static bool ShouldSetPortalSkipFlag(string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      string normalized = name.Replace("_", string.Empty).ToLowerInvariant();
      return normalized.Contains("skipportalanimation")
        || (normalized.Contains("skipportal") && normalized.Contains("animation"));
    }

    private static bool LooksLikePortal(string? value)
    {
      return !string.IsNullOrWhiteSpace(value)
        && value.IndexOf("Portal", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool SettingsEnabled => _prefEnabled?.Value ?? true;

    private static int MinLegendaryVendors
      => Mathf.Clamp(_prefMinLegendaryVendors?.Value ?? DefaultMinLegendaryVendors, 0, MaxConditionCount);

    private static int MinEpicVendors
      => Mathf.Clamp(_prefMinEpicVendors?.Value ?? DefaultMinEpicVendors, 0, MaxConditionCount);

    private static int MinMoai
      => Mathf.Clamp(_prefMinMoai?.Value ?? DefaultMinMoai, 0, MaxConditionCount);

    private static int MinMicrowaves
      => Mathf.Clamp(_prefMinMicrowaves?.Value ?? DefaultMinMicrowaves, 0, MaxConditionCount);

    private static int MinEpicMicrowaves
      => Mathf.Clamp(_prefMinEpicMicrowaves?.Value ?? DefaultMinEpicMicrowaves, 0, MaxConditionCount);

    private static int MinGreenCreditCards
      => Mathf.Clamp(_prefMinGreenCreditCards?.Value ?? DefaultMinGreenCreditCards, 0, MaxConditionCount);

    private static int MinSoulHarvesters
      => Mathf.Clamp(_prefMinSoulHarvesters?.Value ?? DefaultMinSoulHarvesters, 0, MaxConditionCount);

    private static int MainMenuButtonX
      => Mathf.Clamp(_prefMainMenuButtonX?.Value ?? DefaultMainMenuButtonX, 0, 10000);

    private static int MainMenuButtonY
      => Mathf.Clamp(_prefMainMenuButtonY?.Value ?? DefaultMainMenuButtonY, 0, 10000);

    private static int SettingsMenuX
      => Mathf.Clamp(_prefSettingsMenuX?.Value ?? DefaultSettingsMenuX, 0, 10000);

    private static int SettingsMenuY
      => Mathf.Clamp(_prefSettingsMenuY?.Value ?? DefaultSettingsMenuY, 0, 10000);

    private static bool MeetsMinimum(int count, int minimum)
    {
      return minimum <= 0 || count >= minimum;
    }

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
        _smallButtonStyle.margin = new RectOffset { left = 2, right = 2, top = 2, bottom = 2 };
        _smallButtonStyle.padding = new RectOffset { left = 4, right = 4, top = 4, bottom = 4 };
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
      const float rowHeight = 26f;
      const int countRows = 7;
      const int toggleRows = 2;

      float requiredHeight = GetSettingsPanelHeight(countRows, toggleRows, lineHeight, spacing, buttonHeight, rowHeight);
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
        GUI.Label(new Rect(x, y, width, lineHeight), "Auto-restart conditions", _headerStyle);
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

      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum legendary vendors", MinLegendaryVendors, MaxConditionCount, _prefMinLegendaryVendors);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum epic vendors", MinEpicVendors, MaxConditionCount, _prefMinEpicVendors);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum moai", MinMoai, MaxConditionCount, _prefMinMoai);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum soul harvesters", MinSoulHarvesters, MaxConditionCount, _prefMinSoulHarvesters);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum microwaves", MinMicrowaves, MaxConditionCount, _prefMinMicrowaves);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum epic microwaves", MinEpicMicrowaves, MaxConditionCount, _prefMinEpicMicrowaves);
      y = DrawCountRow(x, y, width, lineHeight, spacing,
        "Minimum green credit cards", MinGreenCreditCards, MaxConditionCount, _prefMinGreenCreditCards);
    }

    private float DrawCountRow(float x, float y, float width, float lineHeight, float spacing,
      string label, int value, int maxValue, MelonPreferences_Entry<int>? entry)
    {
      if (_headerStyle != null)
      {
        GUI.Label(new Rect(x, y, width, lineHeight), label, _headerStyle);
      }

      y += lineHeight + spacing;

      float smallButtonWidth = 32f;
      float smallButtonHeight = 26f;
      float valueWidth = 40f;
      float rowWidth = (smallButtonWidth * 2f) + valueWidth + (spacing * 2f);
      float rowX = x + Mathf.Max(0f, (width - rowWidth) * 0.5f);
      float rowY = y;
      int nextValue = value;

      if (_smallButtonStyle != null && GUI.Button(new Rect(rowX, rowY, smallButtonWidth, smallButtonHeight), "-", _smallButtonStyle))
      {
        nextValue = Mathf.Clamp(value - 1, 0, maxValue);
      }

      if (_headerStyle != null)
      {
        GUI.Label(new Rect(rowX + smallButtonWidth + spacing, rowY + 2f, valueWidth, smallButtonHeight), value.ToString(), _headerStyle);
      }

      if (_smallButtonStyle != null && GUI.Button(new Rect(rowX + smallButtonWidth + spacing + valueWidth + spacing, rowY, smallButtonWidth, smallButtonHeight), "+", _smallButtonStyle))
      {
        nextValue = Mathf.Clamp(value + 1, 0, maxValue);
      }

      if (nextValue != value && entry != null)
      {
        entry.Value = nextValue;
        MelonPreferences.Save();
      }

      return y + smallButtonHeight + spacing;
    }

    private float GetSettingsPanelHeight(int countRows, int toggleRows, float lineHeight, float spacing,
      float buttonHeight, float rowHeight)
    {
      float height = 28f;
      height += lineHeight + spacing;
      height += toggleRows * (buttonHeight + spacing);
      height += countRows * (lineHeight + spacing + rowHeight + spacing);
      height += spacing;
      return height;
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

      int minGreenCards = MinGreenCreditCards;
      int minSoulHarvesters = MinSoulHarvesters;
      bool requiresItems = minGreenCards > 0 || minSoulHarvesters > 0;

      VendorScanResult scan = default;
      bool sawItems = !requiresItems;
      bool loggedItemTiming = false;
      float hudReadyTime = _lastHudReadyTime;
      float itemWaitStart = hudReadyTime > 0f ? hudReadyTime : Time.realtimeSinceStartup;
      bool itemWaitExceeded = false;
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
      }

      if (scan.VendorCount == 0)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) No vendors detected; restarting run.");
        _lastBreadcrumbScan = scan;
        _lastBreadcrumbReason = $"{reason}:no-vendors";
        yield return RestartRun();
        yield break;
      }

      if (requiresItems && !sawItems)
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

      int minLegendaryVendors = MinLegendaryVendors;
      int minEpicVendors = MinEpicVendors;
      int minMoai = MinMoai;
      int minMicrowaves = MinMicrowaves;
      int minEpicMicrowaves = MinEpicMicrowaves;

      bool meetsLegendaryVendors = MeetsMinimum(scan.LegendaryVendorTierCount, minLegendaryVendors);
      bool meetsEpicVendors = MeetsMinimum(scan.EpicVendorTierCount, minEpicVendors);
      bool meetsMoai = MeetsMinimum(scan.MoaiCount, minMoai);
      bool meetsMicrowaves = MeetsMinimum(scan.MicrowaveCount, minMicrowaves);
      bool meetsEpicMicrowaves = MeetsMinimum(scan.EpicMicrowaveCount, minEpicMicrowaves);
      bool meetsGreenCards = MeetsMinimum(scan.GreenCreditCardCount, minGreenCards);
      bool meetsSoulHarvester = MeetsMinimum(scan.SoulHarvesterCount, minSoulHarvesters);
      bool meetsAll = meetsLegendaryVendors && meetsEpicVendors && meetsMoai && meetsMicrowaves
        && meetsEpicMicrowaves && meetsGreenCards && meetsSoulHarvester;
      bool pauseOpen = IsPauseMenuOpen();

      MelonLogger.Msg(
        $"{LogPrefix} ({reason}) vendors={scan.VendorCount} done={scan.DoneVendorCount} " +
        $"vendorTierLegendary={scan.LegendaryVendorTierCount} vendorTierEpic={scan.EpicVendorTierCount} " +
        $"moai={scan.MoaiCount} microwaves={scan.MicrowaveCount} epicMicrowaves={scan.EpicMicrowaveCount} " +
        $"soulHarvester={scan.SoulHarvesterCount} greenCards={scan.GreenCreditCardCount} " +
        $"items={scan.ItemCount} legendaryItems={scan.LegendaryItemCount} " +
        $"minLegendaryVendors={minLegendaryVendors} minEpicVendors={minEpicVendors} minMoai={minMoai} " +
        $"minSoulHarvesters={minSoulHarvesters} minMicrowaves={minMicrowaves} " +
        $"minEpicMicrowaves={minEpicMicrowaves} minGreenCards={minGreenCards}");

      if (meetsAll)
      {
        if (pauseOpen)
        {
          MelonLogger.Msg($"{LogPrefix} ({reason}) Vendor conditions met; pause menu already open.");
          yield break;
        }

        if (GameApi.TryOpenPauseMenu(out string pauseSource))
        {
          MelonLogger.Msg($"{LogPrefix} Vendor conditions met. Opened pause menu via {pauseSource}.");
        }
        else
        {
          MelonLogger.Msg($"{LogPrefix} Vendor conditions met. Pressing ESC.");
          yield return InputApi.PressKey("ESC", EscTapSeconds);
        }
        yield break;
      }

      if (pauseOpen)
      {
        MelonLogger.Msg($"{LogPrefix} ({reason}) Pause menu open; blocking auto-restart.");
        yield break;
      }

      MelonLogger.Msg($"{LogPrefix} Vendor conditions not met. Restarting run.");
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
      int minGreenCards = MinGreenCreditCards;
      int minSoulHarvesters = MinSoulHarvesters;
      bool bothItemMins = minGreenCards > 0 && minSoulHarvesters > 0;
      MelonLogger.Msg(
        $"{LogPrefix} Restart streak={_restartStreakCount} window={streakNow - _restartStreakStartAt:0.0}s " +
        $"reason={_lastBreadcrumbReason} minGreenCards={minGreenCards} minSoulHarvesters={minSoulHarvesters} " +
        $"bothItemMins={bothItemMins}.");
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
