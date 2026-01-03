using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "0.3.8", "Strei")]
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
    private const int DefaultMinLegendaryVendors = 1;
    private const int DefaultMinEpicVendors = 0;
    private const int DefaultMinMoai = 0;
    private const int DefaultMinMicrowaves = 0;
    private const int DefaultMinEpicMicrowaves = 0;
    private const int DefaultMinGreenCreditCards = 0;
    private const int DefaultMainMenuButtonX = 20;
    private const int DefaultMainMenuButtonY = 80;
    private const int DefaultSettingsMenuX = 680;
    private const int DefaultSettingsMenuY = 270;
    private const int MaxConditionCount = 20;
    private const float MainMenuProbeIntervalSeconds = 0.5f;
    private const float MainMenuButtonSpacing = 8f;
    private const float PauseUiPollSeconds = 0.5f;

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<bool>? _prefSoundEnabled;
    private static MelonPreferences_Entry<bool>? _prefRequireSoulHarvester;
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
    private static float _soundOnVolume = -1f;
    private static Button? _mainMenuSettingsButton;
    private static float _mainMenuLastProbe;
    private static bool _mainMenuSettingsLogged;
    private static bool _mainMenuSettingsMissingLogged;
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

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAlpha", "Bonk Ultra");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-restart");
      _prefSoundEnabled = _prefs.CreateEntry("SoundEnabled", true, "Game sound on/off");
      _prefRequireSoulHarvester = _prefs.CreateEntry("RequireSoulHarvester", false, "Require Soul Harvester");
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

      if (_settingsVisible)
      {
        _settingsRect.x = SettingsMenuX;
        _settingsRect.y = SettingsMenuY;
        _settingsRect = ClampWindowToScreen(_settingsRect);
        DrawSettingsPanel(_settingsRect);
      }
    }

    private static bool TryGetMainMenuSettingsRect(out Rect rect)
    {
      rect = default;

      float now = Time.realtimeSinceStartup;
      if (_mainMenuSettingsButton == null || now - _mainMenuLastProbe > MainMenuProbeIntervalSeconds)
      {
        _mainMenuSettingsButton = FindMainMenuSettingsButton();
        _mainMenuLastProbe = now;
      }

      if (TryGetSettingsButtonRect(out rect))
      {
        return true;
      }

      if (TryGetSettingsLabelRect(out rect, out string labelInfo))
      {
        if (!_mainMenuSettingsLogged)
        {
          MelonLogger.Msg($"{LogPrefix} Main menu Settings anchor (label): {labelInfo} rect={rect.x:F0},{rect.y:F0},{rect.width:F0},{rect.height:F0}");
          _mainMenuSettingsLogged = true;
        }

        return true;
      }

      if (TryGetSettingsObjectRect(out rect, out string objectInfo))
      {
        if (!_mainMenuSettingsLogged)
        {
          MelonLogger.Msg($"{LogPrefix} Main menu Settings anchor (object): {objectInfo} rect={rect.x:F0},{rect.y:F0},{rect.width:F0},{rect.height:F0}");
          _mainMenuSettingsLogged = true;
        }

        return true;
      }

      if (!_mainMenuSettingsMissingLogged)
      {
        MelonLogger.Msg($"{LogPrefix} Main menu Settings anchor not found.");
        _mainMenuSettingsMissingLogged = true;
      }

      return false;
    }

    private static bool TryGetSettingsButtonRect(out Rect rect)
    {
      rect = default;
      if (_mainMenuSettingsButton == null)
      {
        return false;
      }

      if (!_mainMenuSettingsButton.gameObject.activeInHierarchy)
      {
        return false;
      }

      if (!TryGetButtonRect(_mainMenuSettingsButton, out Rect screenRect))
      {
        return false;
      }

      if (!_mainMenuSettingsLogged)
      {
        string label = GetButtonLabel(_mainMenuSettingsButton);
        MelonLogger.Msg($"{LogPrefix} Main menu Settings anchor: label='{label}' name='{_mainMenuSettingsButton.name}' rect={screenRect.x:F0},{screenRect.y:F0},{screenRect.width:F0},{screenRect.height:F0}");
        _mainMenuSettingsLogged = true;
      }

      rect = screenRect;
      return true;
    }

    private static bool TryGetButtonRect(Button button, out Rect rect)
    {
      rect = default;
      if (button == null)
      {
        return false;
      }

      RectTransform rectTransform = button.GetComponent<RectTransform>();
      if (rectTransform == null)
      {
        return false;
      }

      if (!TryGetRectTransformRect(rectTransform, out Rect screenRect))
      {
        return false;
      }

      rect = screenRect;
      return true;
    }

    private static bool TryGetRectTransformRect(RectTransform rectTransform, out Rect rect)
    {
      rect = default;
      if (rectTransform == null)
      {
        return false;
      }

      Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
      Camera camera = GetCanvasCamera(canvas);
      Rect screenRect = GetGuiRect(rectTransform, camera);
      if (screenRect.width <= 1f || screenRect.height <= 1f)
      {
        return false;
      }

      if (!IsRectOnScreen(screenRect))
      {
        return false;
      }

      rect = screenRect;
      return true;
    }

    private static bool IsRectOnScreen(Rect rect)
    {
      return rect.xMax > 0f && rect.yMax > 0f && rect.xMin < Screen.width && rect.yMin < Screen.height;
    }

    private static Camera GetCanvasCamera(Canvas canvas)
    {
      if (canvas == null)
      {
        return Camera.main;
      }

      if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
      {
        return null;
      }

      if (canvas.worldCamera != null)
      {
        return canvas.worldCamera;
      }

      return Camera.main;
    }

    private static bool TryGetSettingsLabelRect(out Rect rect, out string labelInfo)
    {
      rect = default;
      labelInfo = string.Empty;

      TMP_Text[] tmpLabels = Resources.FindObjectsOfTypeAll<TMP_Text>();
      if (TryGetSettingsLabelRect(tmpLabels, out rect, out labelInfo))
      {
        return true;
      }

      Text[] labels = Resources.FindObjectsOfTypeAll<Text>();
      return TryGetSettingsLabelRect(labels, out rect, out labelInfo);
    }

    private static bool TryGetSettingsLabelRect<TLabel>(TLabel[] labels, out Rect rect, out string labelInfo)
      where TLabel : Component
    {
      rect = default;
      labelInfo = string.Empty;
      Rect bestRect = default;
      string bestInfo = string.Empty;
      bool found = false;

      foreach (var label in labels)
      {
        if (label == null)
        {
          continue;
        }

        string textValue = GetLabelText(label);
        if (!LabelMatches(textValue, "Settings"))
        {
          continue;
        }

        GameObject obj = label.gameObject;
        if (obj == null || !obj.activeInHierarchy)
        {
          continue;
        }

        RectTransform rectTransform = label.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
          continue;
        }

        if (!TryGetRectTransformRect(rectTransform, out Rect labelRect))
        {
          continue;
        }

        if (labelRect.x > Screen.width * 0.5f || labelRect.y < Screen.height * 0.25f)
        {
          continue;
        }

        Rect anchorRect = FindAnchorRectFromLabel(rectTransform, labelRect);
        if (!IsRectOnScreen(anchorRect))
        {
          continue;
        }

        if (!found || anchorRect.y > bestRect.y + 1f || (Mathf.Abs(anchorRect.y - bestRect.y) < 1f && anchorRect.x < bestRect.x))
        {
          bestRect = anchorRect;
          bestInfo = $"label='{textValue}' name='{obj.name}'";
          found = true;
        }
      }

      if (found)
      {
        rect = bestRect;
        labelInfo = bestInfo;
        return true;
      }

      return false;
    }

    private static bool TryGetSettingsObjectRect(out Rect rect, out string labelInfo)
    {
      rect = default;
      labelInfo = string.Empty;
      Rect bestRect = default;
      string bestInfo = string.Empty;
      bool found = false;

      try
      {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
          if (obj == null || !obj.activeInHierarchy)
          {
            continue;
          }

          if (!LabelMatches(obj.name, "Settings") && !LabelMatches(obj.name, "Setting"))
          {
            continue;
          }

          if (!TryGetObjectRect(obj, out Rect objRect))
          {
            continue;
          }

          if (objRect.x > Screen.width * 0.5f || objRect.y < Screen.height * 0.25f)
          {
            continue;
          }

          if (!found || objRect.y > bestRect.y + 1f || (Mathf.Abs(objRect.y - bestRect.y) < 1f && objRect.x < bestRect.x))
          {
            bestRect = objRect;
            bestInfo = $"name='{obj.name}'";
            found = true;
          }
        }
      }
      catch (Exception)
      {
      }

      if (found)
      {
        rect = bestRect;
        labelInfo = bestInfo;
        return true;
      }

      return false;
    }

    private static Rect FindAnchorRectFromLabel(RectTransform labelTransform, Rect labelRect)
    {
      Rect bestRect = labelRect;
      float bestArea = labelRect.width * labelRect.height;
      RectTransform current = labelTransform;

      for (int depth = 0; depth < 6; depth++)
      {
        RectTransform parent = current.parent as RectTransform;
        if (parent == null)
        {
          break;
        }

        if (!TryGetRectTransformRect(parent, out Rect parentRect))
        {
          current = parent;
          continue;
        }

        if (parentRect.width >= labelRect.width + 40f
          && parentRect.height >= labelRect.height + 12f
          && parentRect.width <= Screen.width * 0.9f
          && parentRect.height <= Screen.height * 0.5f)
        {
          float area = parentRect.width * parentRect.height;
          if (area < bestArea)
          {
            bestArea = area;
            bestRect = parentRect;
          }
        }

        current = parent;
      }

      return bestRect;
    }

    private static bool TryGetObjectRect(GameObject obj, out Rect rect)
    {
      rect = default;
      if (obj == null)
      {
        return false;
      }

      RectTransform rectTransform = obj.GetComponent<RectTransform>();
      if (rectTransform != null && TryGetRectTransformRect(rectTransform, out Rect directRect))
      {
        rect = directRect;
        return true;
      }

      List<Rect> rects = new List<Rect>();
      RectTransform[] rectTransforms = obj.GetComponentsInChildren<RectTransform>(true);
      foreach (var childRect in rectTransforms)
      {
        if (TryGetRectTransformRect(childRect, out Rect childRectValue))
        {
          rects.Add(childRectValue);
        }
      }

      if (rects.Count == 0)
      {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
          if (TryGetRendererRect(renderer, out Rect renderRect))
          {
            rects.Add(renderRect);
          }
        }
      }

      if (rects.Count == 0)
      {
        return false;
      }

      rect = UnionRects(rects);
      return rect.width > 1f && rect.height > 1f;
    }

    private static Rect UnionRects(List<Rect> rects)
    {
      float minX = float.MaxValue;
      float minY = float.MaxValue;
      float maxX = float.MinValue;
      float maxY = float.MinValue;

      foreach (var rect in rects)
      {
        minX = Mathf.Min(minX, rect.xMin);
        minY = Mathf.Min(minY, rect.yMin);
        maxX = Mathf.Max(maxX, rect.xMax);
        maxY = Mathf.Max(maxY, rect.yMax);
      }

      if (minX == float.MaxValue)
      {
        return Rect.zero;
      }

      return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    private static bool TryGetRendererRect(Renderer renderer, out Rect rect)
    {
      rect = default;
      if (renderer == null)
      {
        return false;
      }

      Camera camera = GetAnyCamera();
      if (camera == null)
      {
        return false;
      }

      Bounds bounds = renderer.bounds;
      Vector3 center = bounds.center;
      Vector3 extents = bounds.extents;
      Vector3[] corners =
      {
        center + new Vector3(extents.x, extents.y, extents.z),
        center + new Vector3(extents.x, extents.y, -extents.z),
        center + new Vector3(extents.x, -extents.y, extents.z),
        center + new Vector3(extents.x, -extents.y, -extents.z),
        center + new Vector3(-extents.x, extents.y, extents.z),
        center + new Vector3(-extents.x, extents.y, -extents.z),
        center + new Vector3(-extents.x, -extents.y, extents.z),
        center + new Vector3(-extents.x, -extents.y, -extents.z)
      };

      Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 max = new Vector2(float.MinValue, float.MinValue);

      for (int i = 0; i < corners.Length; i++)
      {
        Vector3 screenPoint = camera.WorldToScreenPoint(corners[i]);
        min = Vector2.Min(min, screenPoint);
        max = Vector2.Max(max, screenPoint);
      }

      float width = max.x - min.x;
      float height = max.y - min.y;
      if (width <= 1f || height <= 1f)
      {
        return false;
      }

      rect = new Rect(min.x, Screen.height - max.y, width, height);
      return IsRectOnScreen(rect);
    }

    private static Camera GetAnyCamera()
    {
      Camera main = Camera.main;
      if (main != null)
      {
        return main;
      }

      Camera[] cameras = Camera.allCameras;
      if (cameras != null && cameras.Length > 0)
      {
        return cameras[0];
      }

      return null;
    }

    private static string GetLabelText(Component label)
    {
      if (label is TMP_Text tmp)
      {
        return tmp.text;
      }

      if (label is Text text)
      {
        return text.text;
      }

      return string.Empty;
    }

    private static Rect GetGuiRect(RectTransform rectTransform, Camera camera)
    {
      Vector3[] corners = new Vector3[4];
      rectTransform.GetWorldCorners(corners);
      Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
      Vector2 max = new Vector2(float.MinValue, float.MinValue);

      for (int i = 0; i < corners.Length; i++)
      {
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
        min = Vector2.Min(min, screen);
        max = Vector2.Max(max, screen);
      }

      float width = max.x - min.x;
      float height = max.y - min.y;
      float x = min.x;
      float y = Screen.height - max.y;
      return new Rect(x, y, width, height);
    }

    private static Button FindMainMenuSettingsButton()
    {
      Button bestButton = null;
      Rect bestRect = default;
      try
      {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (var button in buttons)
        {
          if (button == null)
          {
            continue;
          }

          GameObject obj = button.gameObject;
          if (obj == null || !obj.activeInHierarchy)
          {
            continue;
          }

          string label = GetButtonLabel(button);
          if (LabelMatches(label, "Settings")
            || LabelMatches(button.name, "Settings")
            || LabelMatches(obj.name, "Settings"))
          {
            if (!TryGetButtonRect(button, out Rect rect))
            {
              continue;
            }

            if (rect.y < Screen.height * 0.35f)
            {
              continue;
            }

            if (bestButton == null || rect.y > bestRect.y + 1f || (Mathf.Abs(rect.y - bestRect.y) < 1f && rect.x < bestRect.x))
            {
              bestButton = button;
              bestRect = rect;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      return bestButton;
    }

    private static string GetButtonLabel(Button button)
    {
      TMP_Text tmp = button.GetComponentInChildren<TMP_Text>(true);
      if (tmp != null)
      {
        return tmp.text;
      }

      Text text = button.GetComponentInChildren<Text>(true);
      if (text != null)
      {
        return text.text;
      }

      return string.Empty;
    }

    private static bool LabelMatches(string label, string expected)
    {
      if (string.IsNullOrWhiteSpace(label))
      {
        return false;
      }

      return label.Trim().IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
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

    private static bool RequireSoulHarvester => _prefRequireSoulHarvester?.Value ?? false;

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
      const int countRows = 6;
      const int toggleRows = 3;

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

      bool soulRequired = RequireSoulHarvester;
      string soulLabel = soulRequired ? "Soul Harvester: ON" : "Soul Harvester: OFF";
      if (_buttonStyle != null && GUI.Button(new Rect(x, y, width, buttonHeight), soulLabel, _buttonStyle))
      {
        if (_prefRequireSoulHarvester != null)
        {
          _prefRequireSoulHarvester.Value = !soulRequired;
          MelonPreferences.Save();
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

      if (GameApi.IsMainMenu())
      {
        yield break;
      }

      GameApi.StageInfo stageInfo = GameApi.GetStageInfo();
      if (stageInfo.IsBossStage)
      {
        string stageName = string.IsNullOrWhiteSpace(stageInfo.StageName) ? "unknown" : stageInfo.StageName;
        MelonLogger.Msg($"{LogPrefix} ({reason}) Boss stage detected ({stageName}); skipping auto-restart.");
        yield break;
      }

      if (stageInfo.StageTier.HasValue && stageInfo.StageTier.Value >= 2)
      {
        string source = string.IsNullOrWhiteSpace(stageInfo.StageSource) ? "unknown" : stageInfo.StageSource;
        MelonLogger.Msg($"{LogPrefix} ({reason}) Stage tier {stageInfo.StageTier.Value} ({source}); skipping auto-restart.");
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
        if (HasAnyScanData(scan))
        {
          break;
        }

        yield return TimerApi.WaitSeconds(VendorPollIntervalSeconds);
        waited += VendorPollIntervalSeconds;
      }

      if (!HasAnyScanData(scan))
      {
        scan = VendorScanner.Scan();
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
      int minGreenCards = MinGreenCreditCards;

      bool meetsLegendaryVendors = MeetsMinimum(scan.LegendaryVendorTierCount, minLegendaryVendors);
      bool meetsEpicVendors = MeetsMinimum(scan.EpicVendorTierCount, minEpicVendors);
      bool meetsMoai = MeetsMinimum(scan.MoaiCount, minMoai);
      bool meetsMicrowaves = MeetsMinimum(scan.MicrowaveCount, minMicrowaves);
      bool meetsEpicMicrowaves = MeetsMinimum(scan.EpicMicrowaveCount, minEpicMicrowaves);
      bool meetsGreenCards = MeetsMinimum(scan.GreenCreditCardCount, minGreenCards);
      bool meetsSoulHarvester = !RequireSoulHarvester || scan.SoulHarvesterCount > 0;
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
        $"minMicrowaves={minMicrowaves} minEpicMicrowaves={minEpicMicrowaves} minGreenCards={minGreenCards} " +
        $"requireSoulHarvester={RequireSoulHarvester}");

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
                if (NameEquals(itemName, SoulHarvesterName))
                {
                  result.SoulHarvesterCount++;
                }

                if (NameEquals(itemName, CreditCardName) && item.rarity == EItemRarity.Rare)
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

      private static bool NameEquals(string? name, string expected)
      {
        return !string.IsNullOrWhiteSpace(name)
          && name.Trim().Equals(expected, StringComparison.OrdinalIgnoreCase);
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
