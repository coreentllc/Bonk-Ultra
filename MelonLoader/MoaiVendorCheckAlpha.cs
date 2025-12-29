using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
#endif

[assembly: MelonInfo(typeof(Megabonk.MoaiVendorCheckAlphaMod), "MoaiVendorCheck Alpha", "1.0.0-alpha", "OpenAI")]
[assembly: MelonGame(null, "Megabonk")]

namespace Megabonk
{
  public sealed class MoaiVendorCheckAlphaMod : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private const int TargetRunTier = 3;
    private const int TargetStageTier = 1;
    private const float LoadPollIntervalSeconds = 0.5f;
    private const float PostLoadDelaySeconds = 0.5f;
    private const float MaxLoadWaitSeconds = 30f;
    private const float RetryDelaySeconds = 1f;
    private const int MaxAttempts = 60;
    private const float RestartHoldSeconds = 0.6f;
    private const int RestartAttempts = 3;
    private const float RestartRetryDelaySeconds = 1.0f;

    private static ConditionSettings? _settings;
    private static Rect _settingsWindowRect = new Rect(20f, 20f, 320f, 270f);
    private static bool _settingsVisible = true;
    private static GuiTheme? _theme;

    private int _runCheckToken;

    public override void OnInitializeMelon()
    {
      _settings = ConditionSettings.Create();
    }

    public override void OnGUI()
    {
      if (!GameApi.IsMainMenu())
      {
        return;
      }

      DrawSettingsWindow();
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
      _runCheckToken++;
      MelonCoroutines.Start(HandleRunLoaded(_runCheckToken));
    }

    private IEnumerator HandleRunLoaded(int token)
    {
      yield return WaitForLoadReady();

      bool loggedWaiting = false;
      bool loggedTier = false;

      for (int attempt = 0; attempt < MaxAttempts; attempt++)
      {
        if (token != _runCheckToken)
        {
          yield break;
        }

        if (GameApi.IsMainMenu())
        {
          yield break;
        }

        GameApi.TierInfo tierInfo = GameApi.GetTierInfo();
        if (!tierInfo.RunTier.HasValue || !tierInfo.StageTier.HasValue)
        {
          if (!loggedWaiting)
          {
            string runTierValue = tierInfo.RunTier.HasValue ? tierInfo.RunTier.Value.ToString() : "unknown";
            MelonLogger.Msg($"[SeedCheck] Waiting for tier info (run tier {runTierValue}).");
            loggedWaiting = true;
          }

          if (!tierInfo.StageTier.HasValue)
          {
            GameApi.LogStageTierCandidatesOnce();
          }

          yield return TimerApi.WaitSeconds(RetryDelaySeconds);
          continue;
        }

        if (!loggedTier)
        {
          string source = string.IsNullOrWhiteSpace(tierInfo.StageSource) ? "unknown" : tierInfo.StageSource;
          MelonLogger.Msg($"[SeedCheck] Run tier {tierInfo.RunTier.Value}, stage tier {tierInfo.StageTier.Value} (source: {source}).");
          loggedTier = true;
        }

        if (tierInfo.RunTier.Value != TargetRunTier)
        {
          MelonLogger.Msg($"[SeedCheck] Run tier {tierInfo.RunTier.Value} is not target tier {TargetRunTier}. Skipping.");
          yield break;
        }

        if (tierInfo.StageTier.Value != TargetStageTier)
        {
          MelonLogger.Msg($"[SeedCheck] Stage tier {tierInfo.StageTier.Value} is not target tier {TargetStageTier}. Skipping.");
          yield break;
        }

        int? seed = GameApi.GetMapSeed();
        if (!seed.HasValue || seed.Value == 0)
        {
          yield return TimerApi.WaitSeconds(RetryDelaySeconds);
          continue;
        }

        ConditionSettings settings = Settings;
        if (!settings.Enabled || !settings.HasAnyCondition)
        {
          MelonLogger.Msg($"[SeedCheck] Auto-restart disabled or no conditions selected (enabled={settings.Enabled} " +
            $"conditions={settings.ConditionSummary}).");
          yield break;
        }

        SeedCriteriaResult criteria = SeedCriteriaResult.Evaluate();
        if (!criteria.Ready && attempt < MaxAttempts - 1)
        {
          yield return TimerApi.WaitSeconds(RetryDelaySeconds);
          continue;
        }

        MelonLogger.Msg($"[SeedCheck] Seed {seed.Value} vendors={criteria.VendorCount} microwaves={criteria.MicrowaveCount} " +
          $"soulHarvester={criteria.HasSoulHarvester} creditCardGreen={criteria.HasGreenCreditCard} " +
          $"epicMicrowave={criteria.HasEpicMicrowave} epicVendor={criteria.HasEpicVendor} " +
          $"mode={settings.MatchModeLabel} conditions={settings.ConditionSummary}");

        if (criteria.Matches(settings))
        {
          MelonLogger.Msg($"[SeedCheck] Seed {seed.Value} meets criteria. Keeping run.");
          yield break;
        }

        MelonLogger.Msg($"[SeedCheck] Seed {seed.Value} missing {criteria.GetMissingSummary(settings)}. Holding R to restart run.");
        yield return RestartRun(seed.Value);
        yield break;
      }
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

        bool hudReady = GameObject.Find("HUD") != null;
        bool stageReady = MapController.currentStage != null;
        if (hudReady && stageReady)
        {
          yield return TimerApi.WaitSeconds(PostLoadDelaySeconds);
          yield break;
        }

        yield return TimerApi.WaitSeconds(LoadPollIntervalSeconds);
        waited += LoadPollIntervalSeconds;
      }

      MelonLogger.Msg("[SeedCheck] Load wait timed out; continuing.");
      yield return TimerApi.WaitSeconds(PostLoadDelaySeconds);
    }

    private static IEnumerator RestartRun(int seed)
    {
      if (GameApi.IsUserMenuOpen())
      {
        MelonLogger.Msg("[SeedCheck] Menu open; skipping restart.");
        yield break;
      }

      if (GameApi.TryInvokeRestartRun())
      {
        yield return TimerApi.WaitSeconds(RestartRetryDelaySeconds);
        if (GameApi.HasSeedChanged(seed))
        {
          yield break;
        }
      }

      for (int attempt = 1; attempt <= RestartAttempts; attempt++)
      {
        if (GameApi.IsMainMenu())
        {
          yield break;
        }

        if (GameApi.IsUserMenuOpen())
        {
          MelonLogger.Msg("[SeedCheck] Menu open; skipping restart.");
          yield break;
        }

        MelonLogger.Msg($"[SeedCheck] Restart attempt {attempt}/{RestartAttempts} (holding R).");
        yield return InputApi.HoldKey("R", RestartHoldSeconds);
        yield return TimerApi.WaitSeconds(RestartRetryDelaySeconds);

        if (GameApi.HasSeedChanged(seed))
        {
          yield break;
        }
      }

      MelonLogger.Msg("[SeedCheck] Restart attempts exhausted; run continues.");
    }

    private static ConditionSettings Settings => _settings ??= ConditionSettings.Create();

    private static void DrawSettingsWindow()
    {
      if (!_settingsVisible)
      {
        if (GUI.Button(new Rect(20f, 20f, 200f, 32f), "Show Seed Settings", Theme.Button))
        {
          _settingsVisible = true;
        }

        return;
      }

      GuiTheme theme = Theme;
      Rect panel = _settingsWindowRect;
      DrawPanel(panel, theme);

      float x = panel.x + theme.Padding;
      float y = panel.y + theme.Padding;
      float width = panel.width - theme.Padding * 2f;

      GUI.Label(new Rect(x, y, width, theme.HeaderHeight), "MoaiVendorCheck Alpha", theme.Header);
      y += theme.HeaderHeight + theme.SectionSpacing;

      ConditionSettings settings = Settings;
      bool changed = false;

      GUI.Label(new Rect(x, y, width, theme.RowHeight), "Auto-restart conditions:", theme.Label);
      y += theme.RowHeight + theme.RowSpacing;

      bool enabled = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Enable auto-restart", settings.Enabled, theme);
      if (enabled != settings.Enabled)
      {
        settings.Enabled = enabled;
        changed = true;
      }
      y += theme.RowHeight + theme.RowSpacing;

      bool matchAll = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Require ALL selected conditions", settings.MatchAll, theme);
      if (matchAll != settings.MatchAll)
      {
        settings.MatchAll = matchAll;
        changed = true;
      }
      y += theme.RowHeight + theme.SectionSpacing;

      GUI.Label(new Rect(x, y, width, theme.RowHeight), "Conditions:", theme.Label);
      y += theme.RowHeight + theme.RowSpacing;

      bool epicVendor = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Any vendor has an Epic item",
        settings.RequireEpicVendor, theme);
      if (epicVendor != settings.RequireEpicVendor)
      {
        settings.RequireEpicVendor = epicVendor;
        changed = true;
      }
      y += theme.RowHeight + theme.RowSpacing;

      bool epicMicrowave = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Any Epic microwave available",
        settings.RequireEpicMicrowave, theme);
      if (epicMicrowave != settings.RequireEpicMicrowave)
      {
        settings.RequireEpicMicrowave = epicMicrowave;
        changed = true;
      }
      y += theme.RowHeight + theme.RowSpacing;

      bool soulHarvester = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Soul Harvester present",
        settings.RequireSoulHarvester, theme);
      if (soulHarvester != settings.RequireSoulHarvester)
      {
        settings.RequireSoulHarvester = soulHarvester;
        changed = true;
      }
      y += theme.RowHeight + theme.RowSpacing;

      bool greenCard = DrawToggle(new Rect(x, y, width, theme.RowHeight), "Credit Card (Green) present",
        settings.RequireGreenCreditCard, theme);
      if (greenCard != settings.RequireGreenCreditCard)
      {
        settings.RequireGreenCreditCard = greenCard;
        changed = true;
      }
      y += theme.RowHeight + theme.SectionSpacing;

      GUI.Label(new Rect(x, y, width, theme.RowHeight), $"Mode: {settings.MatchModeLabel} | Active: {settings.ConditionSummary}",
        theme.SmallLabel);
      y += theme.RowHeight + theme.RowSpacing;

      if (GUI.Button(new Rect(x, y, 80f, theme.RowHeight + 2f), "Hide", theme.Button))
      {
        _settingsVisible = false;
      }

      if (changed)
      {
        settings.Save();
      }
    }

    private static bool DrawToggle(Rect rect, string label, bool value, GuiTheme theme)
    {
      Rect boxRect = new Rect(rect.x, rect.y + 2f, theme.ToggleSize, theme.ToggleSize);
      GUI.Box(boxRect, GUIContent.none, theme.ToggleBox);
      if (value)
      {
        Rect fillRect = new Rect(boxRect.x + 3f, boxRect.y + 3f, boxRect.width - 6f, boxRect.height - 6f);
        GUI.Box(fillRect, GUIContent.none, theme.ToggleFill);
      }

      Rect labelRect = new Rect(rect.x + theme.ToggleSize + 6f, rect.y, rect.width - theme.ToggleSize - 6f, rect.height);
      GUI.Label(labelRect, label, theme.Label);

      if (GUI.Button(rect, GUIContent.none, theme.TransparentButton))
      {
        return !value;
      }

      return value;
    }

    private static void DrawPanel(Rect rect, GuiTheme theme)
    {
      GUI.Box(rect, GUIContent.none, theme.PanelBorder);
      Rect inner = new Rect(rect.x + theme.Border, rect.y + theme.Border,
        rect.width - theme.Border * 2f, rect.height - theme.Border * 2f);
      GUI.Box(inner, GUIContent.none, theme.PanelFill);
    }

    private static GuiTheme Theme => _theme ??= GuiTheme.Create();

    private sealed class GuiTheme
    {
      private const float DefaultPadding = 10f;
      private const float DefaultRowHeight = 22f;
      private const float DefaultRowSpacing = 4f;
      private const float DefaultSectionSpacing = 8f;
      private const float DefaultHeaderHeight = 22f;
      private const float DefaultBorder = 2f;
      private const float DefaultToggleSize = 16f;

      private static readonly Color PanelBorderColor = new Color(0.16f, 0.16f, 0.17f, 0.95f);
      private static readonly Color PanelFillColor = new Color(0.11f, 0.12f, 0.13f, 0.92f);
      private static readonly Color ButtonFillColor = new Color(0.22f, 0.24f, 0.26f, 1f);
      private static readonly Color ButtonHoverColor = new Color(0.27f, 0.29f, 0.31f, 1f);
      private static readonly Color ButtonActiveColor = new Color(0.18f, 0.2f, 0.22f, 1f);
      private static readonly Color ToggleBoxColor = new Color(0.24f, 0.24f, 0.25f, 1f);
      private static readonly Color ToggleFillColor = new Color(0.25f, 0.72f, 0.3f, 1f);

      private static readonly Color HeaderTextColor = new Color(0.93f, 0.84f, 0.52f, 1f);
      private static readonly Color TextColor = new Color(0.9f, 0.9f, 0.9f, 1f);
      private static readonly Color SmallTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

      public float Padding => DefaultPadding;
      public float RowHeight => DefaultRowHeight;
      public float RowSpacing => DefaultRowSpacing;
      public float SectionSpacing => DefaultSectionSpacing;
      public float HeaderHeight => DefaultHeaderHeight;
      public float Border => DefaultBorder;
      public float ToggleSize => DefaultToggleSize;

      public GUIStyle PanelBorder { get; }
      public GUIStyle PanelFill { get; }
      public GUIStyle Header { get; }
      public GUIStyle Label { get; }
      public GUIStyle SmallLabel { get; }
      public GUIStyle Button { get; }
      public GUIStyle ToggleBox { get; }
      public GUIStyle ToggleFill { get; }
      public GUIStyle TransparentButton { get; }

      private GuiTheme(GUIStyle panelBorder, GUIStyle panelFill, GUIStyle header, GUIStyle label,
        GUIStyle smallLabel, GUIStyle button, GUIStyle toggleBox, GUIStyle toggleFill, GUIStyle transparentButton)
      {
        PanelBorder = panelBorder;
        PanelFill = panelFill;
        Header = header;
        Label = label;
        SmallLabel = smallLabel;
        Button = button;
        ToggleBox = toggleBox;
        ToggleFill = toggleFill;
        TransparentButton = transparentButton;
      }

      public static GuiTheme Create()
      {
        Font? font = TryGetThemeFont();

        var panelBorder = new GUIStyle(GUI.skin.box)
        {
          normal = { background = CreateTexture(PanelBorderColor) },
          border = CreateRectOffset(0, 0, 0, 0),
          padding = CreateRectOffset(0, 0, 0, 0),
          margin = CreateRectOffset(0, 0, 0, 0)
        };

        var panelFill = new GUIStyle(GUI.skin.box)
        {
          normal = { background = CreateTexture(PanelFillColor) },
          border = CreateRectOffset(0, 0, 0, 0),
          padding = CreateRectOffset(0, 0, 0, 0),
          margin = CreateRectOffset(0, 0, 0, 0)
        };

        var header = new GUIStyle(GUI.skin.label)
        {
          alignment = TextAnchor.UpperLeft,
          fontSize = 14,
          font = font
        };
        header.normal.textColor = HeaderTextColor;

        var label = new GUIStyle(GUI.skin.label)
        {
          alignment = TextAnchor.MiddleLeft,
          fontSize = 12,
          font = font
        };
        label.normal.textColor = TextColor;

        var smallLabel = new GUIStyle(GUI.skin.label)
        {
          alignment = TextAnchor.MiddleLeft,
          fontSize = 11,
          font = font
        };
        smallLabel.normal.textColor = SmallTextColor;

        var button = new GUIStyle(GUI.skin.button)
        {
          fontSize = 12,
          font = font,
          alignment = TextAnchor.MiddleCenter
        };
        button.normal.background = CreateTexture(ButtonFillColor);
        button.hover.background = CreateTexture(ButtonHoverColor);
        button.active.background = CreateTexture(ButtonActiveColor);

        var toggleBox = new GUIStyle(GUI.skin.box)
        {
          normal = { background = CreateTexture(ToggleBoxColor) }
        };

        var toggleFill = new GUIStyle(GUI.skin.box)
        {
          normal = { background = CreateTexture(ToggleFillColor) }
        };

        var transparentButton = new GUIStyle(GUI.skin.button)
        {
          normal = { background = null, textColor = new Color(0f, 0f, 0f, 0f) },
          hover = { background = null, textColor = new Color(0f, 0f, 0f, 0f) },
          active = { background = null, textColor = new Color(0f, 0f, 0f, 0f) }
        };

        return new GuiTheme(panelBorder, panelFill, header, label, smallLabel, button, toggleBox, toggleFill, transparentButton);
      }

      private static Texture2D CreateTexture(Color color)
      {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.hideFlags = HideFlags.HideAndDontSave;
        return texture;
      }

      private static RectOffset CreateRectOffset(int left, int right, int top, int bottom)
      {
        var offset = new RectOffset();
        offset.left = left;
        offset.right = right;
        offset.top = top;
        offset.bottom = bottom;
        return offset;
      }

      private static Font? TryGetThemeFont()
      {
        try
        {
          Font[] fonts = Resources.FindObjectsOfTypeAll<Font>();
          if (fonts == null || fonts.Length == 0)
          {
            return null;
          }

          string[] hints = { "pixel", "menu", "retro", "megabonk", "bold" };
          foreach (Font font in fonts)
          {
            if (font == null || string.IsNullOrWhiteSpace(font.name))
            {
              continue;
            }

            string name = font.name.ToLowerInvariant();
            for (int i = 0; i < hints.Length; i++)
            {
              if (name.Contains(hints[i]))
              {
                return font;
              }
            }
          }

          return fonts[0];
        }
        catch (Exception)
        {
          return null;
        }
      }
    }

    private sealed class SeedCriteriaResult
    {
      private const string SoulHarvesterName = "Soul Harvester";
      private const string CreditCardName = "Credit Card";

      public bool HasSoulHarvester { get; private set; }
      public bool HasGreenCreditCard { get; private set; }
      public bool HasEpicMicrowave { get; private set; }
      public bool HasEpicVendor { get; private set; }
      public int VendorCount { get; private set; }
      public int MicrowaveCount { get; private set; }

      public bool Ready => VendorCount > 0 || MicrowaveCount > 0;

      public bool Matches(ConditionSettings settings)
      {
        if (!settings.Enabled || !settings.HasAnyCondition)
        {
          return true;
        }

        bool soulOk = settings.RequireSoulHarvester && HasSoulHarvester;
        bool cardOk = settings.RequireGreenCreditCard && HasGreenCreditCard;
        bool microwaveOk = settings.RequireEpicMicrowave && HasEpicMicrowave;
        bool vendorOk = settings.RequireEpicVendor && HasEpicVendor;

        if (settings.MatchAll)
        {
          return (!settings.RequireSoulHarvester || HasSoulHarvester)
            && (!settings.RequireGreenCreditCard || HasGreenCreditCard)
            && (!settings.RequireEpicMicrowave || HasEpicMicrowave)
            && (!settings.RequireEpicVendor || HasEpicVendor);
        }

        return soulOk || cardOk || microwaveOk || vendorOk;
      }

      public string GetMissingSummary(ConditionSettings settings)
      {
        var missing = new List<string>();
        if (settings.RequireSoulHarvester && !HasSoulHarvester)
        {
          missing.Add("Soul Harvester");
        }

        if (settings.RequireGreenCreditCard && !HasGreenCreditCard)
        {
          missing.Add("Credit Card (Green)");
        }

        if (settings.RequireEpicMicrowave && !HasEpicMicrowave)
        {
          missing.Add("Epic Microwave");
        }

        if (settings.RequireEpicVendor && !HasEpicVendor)
        {
          missing.Add("Epic Vendor Item");
        }

        return missing.Count == 0 ? "nothing" : string.Join(", ", missing);
      }

      public static SeedCriteriaResult Evaluate()
      {
        var result = new SeedCriteriaResult();

        Il2CppArrayBase<InteractableShadyGuy> vendors = UnityEngine.Object.FindObjectsOfType<InteractableShadyGuy>();
        foreach (var vendor in vendors)
        {
          if (vendor == null)
          {
            continue;
          }

          result.VendorCount++;

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

            string name = ((UnlockableBase)item).GetName();
            if (!result.HasSoulHarvester && NameEquals(name, SoulHarvesterName))
            {
              result.HasSoulHarvester = true;
            }

            if (!result.HasGreenCreditCard && NameEquals(name, CreditCardName) && item.rarity == EItemRarity.Rare)
            {
              result.HasGreenCreditCard = true;
            }

            if (!result.HasEpicVendor && item.rarity == EItemRarity.Epic)
            {
              result.HasEpicVendor = true;
            }

            if (result.HasSoulHarvester && result.HasGreenCreditCard && result.HasEpicVendor)
            {
              break;
            }
          }
        }

        Il2CppArrayBase<InteractableMicrowave> microwaves = UnityEngine.Object.FindObjectsOfType<InteractableMicrowave>();
        foreach (var microwave in microwaves)
        {
          if (microwave == null)
          {
            continue;
          }

          result.MicrowaveCount++;

          if (microwave.usesLeft > 0 && microwave.rarity == EItemRarity.Epic)
          {
            result.HasEpicMicrowave = true;
            break;
          }
        }

        return result;
      }

      private static bool NameEquals(string? name, string expected)
      {
        return !string.IsNullOrWhiteSpace(name) &&
          name.Trim().Equals(expected, StringComparison.OrdinalIgnoreCase);
      }
    }

    private sealed class ConditionSettings
    {
      private const string CategoryName = "MoaiVendorCheckAlpha";

      private readonly MelonPreferences_Category _category;
      private readonly MelonPreferences_Entry<bool> _enabled;
      private readonly MelonPreferences_Entry<bool> _matchAll;
      private readonly MelonPreferences_Entry<bool> _requireSoulHarvester;
      private readonly MelonPreferences_Entry<bool> _requireGreenCreditCard;
      private readonly MelonPreferences_Entry<bool> _requireEpicMicrowave;
      private readonly MelonPreferences_Entry<bool> _requireEpicVendor;

      private ConditionSettings(
        MelonPreferences_Category category,
        MelonPreferences_Entry<bool> enabled,
        MelonPreferences_Entry<bool> matchAll,
        MelonPreferences_Entry<bool> requireSoulHarvester,
        MelonPreferences_Entry<bool> requireGreenCreditCard,
        MelonPreferences_Entry<bool> requireEpicMicrowave,
        MelonPreferences_Entry<bool> requireEpicVendor)
      {
        _category = category;
        _enabled = enabled;
        _matchAll = matchAll;
        _requireSoulHarvester = requireSoulHarvester;
        _requireGreenCreditCard = requireGreenCreditCard;
        _requireEpicMicrowave = requireEpicMicrowave;
        _requireEpicVendor = requireEpicVendor;
      }

      public bool Enabled
      {
        get => _enabled.Value;
        set => _enabled.Value = value;
      }

      public bool MatchAll
      {
        get => _matchAll.Value;
        set => _matchAll.Value = value;
      }

      public bool RequireSoulHarvester
      {
        get => _requireSoulHarvester.Value;
        set => _requireSoulHarvester.Value = value;
      }

      public bool RequireGreenCreditCard
      {
        get => _requireGreenCreditCard.Value;
        set => _requireGreenCreditCard.Value = value;
      }

      public bool RequireEpicMicrowave
      {
        get => _requireEpicMicrowave.Value;
        set => _requireEpicMicrowave.Value = value;
      }

      public bool RequireEpicVendor
      {
        get => _requireEpicVendor.Value;
        set => _requireEpicVendor.Value = value;
      }

      public bool HasAnyCondition =>
        RequireSoulHarvester || RequireGreenCreditCard || RequireEpicMicrowave || RequireEpicVendor;

      public string MatchModeLabel => MatchAll ? "ALL" : "ANY";

      public string ConditionSummary
      {
        get
        {
          var items = new List<string>();
          if (RequireEpicVendor)
          {
            items.Add("Epic Vendor");
          }

          if (RequireEpicMicrowave)
          {
            items.Add("Epic Microwave");
          }

          if (RequireSoulHarvester)
          {
            items.Add("Soul Harvester");
          }

          if (RequireGreenCreditCard)
          {
            items.Add("Credit Card (Green)");
          }

          return items.Count == 0 ? "none" : string.Join(", ", items);
        }
      }

      public void Save()
      {
        MelonPreferences.Save();
      }

      public static ConditionSettings Create()
      {
        var category = MelonPreferences.CreateCategory(CategoryName, "MoaiVendorCheck Alpha");

        return new ConditionSettings(
          category,
          category.CreateEntry("Enabled", true),
          category.CreateEntry("MatchAll", false),
          category.CreateEntry("RequireSoulHarvester", false),
          category.CreateEntry("RequireGreenCreditCard", false),
          category.CreateEntry("RequireEpicMicrowave", true),
          category.CreateEntry("RequireEpicVendor", true));
      }
    }
#endif
  }

  public static class GameApi
  {
    private static readonly string[] TierMemberNames = { "tier", "Tier", "currentTier", "CurrentTier" };
    private static readonly string[] TierManagerHints = { "Instance", "Run", "Tier", "GameManager" };
#if !MELONLOADER_STUBS
    private static readonly string[] StageTierMemberHints =
      { "stage", "stagenumber", "stageindex", "stageidx", "level", "levelindex", "levelidx", "floor", "room", "area", "zone" };
    private static readonly string[] StageTierFallbackHints =
      { "tier", "tierindex", "tieridx", "currenttier" };
    private static readonly string[] StageTierExcludeHints =
      { "maptier", "maptierindex", "runtier", "runconfig" };
    private static readonly string[] MenuOpenHints =
      { "pause", "menu", "options", "settings" };
    private static bool _loggedTierCandidates;
    private static bool _loggedRestartMethod;
#endif

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
      catch (Exception)
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

#if !MELONLOADER_STUBS
    public readonly struct TierInfo
    {
      public TierInfo(int? stageTier, int? runTier, string? stageSource)
      {
        StageTier = stageTier;
        RunTier = runTier;
        StageSource = stageSource;
      }

      public int? StageTier { get; }
      public int? RunTier { get; }
      public string? StageSource { get; }
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

    public static bool IsUserMenuOpen()
    {
      if (IsMainMenu())
      {
        return true;
      }

      try
      {
        bool menuLikelyOpen = Time.timeScale <= 0.001f || Cursor.visible;
        if (!menuLikelyOpen)
        {
          return false;
        }

        GameObject[] objects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
        {
          if (obj == null || !obj.activeInHierarchy)
          {
            continue;
          }

          string name = obj.name;
          if (MatchesHint(NormalizeMemberName(name), MenuOpenHints))
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

    public static TierInfo GetTierInfo()
    {
      int? runTier = GetRunTier();
      string? stageSource = null;
      int? stageTier = TryGetStageTierFromStageData(out stageSource);

      if (!stageTier.HasValue)
      {
        stageTier = TryGetStageTierFromMapController(runTier, out stageSource);
      }

      if (!stageTier.HasValue)
      {
        stageTier = TryGetStageTierFromStageName(out stageSource);
      }

      if (!stageTier.HasValue)
      {
        stageTier = GetInstanceTier();
        if (stageTier.HasValue)
        {
          stageSource = "MonoBehaviourProbe";
        }
      }

      return new TierInfo(stageTier, runTier, stageSource);
    }

    public static void LogStageTierCandidatesOnce()
    {
      if (_loggedTierCandidates)
      {
        return;
      }

      _loggedTierCandidates = true;

      try
      {
        var stage = MapController.currentStage;
        if (stage == null)
        {
          MelonLogger.Msg("[SeedCheck] Stage data is null; cannot inspect tier fields.");
        }
        else
        {
          if (stage is UnityEngine.Object stageObject)
          {
            MelonLogger.Msg($"[SeedCheck] Stage name: {stageObject.name}");
          }

          LogNumericMembers(stage, stage.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, "Stage", 25);
        }
      }
      catch (Exception ex)
      {
        MelonLogger.Msg($"[SeedCheck] Stage dump failed: {ex.GetType().Name} {ex.Message}");
      }

      LogMapControllerTierCandidates();
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

        var type = stage.GetType();
        if (type == null)
        {
          return null;
        }

        if (TryGetTierCandidate(stage, type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          StageTierMemberHints, out int value, out string memberName, out string candidateSource))
        {
          string normalized = NormalizeMemberName(memberName);
          int tier = NormalizeTier(normalized, value);
          source = candidateSource;
          return tier;
        }

        if (TryGetTierCandidate(stage, type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          StageTierFallbackHints, out value, out memberName, out candidateSource))
        {
          string normalized = NormalizeMemberName(memberName);
          int tier = NormalizeTier(normalized, value);
          source = candidateSource;
          return tier;
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    private static int? TryGetStageTierFromMapController(int? runTier, out string? source)
    {
      source = null;

      try
      {
        var type = typeof(MapController);

        if (TryGetTierCandidate(null, type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
          StageTierMemberHints, out int value, out string memberName, out string candidateSource))
        {
          int tier = NormalizeTier(NormalizeMemberName(memberName), value);
          if (!IsAmbiguousTierCandidate(memberName, runTier, tier))
          {
            source = candidateSource;
            return tier;
          }
        }

        object? instance = TryGetMapControllerInstance();
        if (instance != null)
        {
          var instanceType = instance.GetType();
          if (TryGetTierCandidate(instance, instanceType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            StageTierMemberHints, out value, out memberName, out candidateSource))
          {
            int tier = NormalizeTier(NormalizeMemberName(memberName), value);
            if (!IsAmbiguousTierCandidate(memberName, runTier, tier))
            {
              source = candidateSource;
              return tier;
            }
          }
        }

        if (TryGetTierCandidate(null, type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
          StageTierFallbackHints, out value, out memberName, out candidateSource))
        {
          int tier = NormalizeTier(NormalizeMemberName(memberName), value);
          if (!IsAmbiguousTierCandidate(memberName, runTier, tier))
          {
            source = candidateSource;
            return tier;
          }
        }

        if (instance != null)
        {
          var instanceType = instance.GetType();
          if (TryGetTierCandidate(instance, instanceType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            StageTierFallbackHints, out value, out memberName, out candidateSource))
          {
            int tier = NormalizeTier(NormalizeMemberName(memberName), value);
            if (!IsAmbiguousTierCandidate(memberName, runTier, tier))
            {
              source = candidateSource;
              return tier;
            }
          }
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    private static int? TryGetStageTierFromStageName(out string? source)
    {
      source = null;

      try
      {
        var stage = MapController.currentStage;
        if (stage == null)
        {
          return null;
        }

        string? stageName = null;
        if (stage is UnityEngine.Object stageObject)
        {
          stageName = stageObject.name;
        }

        if (string.IsNullOrWhiteSpace(stageName))
        {
          stageName = stage.ToString();
        }

        if (TryExtractTierFromName(stageName, out int tier))
        {
          source = $"StageName:{stageName}";
          return tier;
        }
      }
      catch (Exception)
      {
      }

      return null;
    }

    private static bool IsStageTierMemberName(string name)
    {
      string normalized = NormalizeMemberName(name);
      if (MatchesHint(normalized, StageTierExcludeHints))
      {
        return false;
      }

      if (MatchesHint(normalized, StageTierMemberHints))
      {
        return true;
      }

      return MatchesHint(normalized, StageTierFallbackHints);
    }

    private static bool MatchesHint(string normalized, string[] hints)
    {
      for (int i = 0; i < hints.Length; i++)
      {
        if (normalized.Contains(hints[i], StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }

    private static bool IsAmbiguousTierCandidate(string memberName, int? runTier, int tier)
    {
      if (!runTier.HasValue || tier != runTier.Value)
      {
        return false;
      }

      string normalized = NormalizeMemberName(memberName);
      if (normalized.Contains("stage", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("level", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("floor", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("room", StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      return true;
    }

    private static string NormalizeMemberName(string name)
    {
      return name.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("<", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace(">", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("k__BackingField", string.Empty, StringComparison.OrdinalIgnoreCase)
        .ToLowerInvariant();
    }

    private static bool TryConvertToInt(Type type, object? value, out int result)
    {
      result = 0;
      if (value == null)
      {
        return false;
      }

      try
      {
        if (type.IsEnum)
        {
          result = Convert.ToInt32(value);
          return true;
        }

        if (type == typeof(int))
        {
          result = (int)value;
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

    private static bool TryGetTierCandidate(object? instance, Type type, BindingFlags flags, string[] hints,
      out int value, out string memberName, out string source)
    {
      value = 0;
      memberName = string.Empty;
      source = string.Empty;

      foreach (var property in type.GetProperties(flags))
      {
        if (!IsNumericType(property.PropertyType) || !MatchesHint(NormalizeMemberName(property.Name), hints))
        {
          continue;
        }

        if (TryGetMemberValue(instance, property, out value))
        {
          memberName = property.Name;
          source = $"{type.Name}.{property.Name}={value}";
          return true;
        }
      }

      foreach (var field in type.GetFields(flags))
      {
        if (!IsNumericType(field.FieldType) || !MatchesHint(NormalizeMemberName(field.Name), hints))
        {
          continue;
        }

        if (TryGetMemberValue(instance, field, out value))
        {
          memberName = field.Name;
          source = $"{type.Name}.{field.Name}={value}";
          return true;
        }
      }

      return false;
    }

    private static bool TryInvokeRestartMethod(Type type, object? instance, string[] methodNames, out string invoked)
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
          MelonLogger.Msg($"[SeedCheck] Restart method {type.Name}.{method.Name} failed: {ex.GetType().Name} {ex.Message}");
        }
      }

      return false;
    }

    private static void LogNumericMembers(object? instance, Type type, BindingFlags flags, string label, int limit)
    {
      MelonLogger.Msg($"[SeedCheck] {label} type: {type.FullName}");

      int logged = 0;
      foreach (var property in type.GetProperties(flags))
      {
        if (!IsNumericType(property.PropertyType))
        {
          continue;
        }

        if (TryGetMemberValue(instance, property, out int value))
        {
          MelonLogger.Msg($"[SeedCheck] {label} prop {property.Name}={value}");
          logged++;
        }

        if (logged >= limit)
        {
          MelonLogger.Msg($"[SeedCheck] {label} prop dump truncated.");
          return;
        }
      }

      foreach (var field in type.GetFields(flags))
      {
        if (!IsNumericType(field.FieldType))
        {
          continue;
        }

        if (TryGetMemberValue(instance, field, out int value))
        {
          MelonLogger.Msg($"[SeedCheck] {label} field {field.Name}={value}");
          logged++;
        }

        if (logged >= limit)
        {
          MelonLogger.Msg($"[SeedCheck] {label} field dump truncated.");
          return;
        }
      }

      if (logged == 0)
      {
        MelonLogger.Msg($"[SeedCheck] No numeric {label} fields found.");
      }
    }

    private static void LogMapControllerTierCandidates()
    {
      try
      {
        var type = typeof(MapController);
        LogNumericMembers(null, type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, "MapController static", 25);

        object? instance = TryGetMapControllerInstance();
        if (instance == null)
        {
          MelonLogger.Msg("[SeedCheck] MapController instance not found.");
          return;
        }

        LogNumericMembers(instance, instance.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
          "MapController instance", 25);
      }
      catch (Exception ex)
      {
        MelonLogger.Msg($"[SeedCheck] MapController dump failed: {ex.GetType().Name} {ex.Message}");
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

    private static bool TryExtractTierFromName(string? name, out int tier)
    {
      tier = 0;
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      string lower = name.ToLowerInvariant();
      if (TryExtractLastNumber(lower, out tier))
      {
        return true;
      }

      for (int candidate = 1; candidate <= 3; candidate++)
      {
        if (lower.Contains($"tier{candidate}", StringComparison.OrdinalIgnoreCase)
          || lower.Contains($"tier_{candidate}", StringComparison.OrdinalIgnoreCase)
          || lower.Contains($"tier {candidate}", StringComparison.OrdinalIgnoreCase))
        {
          tier = candidate;
          return true;
        }
      }

      return false;
    }

    private static bool TryExtractLastNumber(string name, out int value)
    {
      value = 0;
      int end = name.Length - 1;
      while (end >= 0 && !char.IsDigit(name[end]))
      {
        end--;
      }

      if (end < 0)
      {
        return false;
      }

      int start = end;
      while (start >= 0 && char.IsDigit(name[start]))
      {
        start--;
      }

      start++;
      if (start > end)
      {
        return false;
      }

      string number = name.Substring(start, end - start + 1);
      return int.TryParse(number, out value);
    }

    private static bool IsNumericType(Type type)
    {
      if (type.IsEnum)
      {
        return true;
      }

      return type == typeof(int)
        || type == typeof(uint)
        || type == typeof(short)
        || type == typeof(ushort)
        || type == typeof(long)
        || type == typeof(ulong)
        || type == typeof(byte)
        || type == typeof(sbyte);
    }

    private static bool TryGetMemberValue(object? instance, PropertyInfo property, out int value)
    {
      value = 0;
      try
      {
        return TryConvertToInt(property.PropertyType, property.GetValue(instance), out value);
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static bool TryGetMemberValue(object? instance, FieldInfo field, out int value)
    {
      value = 0;
      try
      {
        return TryConvertToInt(field.FieldType, field.GetValue(instance), out value);
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static int NormalizeTier(string normalizedMemberName, int value)
    {
      if (normalizedMemberName.Contains("index", StringComparison.OrdinalIgnoreCase))
      {
        return value + 1;
      }

      return value;
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

    public static bool HasSeedChanged(int previousSeed)
    {
      int? currentSeed = GetMapSeed();
      return currentSeed.HasValue && currentSeed.Value != 0 && currentSeed.Value != previousSeed;
    }

    public static bool TryInvokeRestartRun()
    {
      string[] methodNames = { "RestartRun", "Restart", "RestartGame", "RestartMap", "RestartLevel", "ResetRun" };

      if (TryInvokeRestartMethod(typeof(MapController), null, methodNames, out string invoked))
      {
        MelonLogger.Msg($"[SeedCheck] Invoked restart method {invoked}.");
        return true;
      }

      object? instance = TryGetMapControllerInstance();
      if (instance != null && TryInvokeRestartMethod(instance.GetType(), instance, methodNames, out invoked))
      {
        MelonLogger.Msg($"[SeedCheck] Invoked restart method {invoked}.");
        return true;
      }

      if (!_loggedRestartMethod)
      {
        MelonLogger.Msg("[SeedCheck] No restart method found on MapController; falling back to R input.");
        _loggedRestartMethod = true;
      }

      return false;
    }
#else
    public readonly struct TierInfo
    {
      public TierInfo(int? stageTier, int? runTier, string? stageSource)
      {
        StageTier = stageTier;
        RunTier = runTier;
        StageSource = stageSource;
      }

      public int? StageTier { get; }
      public int? RunTier { get; }
      public string? StageSource { get; }
    }

    public static bool IsMainMenu() => false;

    public static bool IsUserMenuOpen() => false;

    public static TierInfo GetTierInfo() => new TierInfo(null, null, null);

    public static int? GetRunTier() => null;

    public static int? GetMapSeed() => null;

    public static bool HasSeedChanged(int previousSeed) => false;

    public static bool TryInvokeRestartRun() => false;
#endif

    private static int? ReadTierValue(Type type, object instance)
    {
      foreach (string memberName in TierMemberNames)
      {
        var property = type.GetProperty(memberName);
        if (property != null && property.PropertyType == typeof(int))
        {
          var value = property.GetValue(instance);
          if (value is int intValue)
          {
            return intValue;
          }
        }

        var field = type.GetField(memberName);
        if (field != null && field.FieldType == typeof(int))
        {
          var value = field.GetValue(instance);
          if (value is int intValue)
          {
            return intValue;
          }
        }
      }

      return null;
    }
  }

#if !MELONLOADER_STUBS
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
        MelonLogger.Msg($"[SeedCheck] Unable to map key '{keyName}'.");
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
