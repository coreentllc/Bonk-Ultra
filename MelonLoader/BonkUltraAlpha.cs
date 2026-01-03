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
using Il2CppTMPro;
using UnityEngine.UI;
#endif

[assembly: MelonInfo(typeof(BonkUltraAlpha.BonkUltraAlphaMod), "Bonk Ultra, Alpha", "0.3.2", "Strei")]
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
    private const float UiSetupRetrySeconds = 1.0f;
    private const float UiSetupTimeoutSeconds = 30.0f;

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<int>? _prefMinLegendary;
    private GameObject? _pauseUiRoot;
    private Button? _bonkButton;
    private GameObject? _settingsPanel;
    private Button? _autoRestartButton;
    private float _nextUiSetupTime;
    private float _uiSetupStartTime;
    private bool _uiSetupComplete;
    private bool _uiSetupLoggedMissing;

    private int _runCheckToken;
    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAlpha", "Bonk Ultra");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-restart");
      _prefMinLegendary = _prefs.CreateEntry("MinLegendaryItems", DefaultMinLegendaryItems, "Minimum legendary items");
      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnUpdate()
    {
      if (_uiSetupComplete)
      {
        return;
      }

      float now = Time.realtimeSinceStartup;
      if (_uiSetupStartTime <= 0f)
      {
        _uiSetupStartTime = now;
      }

      if (now < _nextUiSetupTime)
      {
        return;
      }

      if (TrySetupPauseMenuUi())
      {
        _uiSetupComplete = true;
        return;
      }

      if (!_uiSetupLoggedMissing && now - _uiSetupStartTime > UiSetupTimeoutSeconds)
      {
        MelonLogger.Msg($"{LogPrefix} Pause menu UI not ready yet; will keep trying.");
        _uiSetupLoggedMissing = true;
      }

      _nextUiSetupTime = now + UiSetupRetrySeconds;
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      _runCheckToken++;
      ResetPauseUiState();
      MelonCoroutines.Start(EvaluateRun(_runCheckToken, "auto"));
    }

    private void ResetPauseUiState()
    {
      _pauseUiRoot = null;
      _bonkButton = null;
      _settingsPanel = null;
      _autoRestartButton = null;
      _uiSetupComplete = false;
      _uiSetupLoggedMissing = false;
      _uiSetupStartTime = 0f;
      _nextUiSetupTime = 0f;
    }

    private static bool SettingsEnabled => _prefEnabled?.Value ?? true;

    private static int MinLegendaryItems
      => Mathf.Clamp(_prefMinLegendary?.Value ?? DefaultMinLegendaryItems, 0, MaxLegendaryItems);

    private bool TrySetupPauseMenuUi()
    {
      Button? resumeButton = FindMenuButton(new[] { "Resume", "Continue" });
      if (resumeButton == null)
      {
        return false;
      }

      Transform parent = resumeButton.transform.parent;
      _pauseUiRoot = GetUiRoot(resumeButton);
      Button? exitButton = FindMenuButton(
        new[] { "Exit", "Quit", "Main Menu", "Exit to Menu", "Quit to Menu" },
        parent);

      if (_bonkButton == null)
      {
        _bonkButton = _pauseUiRoot != null ? FindButtonByName(_pauseUiRoot, "BonkUltraButton") : null;
      }

      if (_bonkButton == null)
      {
        GameObject clone = UnityEngine.Object.Instantiate(resumeButton.gameObject, parent, false);
        clone.name = "BonkUltraButton";
        clone.transform.SetSiblingIndex(GetInsertIndex(exitButton, parent));
        _bonkButton = clone.GetComponent<Button>();
      }

      if (_bonkButton != null)
      {
        ConfigureBonkButton(_bonkButton, exitButton, resumeButton);
      }

      if (_settingsPanel == null)
      {
        _settingsPanel = _pauseUiRoot != null ? FindChildByName(_pauseUiRoot, "BonkUltraSettings") : null;
      }

      if (_settingsPanel == null && _bonkButton != null)
      {
        _settingsPanel = CreateSettingsPanel(parent, _bonkButton.GetComponent<RectTransform>());
      }

      if (_settingsPanel != null && _autoRestartButton == null)
      {
        _autoRestartButton = FindButtonByName(_settingsPanel, "BonkUltraAutoRestart");
      }

      if (_settingsPanel != null && _autoRestartButton == null)
      {
        _autoRestartButton = CreateAutoRestartButton(resumeButton, _settingsPanel.transform);
      }

      if (_autoRestartButton != null)
      {
        ConfigureAutoRestartButton(_autoRestartButton);
      }

      UpdateAutoRestartLabel();
      return _bonkButton != null && _settingsPanel != null && _autoRestartButton != null;
    }

    private static GameObject GetUiRoot(Button button)
    {
      return button.transform.root.gameObject;
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

    private static Button? FindMenuButton(string[] labels, Transform? requiredParent = null)
    {
      Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
      foreach (var button in buttons)
      {
        if (button == null)
        {
          continue;
        }

        GameObject obj = button.gameObject;
        if (!obj.activeInHierarchy)
        {
          continue;
        }

        if (!IsSceneObject(obj))
        {
          continue;
        }

        if (requiredParent != null && button.transform.parent != requiredParent)
        {
          continue;
        }

        string? text = GetButtonLabel(button);
        if (!string.IsNullOrWhiteSpace(text))
        {
          string trimmed = text.Trim();
          foreach (string candidate in labels)
          {
            if (trimmed.Equals(candidate, StringComparison.OrdinalIgnoreCase))
            {
              return button;
            }
          }
        }

        foreach (string candidate in labels)
        {
          if (obj.name.IndexOf(candidate, StringComparison.OrdinalIgnoreCase) >= 0)
          {
            return button;
          }
        }
      }

      return null;
    }

    private static Button? FindButtonByName(GameObject root, string name)
    {
      foreach (Button button in root.GetComponentsInChildren<Button>(true))
      {
        if (button != null && button.name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          return button;
        }
      }

      return null;
    }

    private static GameObject? FindChildByName(GameObject root, string name)
    {
      foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
      {
        if (child != null && child.name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          return child.gameObject;
        }
      }

      return null;
    }

    private static string? GetButtonLabel(Button button)
    {
      TMP_Text[] tmpLabels = button.GetComponentsInChildren<TMP_Text>(true);
      foreach (var tmp in tmpLabels)
      {
        if (tmp != null && !string.IsNullOrWhiteSpace(tmp.text))
        {
          return tmp.text;
        }
      }

      Text[] uiLabels = button.GetComponentsInChildren<Text>(true);
      foreach (var label in uiLabels)
      {
        if (label != null && !string.IsNullOrWhiteSpace(label.text))
        {
          return label.text;
        }
      }

      return null;
    }

    private static Component? FindTextComponent(GameObject obj)
    {
      TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>(true);
      if (tmp != null)
      {
        return tmp;
      }

      Text ui = obj.GetComponentInChildren<Text>(true);
      return ui;
    }

    private static string? GetText(Component textComponent)
    {
      if (textComponent is TMP_Text tmp)
      {
        return tmp.text;
      }

      if (textComponent is Text ui)
      {
        return ui.text;
      }

      return null;
    }

    private static void SetText(Component? textComponent, string text, Color? color)
    {
      if (textComponent == null)
      {
        return;
      }

      if (textComponent is TMP_Text tmp)
      {
        tmp.text = text;
        if (color.HasValue)
        {
          tmp.color = color.Value;
        }
        return;
      }

      if (textComponent is Text ui)
      {
        ui.text = text;
        if (color.HasValue)
        {
          ui.color = color.Value;
        }
      }
    }

    private static void ApplyLegendaryButtonStyle(Button button, Component? textComponent)
    {
      if (button == null)
      {
        return;
      }

      Image? image = button.GetComponent<Image>();
      if (image != null)
      {
        image.color = LegendaryYellow;
      }

      var colors = button.colors;
      colors.normalColor = LegendaryYellow;
      colors.highlightedColor = new Color(0.98f, 0.86f, 0.28f, 1f);
      colors.pressedColor = new Color(0.82f, 0.7f, 0.14f, 1f);
      colors.selectedColor = LegendaryYellow;
      button.colors = colors;

      string label = textComponent != null ? GetText(textComponent) ?? string.Empty : string.Empty;
      SetText(textComponent, label, LegendaryText);
    }

    private void ConfigureBonkButton(Button button, Button? exitButton, Button resumeButton)
    {
      StripButtonBehaviours(button.gameObject);
      ResetButtonEvents(button);
      button.onClick.AddListener((UnityEngine.Events.UnityAction)ToggleSettingsPanel);
      Component? textComponent = FindTextComponent(button.gameObject);
      SetText(textComponent, "Bonk Ultra", LegendaryText);
      ApplyLegendaryButtonStyle(button, textComponent);
      AlignButtonBelowExitIfNeeded(button, exitButton, resumeButton);
    }

    private void ConfigureAutoRestartButton(Button button)
    {
      StripButtonBehaviours(button.gameObject);
      ResetButtonEvents(button);
      button.onClick.AddListener((UnityEngine.Events.UnityAction)ToggleAutoRestart);
    }

    private static void ResetButtonEvents(Button button)
    {
      button.onClick = new Button.ButtonClickedEvent();
    }

    private static void StripButtonBehaviours(GameObject root)
    {
      StripComponentsRecursive(root.transform);
    }

    private static void StripComponentsRecursive(Transform root)
    {
      Component[] components = root.GetComponents<Component>();
      foreach (var component in components)
      {
        if (component == null)
        {
          continue;
        }

        if (IsAllowedComponent(component))
        {
          continue;
        }

        UnityEngine.Object.Destroy(component);
      }

      for (int i = 0; i < root.childCount; i++)
      {
        Transform child = root.GetChild(i);
        if (child != null)
        {
          StripComponentsRecursive(child);
        }
      }
    }

    private static bool IsAllowedComponent(Component component)
    {
      if (component is Transform || component is RectTransform || component is CanvasRenderer)
      {
        return true;
      }

      string ns = component.GetType().Namespace ?? string.Empty;
      if (ns.StartsWith("UnityEngine.UI", StringComparison.Ordinal)
        || ns.StartsWith("Il2CppTMPro", StringComparison.Ordinal))
      {
        return true;
      }

      return component is TMP_Text || component is Text;
    }

    private static int GetInsertIndex(Button? exitButton, Transform parent)
    {
      if (exitButton != null && exitButton.transform.parent == parent)
      {
        return exitButton.transform.GetSiblingIndex() + 1;
      }

      return parent.childCount;
    }

    private static void AlignButtonBelowExitIfNeeded(Button bonkButton, Button? exitButton, Button resumeButton)
    {
      Transform parent = bonkButton.transform.parent;
      if (parent.GetComponent<VerticalLayoutGroup>() != null)
      {
        return;
      }

      RectTransform? exitRect = exitButton != null ? exitButton.GetComponent<RectTransform>() : null;
      RectTransform? resumeRect = resumeButton.GetComponent<RectTransform>();
      RectTransform? bonkRect = bonkButton.GetComponent<RectTransform>();
      if (exitRect == null || resumeRect == null || bonkRect == null)
      {
        return;
      }

      float spacing = Mathf.Abs(resumeRect.anchoredPosition.y - exitRect.anchoredPosition.y);
      if (spacing <= 0.01f)
      {
        spacing = resumeRect.rect.height + 8f;
      }

      bonkRect.anchoredPosition = new Vector2(exitRect.anchoredPosition.x, exitRect.anchoredPosition.y - spacing);
    }

    private GameObject CreateSettingsPanel(Transform parent, RectTransform? buttonRect)
    {
      var panel = new GameObject("BonkUltraSettings");
      panel.transform.SetParent(parent, false);
      var panelRect = panel.AddComponent<RectTransform>();
      var panelImage = panel.AddComponent<Image>();
      var layoutElement = panel.AddComponent<LayoutElement>();

      if (layoutElement != null)
      {
        layoutElement.ignoreLayout = true;
      }

      if (panelImage != null)
      {
        panelImage.color = PanelBackground;
      }

      if (buttonRect != null)
      {
        float width = buttonRect.rect.width > 0f ? buttonRect.rect.width : buttonRect.sizeDelta.x;
        float height = buttonRect.rect.height > 0f ? buttonRect.rect.height : buttonRect.sizeDelta.y;
        if (width <= 0f)
        {
          width = 240f;
        }

        if (height <= 0f)
        {
          height = 48f;
        }

        panelRect.anchorMin = buttonRect.anchorMin;
        panelRect.anchorMax = buttonRect.anchorMax;
        panelRect.pivot = new Vector2(0f, 0.5f);
        panelRect.sizeDelta = new Vector2(width * 1.2f, height * 1.4f);
        panelRect.anchoredPosition = buttonRect.anchoredPosition + new Vector2(width + 20f, 0f);
      }
      else
      {
        panelRect.sizeDelta = new Vector2(260f, 80f);
      }

      panel.SetActive(false);
      return panel;
    }

    private Button? CreateAutoRestartButton(Button templateButton, Transform parent)
    {
      GameObject clone = UnityEngine.Object.Instantiate(templateButton.gameObject, parent, false);
      clone.name = "BonkUltraAutoRestart";
      Button? button = clone.GetComponent<Button>();
      if (button == null)
      {
        return null;
      }

      RectTransform? buttonRect = button.GetComponent<RectTransform>();
      if (buttonRect != null)
      {
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = Vector2.zero;
      }
      return button;
    }

    private void ToggleSettingsPanel()
    {
      if (_settingsPanel == null)
      {
        return;
      }

      bool newState = !_settingsPanel.activeSelf;
      _settingsPanel.SetActive(newState);
      if (newState)
      {
        UpdateAutoRestartLabel();
      }
    }

    private void ToggleAutoRestart()
    {
      if (_prefEnabled == null)
      {
        return;
      }

      _prefEnabled.Value = !SettingsEnabled;
      MelonPreferences.Save();
      UpdateAutoRestartLabel();
    }

    private void UpdateAutoRestartLabel()
    {
      if (_autoRestartButton == null)
      {
        return;
      }

      Component? textComponent = FindTextComponent(_autoRestartButton.gameObject);
      if (textComponent == null)
      {
        return;
      }

      string label = SettingsEnabled ? "Auto Restart On" : "Auto Restart Off";
      SetText(textComponent, label, null);
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
