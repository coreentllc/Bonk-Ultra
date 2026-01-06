using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using Il2CppTMPro;
using UnityEngine.UI;
using Il2CppAssets.Scripts.Managers;
#endif

[assembly: MelonInfo(typeof(BonkAutoSelectPriority.BonkAutoSelectPriorityMod), "Bonk AutoSelect Priority", "0.1.0", "Strei")]
[assembly: MelonGame(null, "Megabonk")]

namespace BonkAutoSelectPriority
{
  public sealed class BonkAutoSelectPriorityMod : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private const string LogPrefix = "[BonkAutoSelect]";
    private const float LevelUpScanIntervalSeconds = 0.4f;
    private const float ToggleScanIntervalSeconds = 2.0f;
    private const float BonkUltraPrefScanIntervalSeconds = 2.0f;
    private const float DefaultPickDelaySeconds = 0.15f;
    private const float DefaultButtonWidth = 200f;
    private const float DefaultButtonHeight = 32f;
    private const float DefaultButtonSpacing = 8f;
    private const int DefaultMainMenuButtonX = 20;
    private const int DefaultMainMenuButtonY = 120;
    private const int LegacyMainMenuButtonY = 80;
    private const int DefaultPauseMenuButtonX = 20;
    private const int DefaultPauseMenuButtonY = 120;
    private const float DefaultSettingsWidth = 520f;
    private const float DefaultSettingsHeight = 640f;
    private const string BonkUltraPrefCategory = "BonkUltraAlpha";
    private const string BonkUltraPrefMainMenuButtonX = "MainMenuButtonX";
    private const string BonkUltraPrefMainMenuButtonY = "MainMenuButtonY";

    private static readonly Color LegendaryYellow = new Color(0.93f, 0.79f, 0.2f, 1f);
    private static readonly Color LegendaryText = new Color(0f, 0f, 0f, 1f);
    private static readonly Color PanelBackground = new Color(0.1f, 0.11f, 0.12f, 0.95f);

    private static readonly string[] LevelUpRootHints = { "levelup", "level_up", "level up", "upgrade", "card", "choose" };
    private static readonly string[] AutoSelectToggleHints =
      { "auto select levelup upgrades", "auto select level up", "auto select levelup", "auto select" };
    private static readonly HashSet<string> PreferredWeaponNames =
      new(StringComparer.OrdinalIgnoreCase) { "Katana", "Dexecutioner", "Axe" };
    private static readonly HashSet<string> PreferredTomeNames =
      new(StringComparer.OrdinalIgnoreCase) { "Luck", "Chaos", "Curse", "XP" };

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<bool>? _prefRespectGameSetting;
    private static MelonPreferences_Entry<bool>? _prefRarityFirst;
    private static MelonPreferences_Entry<bool>? _prefDebugProbe;
    private static MelonPreferences_Entry<float>? _prefPickDelaySeconds;
    private static MelonPreferences_Entry<string>? _prefStatPriority;
    private static MelonPreferences_Entry<string>? _prefWeaponPriority;
    private static MelonPreferences_Entry<string>? _prefTomePriorityPreXp10;
    private static MelonPreferences_Entry<string>? _prefTomePriorityPostXp10;
    private static MelonPreferences_Entry<int>? _prefXpTomeLevelOverride;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonX;
    private static MelonPreferences_Entry<int>? _prefMainMenuButtonY;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuX;
    private static MelonPreferences_Entry<int>? _prefSettingsMenuY;

    private static List<string> _statPriority = new List<string>();
    private static List<string> _weaponPriority = new List<string>();
    private static List<PriorityGroup> _tomePriorityPreXp10 = new List<PriorityGroup>();
    private static List<PriorityGroup> _tomePriorityPostXp10 = new List<PriorityGroup>();

    private static bool _settingsVisible;
    private static Rect _settingsRect = new Rect(20f, 120f, DefaultSettingsWidth, DefaultSettingsHeight);
    private static Vector2 _settingsScroll;
    private static GUIStyle? _buttonStyle;
    private static GUIStyle? _windowStyle;
    private static GUIStyle? _headerStyle;
    private static Texture2D? _legendaryTexture;
    private static Texture2D? _panelTexture;
    private static GameObject? _uiBlocker;

    private static GameObject? _levelUpRoot;
    private static bool _levelUpOpen;
    private static float _levelUpOpenedAt;
    private static float _lastLevelUpScanAt;
    private static string _lastOptionsSignature = string.Empty;
    private static bool _autoPickedThisMenu;
    private static float _lastAutoPickAt;

    private static Toggle? _autoSelectToggle;
    private static float _lastToggleScanAt;
    private static bool _loggedMissingToggle;

    private static HarmonyLib.Harmony? _harmony;
    private static MethodInfo? _autoSelectMethod;
    private static Type[]? _gameAssemblyTypes;
    private static bool _loggedHarmonyAutoSelectHandled;
    private static float _lastBonkUltraPrefScanAt;
    private static bool _bonkUltraPrefAvailable;
    private static int _bonkUltraMainMenuX;
    private static int _bonkUltraMainMenuY;

    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkAutoSelectPriority", "Bonk AutoSelect");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-select priority");
      _prefRespectGameSetting = _prefs.CreateEntry("RespectGameSetting", true, "Only run when game auto-select is on");
      _prefRarityFirst = _prefs.CreateEntry("RarityFirst", true, "Tie-breaker: rarity first");
      _prefDebugProbe = _prefs.CreateEntry("DebugProbe", false, "Log level-up option probe details");
      _prefPickDelaySeconds = _prefs.CreateEntry("PickDelaySeconds", DefaultPickDelaySeconds, "Delay before auto-pick");
      _prefStatPriority = _prefs.CreateEntry("StatPriority", DefaultStatPriorityCsv(), "Stat priority order");
      _prefWeaponPriority = _prefs.CreateEntry("WeaponPriority", DefaultWeaponPriorityCsv(), "Weapon priority order");
      _prefTomePriorityPreXp10 = _prefs.CreateEntry("TomePriorityPreXp10", DefaultTomePriorityPreXp10Csv(),
        "Tome priority before XP tome level 10");
      _prefTomePriorityPostXp10 = _prefs.CreateEntry("TomePriorityPostXp10", DefaultTomePriorityPostXp10Csv(),
        "Tome priority after XP tome level 10");
      _prefXpTomeLevelOverride = _prefs.CreateEntry("XpTomeLevelOverride", -1,
        "Override XP tome level (-1 auto)");
      _prefMainMenuButtonX = _prefs.CreateEntry("MainMenuButtonX", DefaultMainMenuButtonX, "Main menu button X");
      _prefMainMenuButtonY = _prefs.CreateEntry("MainMenuButtonY", DefaultMainMenuButtonY, "Main menu button Y");
      _prefSettingsMenuX = _prefs.CreateEntry("SettingsMenuX", 640, "Settings menu X");
      _prefSettingsMenuY = _prefs.CreateEntry("SettingsMenuY", 220, "Settings menu Y");

      ApplyMainMenuButtonDefaults();

      LoadPrioritiesFromPrefs();

      _harmony = new HarmonyLib.Harmony("Strei.BonkAutoSelectPriority");
      TryPatchAutoSelect(_harmony);

      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnGUI()
    {
      bool pauseOpen = IsPauseMenuOpen();
      bool mainMenuOpen = IsMainMenu();
      if (!pauseOpen && !mainMenuOpen)
      {
        _settingsVisible = false;
        SetUiBlockerActive(false);
        return;
      }

      EnsureGuiStyles();

      float buttonWidth = DefaultButtonWidth;
      float buttonHeight = DefaultButtonHeight;
      float buttonX = DefaultPauseMenuButtonX;
      float buttonY = DefaultPauseMenuButtonY;

      if (mainMenuOpen)
      {
        ApplyMainMenuButtonDefaults();
        buttonX = Mathf.Clamp(_prefMainMenuButtonX?.Value ?? DefaultMainMenuButtonX, 0, 10000);
        buttonY = Mathf.Clamp(_prefMainMenuButtonY?.Value ?? DefaultMainMenuButtonY, 0, 10000);
      }

      if (_buttonStyle != null && GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight),
        "AutoSelect Priority", _buttonStyle))
      {
        _settingsVisible = !_settingsVisible;
      }

      SetUiBlockerActive(_settingsVisible);

      if (_settingsVisible)
      {
        _settingsRect.x = Mathf.Clamp(_prefSettingsMenuX?.Value ?? 640, 0, 10000);
        _settingsRect.y = Mathf.Clamp(_prefSettingsMenuY?.Value ?? 220, 0, 10000);
        _settingsRect = ClampWindowToScreen(_settingsRect);
        Rect autoButtonRect = new Rect(buttonX, buttonY, buttonWidth, buttonHeight);
        _settingsRect = AvoidMenuOverlap(_settingsRect, autoButtonRect);
        if (TryGetBonkUltraButtonRect(out Rect bonkRect))
        {
          _settingsRect = AvoidMenuOverlap(_settingsRect, bonkRect);
        }
        DrawSettingsPanel(_settingsRect);
      }
    }

    public override void OnUpdate()
    {
      if (!SettingsEnabled)
      {
        return;
      }

      UpdateLevelUpState();
      if (!_levelUpOpen)
      {
        _autoPickedThisMenu = false;
        return;
      }

      TryAutoPick();
    }

    private static bool SettingsEnabled => _prefEnabled?.Value ?? true;
    private static bool RespectGameSetting => _prefRespectGameSetting?.Value ?? true;
    private static bool RarityFirst => _prefRarityFirst?.Value ?? true;
    private static bool DebugProbe => _prefDebugProbe?.Value ?? false;
    private static float PickDelaySeconds => Mathf.Max(0f, _prefPickDelaySeconds?.Value ?? DefaultPickDelaySeconds);

    private static int XpTomeLevelOverride => _prefXpTomeLevelOverride?.Value ?? -1;

    private static void UpdateLevelUpState()
    {
      float now = Time.realtimeSinceStartup;
      if (now - _lastLevelUpScanAt < LevelUpScanIntervalSeconds)
      {
        return;
      }

      _lastLevelUpScanAt = now;
      GameObject? root = FindLevelUpRoot();
      bool open = root != null;
      if (open != _levelUpOpen)
      {
        _levelUpOpen = open;
        _levelUpOpenedAt = open ? now : 0f;
        _autoPickedThisMenu = false;
        _lastOptionsSignature = string.Empty;
      }

      _levelUpRoot = root;
    }

    private static bool TryAutoPick()
    {
      if (_autoPickedThisMenu)
      {
        return false;
      }

      float now = Time.realtimeSinceStartup;
      if (now - _levelUpOpenedAt < PickDelaySeconds)
      {
        return false;
      }

      if (RespectGameSetting)
      {
        bool? enabled = TryGetGameAutoSelectEnabled();
        if (!enabled.HasValue)
        {
          if (!_loggedMissingToggle)
          {
            _loggedMissingToggle = true;
            MelonLogger.Msg($"{LogPrefix} Auto-select toggle not found yet; disable RespectGameSetting to force picks.");
          }
          return false;
        }

        if (!enabled.Value)
        {
          return false;
        }
      }

      if (_levelUpRoot == null)
      {
        return false;
      }

      List<OptionCandidate> options = CollectOptions(_levelUpRoot);
      if (options.Count == 0)
      {
        return false;
      }

      OptionCandidate? best = SelectBestOption(options, out string summary);
      if (best == null)
      {
        return false;
      }

      if (DebugProbe)
      {
        MelonLogger.Msg($"{LogPrefix} Auto-pick: {summary}");
      }

      try
      {
        best.Button.onClick.Invoke();
        _autoPickedThisMenu = true;
        _lastAutoPickAt = now;
        return true;
      }
      catch (Exception ex)
      {
        MelonLogger.Msg($"{LogPrefix} Auto-pick failed: {ex.GetType().Name} {ex.Message}");
      }
      return false;
    }

    private static List<OptionCandidate> CollectOptions(GameObject root)
    {
      var candidates = new List<OptionCandidate>();
      Button[] buttons = root.GetComponentsInChildren<Button>(true);
      for (int i = 0; i < buttons.Length; i++)
      {
        Button button = buttons[i];
        if (button == null)
        {
          continue;
        }

        OptionCandidate? candidate = BuildCandidate(button, i);
        if (candidate == null)
        {
          continue;
        }

        candidates.Add(candidate);
      }

      if (candidates.Count > 3)
      {
        candidates = candidates.OrderByDescending(c => c.CandidateScore).Take(3).ToList();
      }

      string signature = string.Join("|", candidates.Select(c => c.Signature));
      if (signature != _lastOptionsSignature)
      {
        _lastOptionsSignature = signature;
        if (DebugProbe)
        {
          LogCandidates(candidates);
        }
      }

      return candidates;
    }

    private static OptionCandidate? BuildCandidate(Button button, int index)
    {
      List<string> lines = ExtractTextLines(button.gameObject);
      if (lines.Count == 0)
      {
        return null;
      }

      var candidate = new OptionCandidate(button, index, lines);
      ParseCandidate(candidate);

      if (!candidate.IsUpgradeCandidate)
      {
        return null;
      }

      return candidate;
    }

    private static void ParseCandidate(OptionCandidate candidate)
    {
      string combined = string.Join(" ", candidate.TextLines).ToLowerInvariant();

      candidate.WeaponName = FindMatch(_weaponPriority, combined);
      candidate.TomeName = FindTomeName(combined);
      candidate.Stats = ExtractStats(candidate.TextLines);
      candidate.Rarity = DetectRarity(candidate.TextLines, candidate.Button.gameObject);
      candidate.IsTome = candidate.TomeName != null;
      candidate.IsWeapon = candidate.WeaponName != null;

      candidate.SpecialWeaponUpgrade = candidate.IsWeapon
        && candidate.Stats.Contains("Damage")
        && candidate.Stats.Any(stat => stat == "Crit Damage" || stat == "Crit Chance" || stat == "Projectile Count")
        && candidate.Stats.Count >= 2;

      int score = 0;
      if (candidate.IsWeapon || candidate.IsTome)
      {
        score += 5;
      }

      score += candidate.Stats.Count * 2;
      if (candidate.SpecialWeaponUpgrade)
      {
        score += 10;
      }

      if (candidate.Rarity != Rarity.Unknown)
      {
        score += 3;
      }

      candidate.CandidateScore = score;
    }

    private static void LogCandidates(List<OptionCandidate> candidates)
    {
      for (int i = 0; i < candidates.Count; i++)
      {
        OptionCandidate option = candidates[i];
        string weapon = option.WeaponName ?? "n/a";
        string tome = option.TomeName ?? "n/a";
        string stats = option.Stats.Count > 0 ? string.Join(", ", option.Stats) : "n/a";
        MelonLogger.Msg($"{LogPrefix} Option {i + 1}: weapon={weapon} tome={tome} rarity={option.Rarity} stats={stats}");
        if (DebugProbe)
        {
          MelonLogger.Msg($"{LogPrefix} Option {i + 1} text: {string.Join(" | ", option.TextLines)}");
        }
      }
    }

    private static OptionCandidate? SelectBestOption(List<OptionCandidate> options, out string summary)
    {
      summary = string.Empty;
      if (options.Count == 0)
      {
        return null;
      }

      options = options.Where(IsPreferredCandidate).ToList();
      if (options.Count == 0)
      {
        return null;
      }

      int xpLevel = ResolveXpTomeLevel();
      foreach (var option in options)
      {
        option.Score = ScoreOption(option, xpLevel);
      }

      options.Sort(CompareOptionScores);
      OptionCandidate best = options[0];
      summary = best.Score.Describe();
      return best;
    }

    private static int CompareOptionScores(OptionCandidate a, OptionCandidate b)
    {
      OptionScore left = a.Score;
      OptionScore right = b.Score;

      if (left.IsWeapon != right.IsWeapon)
      {
        return left.IsWeapon ? -1 : 1;
      }

      if (left.SpecialWeapon != right.SpecialWeapon)
      {
        return left.SpecialWeapon ? -1 : 1;
      }

      if (RarityFirst)
      {
        int rarityCmp = right.RarityRank.CompareTo(left.RarityRank);
        if (rarityCmp != 0)
        {
          return rarityCmp;
        }

        int priorityCmp = left.PriorityRank.CompareTo(right.PriorityRank);
        if (priorityCmp != 0)
        {
          return priorityCmp;
        }
      }
      else
      {
        int priorityCmp = left.PriorityRank.CompareTo(right.PriorityRank);
        if (priorityCmp != 0)
        {
          return priorityCmp;
        }

        int rarityCmp = right.RarityRank.CompareTo(left.RarityRank);
        if (rarityCmp != 0)
        {
          return rarityCmp;
        }
      }

      return left.Index.CompareTo(right.Index);
    }

    private static OptionScore ScoreOption(OptionCandidate option, int xpTomeLevel)
    {
      var score = new OptionScore(option.Index)
      {
        SpecialWeapon = option.SpecialWeaponUpgrade,
        RarityRank = GetRarityRank(option.Rarity),
        WeaponRank = GetPriorityIndex(_weaponPriority, option.WeaponName),
        StatRank = GetPriorityIndex(_statPriority, option.Stats),
        TomeRank = GetTomePriorityRank(option.TomeName, xpTomeLevel)
      };

      if (option.IsTome)
      {
        score.PriorityRank = score.TomeRank;
      }
      else if (option.IsWeapon)
      {
        score.PriorityRank = score.WeaponRank * 100 + score.StatRank;
      }
      else
      {
        score.PriorityRank = 10000 + score.StatRank;
      }

      score.WeaponName = option.WeaponName;
      score.TomeName = option.TomeName;
      score.Stats = option.Stats;
      score.Rarity = option.Rarity;
      return score;
    }

    private static int ResolveXpTomeLevel()
    {
      int overrideLevel = XpTomeLevelOverride;
      if (overrideLevel >= 0)
      {
        return overrideLevel;
      }

      return 0;
    }

    private static int GetTomePriorityRank(string? tomeName, int xpTomeLevel)
    {
      if (string.IsNullOrWhiteSpace(tomeName))
      {
        return 9999;
      }

      List<PriorityGroup> groups = xpTomeLevel >= 10 ? _tomePriorityPostXp10 : _tomePriorityPreXp10;
      for (int i = 0; i < groups.Count; i++)
      {
        if (groups[i].Matches(tomeName))
        {
          return i;
        }
      }

      return 9999;
    }

    private static int GetPriorityIndex(List<string> priority, string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return 9999;
      }

      for (int i = 0; i < priority.Count; i++)
      {
        if (string.Equals(priority[i], name, StringComparison.OrdinalIgnoreCase))
        {
          return i;
        }
      }

      return 9999;
    }

    private static int GetPriorityIndex(List<string> priority, List<string> names)
    {
      if (names.Count == 0)
      {
        return 9999;
      }

      int best = 9999;
      foreach (string name in names)
      {
        int index = GetPriorityIndex(priority, name);
        if (index < best)
        {
          best = index;
        }
      }

      return best;
    }

    private static List<string> ExtractStats(List<string> lines)
    {
      var result = new List<string>();
      foreach (string line in lines)
      {
        string text = line.ToLowerInvariant();
        AddStatIfMatch(result, text, "Damage", new[] { "damage", "dmg" });
        AddStatIfMatch(result, text, "Crit Damage", new[] { "crit damage", "critical damage", "crit dmg" });
        AddStatIfMatch(result, text, "Crit Chance", new[] { "crit chance", "critical chance", "crit %" });
        AddStatIfMatch(result, text, "Projectile Count", new[] { "projectile count", "projectiles", "extra projectile", "additional projectile" });
        AddStatIfMatch(result, text, "Projectile Speed", new[] { "projectile speed", "proj speed", "bullet speed" });
        AddStatIfMatch(result, text, "Size", new[] { "size", "radius", "area" });
        AddStatIfMatch(result, text, "Duration", new[] { "duration", "lifetime", "time" });
      }

      return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static void AddStatIfMatch(List<string> stats, string text, string canonical, string[] hints)
    {
      foreach (string hint in hints)
      {
        if (text.Contains(hint))
        {
          if (!stats.Contains(canonical))
          {
            stats.Add(canonical);
          }
          return;
        }
      }
    }

    private static Rarity DetectRarity(List<string> lines, GameObject root)
    {
      foreach (string line in lines)
      {
        string text = line.ToLowerInvariant();
        if (text.Contains("legendary"))
        {
          return Rarity.Legendary;
        }
        if (text.Contains("epic"))
        {
          return Rarity.Epic;
        }
        if (text.Contains("rare"))
        {
          return Rarity.Rare;
        }
        if (text.Contains("uncommon"))
        {
          return Rarity.Uncommon;
        }
        if (text.Contains("common"))
        {
          return Rarity.Common;
        }
      }

      Image? rarityImage = FindRarityImage(root);
      if (rarityImage != null)
      {
        return GuessRarityFromColor(rarityImage.color);
      }

      return Rarity.Unknown;
    }

    private static Image? FindRarityImage(GameObject root)
    {
      Image[] images = root.GetComponentsInChildren<Image>(true);
      Image? best = null;
      float bestAlpha = 0.1f;
      foreach (Image image in images)
      {
        if (image == null)
        {
          continue;
        }

        if (image.color.a < 0.3f)
        {
          continue;
        }

        if (image.color.a > bestAlpha)
        {
          best = image;
          bestAlpha = image.color.a;
        }
      }

      return best;
    }

    private static Rarity GuessRarityFromColor(Color color)
    {
      if (color.r > 0.85f && color.g > 0.7f && color.b < 0.4f)
      {
        return Rarity.Legendary;
      }
      if (color.r > 0.6f && color.b > 0.6f)
      {
        return Rarity.Epic;
      }
      if (color.b > 0.7f && color.g > 0.7f)
      {
        return Rarity.Rare;
      }
      if (color.g > 0.7f)
      {
        return Rarity.Uncommon;
      }

      return Rarity.Unknown;
    }

    private static int GetRarityRank(Rarity rarity)
    {
      switch (rarity)
      {
        case Rarity.Legendary:
          return 4;
        case Rarity.Epic:
          return 3;
        case Rarity.Rare:
          return 2;
        case Rarity.Uncommon:
          return 1;
        case Rarity.Common:
          return 0;
        default:
          return -1;
      }
    }

    private static string? FindMatch(List<string> list, string text)
    {
      foreach (string item in list)
      {
        if (text.Contains(item.ToLowerInvariant()))
        {
          return item;
        }
      }

      return null;
    }

    private static string? FindTomeName(string text)
    {
      if (text.Contains("tome"))
      {
        if (text.Contains("curse"))
        {
          return "Curse";
        }
        if (text.Contains("luck"))
        {
          return "Luck";
        }
        if (text.Contains("xp") || text.Contains("exp"))
        {
          return "XP";
        }
        if (text.Contains("difficulty"))
        {
          return "Difficulty";
        }
        if (text.Contains("chaos"))
        {
          return "Chaos";
        }

        return "Tome";
      }

      if (text.Contains("luck"))
      {
        return "Luck";
      }
      if (text.Contains("xp") || text.Contains("exp"))
      {
        return "XP";
      }
      if (text.Contains("difficulty"))
      {
        return "Difficulty";
      }
      if (text.Contains("chaos"))
      {
        return "Chaos";
      }
      if (text.Contains("curse"))
      {
        return "Curse";
      }

      return null;
    }

    private static List<string> ExtractTextLines(GameObject root)
    {
      var lines = new List<string>();
      TMP_Text[] tmpTexts = root.GetComponentsInChildren<TMP_Text>(true);
      foreach (TMP_Text tmp in tmpTexts)
      {
        if (tmp == null)
        {
          continue;
        }

        AddTextLines(lines, tmp.text);
      }

      Text[] uiTexts = root.GetComponentsInChildren<Text>(true);
      foreach (Text text in uiTexts)
      {
        if (text == null)
        {
          continue;
        }

        AddTextLines(lines, text.text);
      }

      return lines;
    }

    private static void AddTextLines(List<string> lines, string? text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        return;
      }

      string[] parts = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string part in parts)
      {
        string trimmed = part.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
        {
          lines.Add(trimmed);
        }
      }
    }

    private static GameObject? FindLevelUpRoot()
    {
      GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
      GameObject? best = null;
      int bestScore = 0;
      foreach (GameObject obj in objects)
      {
        if (obj == null || !obj.activeInHierarchy)
        {
          continue;
        }

        string name = obj.name ?? string.Empty;
        if (!ContainsHint(name, LevelUpRootHints))
        {
          continue;
        }

        Button[] buttons = obj.GetComponentsInChildren<Button>(true);
        int score = buttons.Length;
        if (score >= 3 && score > bestScore)
        {
          bestScore = score;
          best = obj;
        }
      }

      return best;
    }

    private static bool ContainsHint(string value, string[] hints)
    {
      string lower = value.ToLowerInvariant();
      foreach (string hint in hints)
      {
        if (lower.Contains(hint))
        {
          return true;
        }
      }

      return false;
    }

    private static bool IsMainMenu()
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

    private static bool IsPauseMenuOpen()
    {
      GameObject obj = GameObject.Find("PauseUI");
      if (obj != null)
      {
        return obj.activeInHierarchy;
      }

      obj = GameObject.Find("PauseMenu");
      return obj != null && obj.activeInHierarchy;
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

    private static Rect AvoidMenuOverlap(Rect rect, Rect avoid)
    {
      if (!rect.Overlaps(avoid))
      {
        return rect;
      }

      Rect candidate = rect;
      float spacing = DefaultButtonSpacing;

      if (TryMoveWindow(ref candidate, avoid.xMax + spacing, rect.y, avoid))
      {
        return candidate;
      }
      if (TryMoveWindow(ref candidate, avoid.xMin - rect.width - spacing, rect.y, avoid))
      {
        return candidate;
      }
      if (TryMoveWindow(ref candidate, rect.x, avoid.yMax + spacing, avoid))
      {
        return candidate;
      }
      if (TryMoveWindow(ref candidate, rect.x, avoid.yMin - rect.height - spacing, avoid))
      {
        return candidate;
      }

      return ClampWindowToScreen(rect);
    }

    private static bool TryMoveWindow(ref Rect rect, float x, float y, Rect avoid)
    {
      Rect candidate = new Rect(x, y, rect.width, rect.height);
      candidate = ClampWindowToScreen(candidate);
      if (!candidate.Overlaps(avoid))
      {
        rect = candidate;
        return true;
      }

      return false;
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

      var blocker = new GameObject("BonkAutoSelectUiBlocker");
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

    private static void DrawSettingsPanel(Rect rect)
    {
      if (_windowStyle != null)
      {
        GUI.Box(rect, "AutoSelect Priority", _windowStyle);
      }
      else
      {
        GUI.Box(rect, "AutoSelect Priority");
      }

      float padding = 12f;
      float headerHeight = 26f;
      Rect area = new Rect(rect.x + padding, rect.y + headerHeight, rect.width - padding * 2f,
        rect.height - headerHeight - padding);
      GUILayout.BeginArea(area);
      GUILayout.BeginVertical();
      _settingsScroll = GUILayout.BeginScrollView(_settingsScroll);

      GUILayout.Label("Behavior", _headerStyle);
      _prefEnabled!.Value = GUILayout.Toggle(SettingsEnabled, "Enabled");
      _prefRespectGameSetting!.Value = GUILayout.Toggle(RespectGameSetting, "Only when game Auto Select is on");
      _prefRarityFirst!.Value = GUILayout.Toggle(RarityFirst, "Tie-breaker: rarity first");
      _prefDebugProbe!.Value = GUILayout.Toggle(DebugProbe, "Log probe details");

      GUILayout.Space(8);
      GUILayout.Label("Stat Priority", _headerStyle);
      DrawReorderableList(_statPriority, UpdateStatPriority);

      GUILayout.Space(8);
      GUILayout.Label("Weapon Priority", _headerStyle);
      DrawReorderableList(_weaponPriority, UpdateWeaponPriority);

      GUILayout.Space(8);
      GUILayout.Label("Tome Priority (XP < 10)", _headerStyle);
      DrawReorderableGroups(_tomePriorityPreXp10, UpdateTomePriorityPreXp10);

      GUILayout.Space(8);
      GUILayout.Label("Tome Priority (XP >= 10)", _headerStyle);
      DrawReorderableGroups(_tomePriorityPostXp10, UpdateTomePriorityPostXp10);

      GUILayout.Space(8);
      GUILayout.Label("XP Tome Level Override", _headerStyle);
      int overrideValue = XpTomeLevelOverride;
      string overrideText = GUILayout.TextField(overrideValue.ToString());
      if (int.TryParse(overrideText, out int parsed))
      {
        _prefXpTomeLevelOverride!.Value = parsed;
      }

      GUILayout.Space(12);
      if (GUILayout.Button("Reset Defaults"))
      {
        ResetDefaults();
      }

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndArea();
    }

    private static void DrawReorderableList(List<string> list, Action<List<string>> onChanged)
    {
      for (int i = 0; i < list.Count; i++)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{i + 1}. {list[i]}", GUILayout.Width(320));
        if (GUILayout.Button("Up", GUILayout.Width(50)) && i > 0)
        {
          MoveItem(list, i, i - 1);
          onChanged(list);
        }
        if (GUILayout.Button("Down", GUILayout.Width(50)) && i < list.Count - 1)
        {
          MoveItem(list, i, i + 1);
          onChanged(list);
        }
        GUILayout.EndHorizontal();
      }
    }

    private static void DrawReorderableGroups(List<PriorityGroup> groups, Action<List<PriorityGroup>> onChanged)
    {
      for (int i = 0; i < groups.Count; i++)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{i + 1}. {groups[i].Display}", GUILayout.Width(320));
        if (GUILayout.Button("Up", GUILayout.Width(50)) && i > 0)
        {
          MoveItem(groups, i, i - 1);
          onChanged(groups);
        }
        if (GUILayout.Button("Down", GUILayout.Width(50)) && i < groups.Count - 1)
        {
          MoveItem(groups, i, i + 1);
          onChanged(groups);
        }
        GUILayout.EndHorizontal();
      }
    }

    private static void MoveItem<T>(List<T> list, int from, int to)
    {
      T item = list[from];
      list.RemoveAt(from);
      list.Insert(to, item);
    }

    private static void ResetDefaults()
    {
      _prefStatPriority!.Value = DefaultStatPriorityCsv();
      _prefWeaponPriority!.Value = DefaultWeaponPriorityCsv();
      _prefTomePriorityPreXp10!.Value = DefaultTomePriorityPreXp10Csv();
      _prefTomePriorityPostXp10!.Value = DefaultTomePriorityPostXp10Csv();
      LoadPrioritiesFromPrefs();
    }

    private static void LoadPrioritiesFromPrefs()
    {
      _statPriority = ParseSimpleCsv(_prefStatPriority?.Value ?? DefaultStatPriorityCsv());
      _weaponPriority = ParseSimpleCsv(_prefWeaponPriority?.Value ?? DefaultWeaponPriorityCsv());
      _tomePriorityPreXp10 = ParsePriorityGroups(_prefTomePriorityPreXp10?.Value ?? DefaultTomePriorityPreXp10Csv());
      _tomePriorityPostXp10 = ParsePriorityGroups(_prefTomePriorityPostXp10?.Value ?? DefaultTomePriorityPostXp10Csv());
    }

    private static bool IsPreferredCandidate(OptionCandidate candidate)
    {
      if (candidate.IsWeapon)
      {
        return IsPreferredWeaponName(candidate.WeaponName);
      }

      if (candidate.IsTome)
      {
        return IsPreferredTomeName(candidate.TomeName);
      }

      return false;
    }

    private static bool IsPreferredWeaponName(string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      return PreferredWeaponNames.Contains(name.Trim());
    }

    private static bool IsPreferredTomeName(string? name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return false;
      }

      return PreferredTomeNames.Contains(name.Trim());
    }

    private static void ApplyMainMenuButtonDefaults()
    {
      if (_prefMainMenuButtonX == null || _prefMainMenuButtonY == null)
      {
        return;
      }

      int currentX = _prefMainMenuButtonX.Value;
      int currentY = _prefMainMenuButtonY.Value;
      bool isDefault = currentX == DefaultMainMenuButtonX && currentY == DefaultMainMenuButtonY;
      bool isLegacy = currentX == DefaultMainMenuButtonX && currentY == LegacyMainMenuButtonY;

      if (!isDefault && !isLegacy)
      {
        return;
      }

      if (TryGetBonkUltraMainMenuButton(out int bonkX, out int bonkY))
      {
        int offset = Mathf.RoundToInt(DefaultButtonHeight + DefaultButtonSpacing);
        _prefMainMenuButtonX.Value = bonkX;
        _prefMainMenuButtonY.Value = bonkY + offset;
        return;
      }

      if (isLegacy)
      {
        _prefMainMenuButtonY.Value = DefaultMainMenuButtonY;
      }
    }

    private static bool TryGetBonkUltraButtonRect(out Rect rect)
    {
      rect = default;
      if (!TryGetBonkUltraMainMenuButton(out int x, out int y))
      {
        return false;
      }

      rect = new Rect(x, y, DefaultButtonWidth, DefaultButtonHeight);
      return true;
    }

    private static bool TryGetBonkUltraMainMenuButton(out int x, out int y)
    {
      x = _bonkUltraMainMenuX;
      y = _bonkUltraMainMenuY;

      if (_bonkUltraPrefAvailable)
      {
        return true;
      }

      float now = Time.realtimeSinceStartup;
      if (_lastBonkUltraPrefScanAt > 0f
        && now - _lastBonkUltraPrefScanAt < BonkUltraPrefScanIntervalSeconds)
      {
        return false;
      }

      _lastBonkUltraPrefScanAt = now;

      if (!TryReadBonkUltraPreference(BonkUltraPrefMainMenuButtonX, out int prefX)
        || !TryReadBonkUltraPreference(BonkUltraPrefMainMenuButtonY, out int prefY))
      {
        return false;
      }

      _bonkUltraMainMenuX = prefX;
      _bonkUltraMainMenuY = prefY;
      _bonkUltraPrefAvailable = true;
      x = prefX;
      y = prefY;
      return true;
    }

    private static bool TryReadBonkUltraPreference(string entryName, out int value)
    {
      value = 0;
      if (!TryGetPreferenceEntry(BonkUltraPrefCategory, entryName, out object? entry))
      {
        return false;
      }

      return entry != null && TryReadPreferenceInt(entry, out value);
    }

    // Reflection keeps this compatible with MelonLoader API differences.
    private static bool TryGetPreferenceEntry(string categoryName, string entryName, out object? entry)
    {
      entry = null;
      try
      {
        Type prefsType = typeof(MelonPreferences);
        MethodInfo? getEntry = prefsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
          .FirstOrDefault(method => method.Name == "GetEntry"
            && method.IsGenericMethodDefinition
            && method.GetParameters().Length == 2
            && method.GetParameters()[0].ParameterType == typeof(string)
            && method.GetParameters()[1].ParameterType == typeof(string));

        if (getEntry != null)
        {
          entry = getEntry.MakeGenericMethod(typeof(int))
            .Invoke(null, new object?[] { categoryName, entryName });
          return entry != null;
        }

        MethodInfo? getCategory = prefsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
          .FirstOrDefault(method => method.Name == "GetCategory"
            && method.GetParameters().Length >= 1
            && method.GetParameters()[0].ParameterType == typeof(string));
        if (getCategory == null)
        {
          return false;
        }

        object?[] categoryArgs;
        ParameterInfo[] categoryParams = getCategory.GetParameters();
        if (categoryParams.Length == 1)
        {
          categoryArgs = new object?[] { categoryName };
        }
        else if (categoryParams.Length == 2 && categoryParams[1].ParameterType == typeof(bool))
        {
          categoryArgs = new object?[] { categoryName, false };
        }
        else
        {
          return false;
        }

        object? category = getCategory.Invoke(null, categoryArgs);
        if (category == null)
        {
          return false;
        }

        MethodInfo? categoryGetEntry = category.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
          .FirstOrDefault(method => method.Name == "GetEntry"
            && method.IsGenericMethodDefinition
            && method.GetParameters().Length >= 1
            && method.GetParameters()[0].ParameterType == typeof(string));

        if (categoryGetEntry == null)
        {
          return false;
        }

        object?[] entryArgs;
        ParameterInfo[] entryParams = categoryGetEntry.GetParameters();
        if (entryParams.Length == 1)
        {
          entryArgs = new object?[] { entryName };
        }
        else if (entryParams.Length == 2 && entryParams[1].ParameterType == typeof(bool))
        {
          entryArgs = new object?[] { entryName, false };
        }
        else
        {
          return false;
        }

        entry = categoryGetEntry.MakeGenericMethod(typeof(int))
          .Invoke(category, entryArgs);
        return entry != null;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static bool TryReadPreferenceInt(object entry, out int value)
    {
      value = 0;
      try
      {
        PropertyInfo? valueProperty = entry.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        if (valueProperty == null)
        {
          return false;
        }

        object? raw = valueProperty.GetValue(entry, null);
        if (raw is int intValue)
        {
          value = intValue;
          return true;
        }

        if (raw != null && int.TryParse(raw.ToString(), out int parsed))
        {
          value = parsed;
          return true;
        }
      }
      catch (Exception)
      {
      }

      return false;
    }

    private static void UpdateStatPriority(List<string> list)
    {
      _statPriority = new List<string>(list);
      _prefStatPriority!.Value = string.Join(",", list);
    }

    private static void UpdateWeaponPriority(List<string> list)
    {
      _weaponPriority = new List<string>(list);
      _prefWeaponPriority!.Value = string.Join(",", list);
    }

    private static void UpdateTomePriorityPreXp10(List<PriorityGroup> list)
    {
      _tomePriorityPreXp10 = new List<PriorityGroup>(list);
      _prefTomePriorityPreXp10!.Value = string.Join(",", list.Select(g => g.Serialized));
    }

    private static void UpdateTomePriorityPostXp10(List<PriorityGroup> list)
    {
      _tomePriorityPostXp10 = new List<PriorityGroup>(list);
      _prefTomePriorityPostXp10!.Value = string.Join(",", list.Select(g => g.Serialized));
    }

    private static List<string> ParseSimpleCsv(string csv)
    {
      return csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(item => item.Trim())
        .Where(item => !string.IsNullOrWhiteSpace(item))
        .ToList();
    }

    private static List<PriorityGroup> ParsePriorityGroups(string csv)
    {
      var groups = new List<PriorityGroup>();
      string[] parts = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string part in parts)
      {
        string trimmed = part.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
          continue;
        }

        string[] names = trimmed.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
        var cleaned = names.Select(name => name.Trim()).Where(name => !string.IsNullOrWhiteSpace(name)).ToList();
        if (cleaned.Count > 0)
        {
          groups.Add(new PriorityGroup(cleaned));
        }
      }

      return groups;
    }

    private static string DefaultStatPriorityCsv()
    {
      return "Damage,Crit Damage,Crit Chance,Projectile Count,Size,Duration,Projectile Speed";
    }

    private static string DefaultWeaponPriorityCsv()
    {
      return "Katana,Dexecutioner,Axe,Default";
    }

    private static string DefaultTomePriorityPreXp10Csv()
    {
      return "Luck,XP,Difficulty=Chaos";
    }

    private static string DefaultTomePriorityPostXp10Csv()
    {
      return "Luck,XP=Difficulty,Chaos";
    }

    private static bool? TryGetGameAutoSelectEnabled()
    {
      if (_autoSelectToggle != null)
      {
        return _autoSelectToggle.isOn;
      }

      float now = Time.realtimeSinceStartup;
      if (now - _lastToggleScanAt < ToggleScanIntervalSeconds)
      {
        return null;
      }

      _lastToggleScanAt = now;
      _autoSelectToggle = FindAutoSelectToggle();
      if (_autoSelectToggle != null)
      {
        return _autoSelectToggle.isOn;
      }

      return null;
    }

    private static Toggle? FindAutoSelectToggle()
    {
      Toggle[] toggles = Resources.FindObjectsOfTypeAll<Toggle>();
      foreach (Toggle toggle in toggles)
      {
        if (toggle == null)
        {
          continue;
        }

        List<string> lines = ExtractTextLines(toggle.gameObject);
        if (lines.Count == 0)
        {
          continue;
        }

        string combined = string.Join(" ", lines).ToLowerInvariant();
        foreach (string hint in AutoSelectToggleHints)
        {
          if (combined.Contains(hint))
          {
            return toggle;
          }
        }
      }

      return null;
    }

    private static void TryPatchAutoSelect(HarmonyLib.Harmony harmony)
    {
      if (harmony == null)
      {
        return;
      }

      MethodInfo? autoMethod = FindAutoSelectMethod();
      if (autoMethod == null)
      {
        MelonLogger.Msg($"{LogPrefix} Harmony patch pending; will auto-pick via UI until method is identified.");
        return;
      }

      var prefix = new HarmonyLib.HarmonyMethod(typeof(BonkAutoSelectPriorityMod), nameof(AutoSelectPrefix));
      harmony.Patch(autoMethod, prefix: prefix);
      MelonLogger.Msg($"{LogPrefix} Harmony auto-select patch applied to {autoMethod.DeclaringType?.Name}.{autoMethod.Name}.");
    }

    private static bool AutoSelectPrefix(MethodBase __originalMethod)
    {
      bool handled = TryAutoPick();
      if (!handled)
      {
        return true;
      }

      if (!_loggedHarmonyAutoSelectHandled)
      {
        _loggedHarmonyAutoSelectHandled = true;
        MelonLogger.Msg($"{LogPrefix} Harmony auto-select override active on {__originalMethod.DeclaringType?.Name}.{__originalMethod.Name}.");
      }

      return false;
    }

    private static MethodInfo? FindAutoSelectMethod()
    {
      if (_autoSelectMethod != null)
      {
        return _autoSelectMethod;
      }

      MethodInfo? best = null;
      int bestScore = int.MinValue;
      foreach (Type type in GetGameAssemblyTypes())
      {
        if (type == null)
        {
          continue;
        }

        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (MethodInfo method in methods)
        {
          string nameLower = method.Name.ToLowerInvariant();
          if (!nameLower.Contains("autoselect") && !nameLower.Contains("autopick"))
          {
            continue;
          }

          if (method.ReturnType != typeof(void) && method.ReturnType != typeof(bool))
          {
            continue;
          }

          int score = ScoreAutoSelectMethod(method);
          if (score <= bestScore)
          {
            continue;
          }

          bestScore = score;
          best = method;
        }
      }

      _autoSelectMethod = best;
      return best;
    }

    private static int ScoreAutoSelectMethod(MethodInfo method)
    {
      int score = 0;
      string typeName = method.DeclaringType?.Name ?? string.Empty;
      if (typeName.IndexOf("Level", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 4;
      }
      if (typeName.IndexOf("Upgrade", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 4;
      }
      if (typeName.IndexOf("Option", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 2;
      }
      if (typeName.IndexOf("Panel", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 1;
      }

      string methodName = method.Name;
      if (methodName.IndexOf("Level", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 2;
      }
      if (methodName.IndexOf("Upgrade", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        score += 1;
      }

      if (method.GetParameters().Length == 0)
      {
        score += 1;
      }

      return score;
    }

    private static Type[] GetGameAssemblyTypes()
    {
      if (_gameAssemblyTypes != null)
      {
        return _gameAssemblyTypes;
      }

      Assembly asm = typeof(MapController).Assembly;
      try
      {
        _gameAssemblyTypes = asm.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        _gameAssemblyTypes = ex.Types.Where(type => type != null).ToArray()!;
      }

      return _gameAssemblyTypes;
    }

    private struct OptionScore
    {
      public OptionScore(int index)
      {
        Index = index;
        SpecialWeapon = false;
        RarityRank = -1;
        WeaponRank = 9999;
        StatRank = 9999;
        TomeRank = 9999;
        PriorityRank = 9999;
        WeaponName = null;
        TomeName = null;
        Stats = new List<string>();
        Rarity = Rarity.Unknown;
      }

      public int Index;
      public bool SpecialWeapon;
      public int RarityRank;
      public int WeaponRank;
      public int StatRank;
      public int TomeRank;
      public int PriorityRank;
      public string? WeaponName;
      public string? TomeName;
      public List<string> Stats;
      public Rarity Rarity;

      public bool IsWeapon => !string.IsNullOrWhiteSpace(WeaponName);
      public bool IsTome => !string.IsNullOrWhiteSpace(TomeName);

      public string Describe()
      {
        string stats = Stats.Count > 0 ? string.Join(", ", Stats) : "n/a";
        string weapon = WeaponName ?? "n/a";
        string tome = TomeName ?? "n/a";
        return $"weapon={weapon} tome={tome} rarity={Rarity} stats={stats} priority={PriorityRank}";
      }
    }

    private sealed class OptionCandidate
    {
      public OptionCandidate(Button button, int index, List<string> lines)
      {
        Button = button;
        Index = index;
        TextLines = lines;
        Stats = new List<string>();
        Rarity = Rarity.Unknown;
        IsUpgradeCandidate = true;
        Signature = string.Join("|", lines);
        Score = new OptionScore(index);
      }

      public Button Button { get; }
      public int Index { get; }
      public List<string> TextLines { get; }
      public List<string> Stats { get; set; }
      public string? WeaponName { get; set; }
      public string? TomeName { get; set; }
      public bool IsWeapon { get; set; }
      public bool IsTome { get; set; }
      public bool SpecialWeaponUpgrade { get; set; }
      public Rarity Rarity { get; set; }
      public bool IsUpgradeCandidate { get; set; }
      public int CandidateScore { get; set; }
      public string Signature { get; }
      public OptionScore Score { get; set; }
    }

    private sealed class PriorityGroup
    {
      public PriorityGroup(List<string> names)
      {
        Names = names;
        Serialized = string.Join("=", names);
        Display = Serialized;
      }

      public List<string> Names { get; }
      public string Serialized { get; }
      public string Display { get; }

      public bool Matches(string name)
      {
        return Names.Any(item => string.Equals(item, name, StringComparison.OrdinalIgnoreCase));
      }
    }

    private enum Rarity
    {
      Unknown = 0,
      Common = 1,
      Uncommon = 2,
      Rare = 3,
      Epic = 4,
      Legendary = 5
    }
#endif
  }
}
