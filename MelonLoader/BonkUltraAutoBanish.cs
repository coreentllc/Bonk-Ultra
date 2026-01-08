using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Interactables;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppAssets.Scripts.Managers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

[assembly: MelonInfo(typeof(BonkUltraAutoBanish.BonkUltraAutoBanishMod), "Bonk Ultra Auto Banish", "0.0.2", "Strei")]
[assembly: MelonGame(null, "Megabonk")]

namespace BonkUltraAutoBanish
{
  public sealed class BonkUltraAutoBanishMod : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private const string LogPrefix = "[BonkUltraAutoBanish]";
    private const float VendorScanIntervalSeconds = 0.5f;

    private const string ItemAnvil = "Anvil";
    private const string ItemSuckyMagnet = "Sucky Magnet";
    private const string ItemSpicyMeatball = "Spicy Meatball";
    private const string ItemPowerGloves = "Power Gloves";
    private const string ItemBigBonk = "Big Bonk";
    private const string ItemSoulHarvester = "Soul Harvester";
    private const string ItemSpikyShield = "Spiky Shield";
    private const string ItemScarf = "Scarf";
    private const string ItemSlurpGloves = "Slurp Gloves";
    private const string ItemTurboSkates = "Turbo Skates";
    private const string ItemMirror = "Mirror";
    private const string ItemCreditCardGreen = "Credit Card (Green)";

    private static readonly string[] SingletonHints =
      { "Instance", "instance", "_instance", "s_instance", "Singleton", "Current", "current" };

    private static MelonPreferences_Category? _prefs;
    private static MelonPreferences_Entry<bool>? _prefEnabled;
    private static MelonPreferences_Entry<bool>? _prefRespectSkip;
    private static MelonPreferences_Entry<bool>? _prefDebug;
    private static MelonPreferences_Entry<bool>? _prefLogItems;

    private static HarmonyLib.Harmony? _harmony;
    private static bool? _skipChestAnimationEnabled;
    private static bool _loggedSkipSettingMissing;

    private static bool _seenRun;
    private static int? _lastSeed;

    private static readonly Dictionary<string, int> _inventoryCounts =
      new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> _banishedItems =
      new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static float _lastVendorScanAt;
    private static VendorSnapshot _vendorSnapshot;

    private static MethodInfo? _banishItemMethod;
    private static bool _banishMethodsResolved;
    private static bool _banishMethodLogged;

    private static IntGetter? _getBanishes;
    private static IntGetter? _getBanishesUsed;
    private static bool _banishGetterResolved;
    private static bool _banishGetterLogged;

    private static Type[]? _assemblyTypes;

    public override void OnInitializeMelon()
    {
      _prefs = MelonPreferences.CreateCategory("BonkUltraAutoBanish", "Bonk Ultra Auto Banish");
      _prefEnabled = _prefs.CreateEntry("Enabled", true, "Enable auto-banish");
      _prefRespectSkip = _prefs.CreateEntry("RespectSkipChestAnimation", true, "Only run when skip chest animation is on");
      _prefDebug = _prefs.CreateEntry("DebugLog", false, "Log auto-banish decisions");
      _prefLogItems = _prefs.CreateEntry("LogItemEvents", true, "Log item gains and banishes");

      _harmony = new HarmonyLib.Harmony("Strei.BonkUltraAutoBanish");
      TryPatchOpenChest(_harmony);
      TryPatchGiveItem(_harmony);
      TryPatchSkipSetting(_harmony);

      MelonLogger.Msg($"{LogPrefix} Loaded.");
    }

    public override void OnUpdate()
    {
      UpdateRunState();
    }

    private static bool Enabled => _prefEnabled?.Value ?? true;
    private static bool RespectSkipSetting => _prefRespectSkip?.Value ?? true;
    private static bool DebugLog => _prefDebug?.Value ?? false;
    private static bool LogItemEvents => _prefLogItems?.Value ?? true;

    private static void UpdateRunState()
    {
      if (IsMainMenu())
      {
        if (_seenRun)
        {
          ResetRunState("main-menu");
        }
        _seenRun = false;
        return;
      }

      _seenRun = true;
      int? seed = TryGetMapSeed();
      if (seed.HasValue && seed.Value != 0)
      {
        if (_lastSeed.HasValue && _lastSeed.Value != seed.Value)
        {
          ResetRunState("seed-change");
        }
        _lastSeed = seed;
      }
    }

    private static void ResetRunState(string reason)
    {
      _inventoryCounts.Clear();
      _banishedItems.Clear();
      _vendorSnapshot = default;
      _lastVendorScanAt = 0f;
      _lastSeed = null;
      if (DebugLog)
      {
        MelonLogger.Msg($"{LogPrefix} Reset run state ({reason}).");
      }
    }

    private static void TryPatchOpenChest(HarmonyLib.Harmony harmony)
    {
      var matches = new HashSet<MethodInfo>();
      MethodInfo? direct = AccessTools.Method(typeof(InventoryUtility), "OpenChest");
      if (direct != null && AcceptsItemDataParam(direct))
      {
        matches.Add(direct);
      }

      foreach (MethodInfo method in FindMethods("OpenChest", AcceptsItemDataParam))
      {
        matches.Add(method);
      }

      if (!matches.Any())
      {
        MelonLogger.Msg($"{LogPrefix} OpenChest method not found; auto-banish disabled.");
        return;
      }

      var prefix = new HarmonyLib.HarmonyMethod(typeof(BonkUltraAutoBanishMod), nameof(OpenChestPrefix));
      foreach (MethodInfo method in matches)
      {
        harmony.Patch(method, prefix: prefix);
      }

      MelonLogger.Msg($"{LogPrefix} Patched {matches.Count} OpenChest method(s).");
    }

    private static void TryPatchGiveItem(HarmonyLib.Harmony harmony)
    {
      var matches = new HashSet<MethodInfo>();
      MethodInfo? direct = AccessTools.Method(typeof(InventoryUtility), "GiveItem");
      if (direct != null && AcceptsItemDataParam(direct))
      {
        matches.Add(direct);
      }

      foreach (MethodInfo method in FindMethods("GiveItem", AcceptsItemDataParam))
      {
        matches.Add(method);
      }

      if (matches.Count == 0)
      {
        MelonLogger.Msg($"{LogPrefix} GiveItem method not found; inventory tracking disabled.");
        return;
      }

      var postfix = new HarmonyLib.HarmonyMethod(typeof(BonkUltraAutoBanishMod), nameof(GiveItemPostfix));
      foreach (MethodInfo method in matches)
      {
        harmony.Patch(method, postfix: postfix);
      }
    }

    private static void TryPatchSkipSetting(HarmonyLib.Harmony harmony)
    {
      var matches = FindMethods("UpdateSkipChestAnimation", HasSingleIntParam);
      if (!matches.Any())
      {
        MelonLogger.Msg($"{LogPrefix} Skip chest animation setting hook not found.");
        return;
      }

      var postfix = new HarmonyLib.HarmonyMethod(typeof(BonkUltraAutoBanishMod), nameof(UpdateSkipChestAnimationPostfix));
      foreach (MethodInfo method in matches)
      {
        harmony.Patch(method, postfix: postfix);
      }
    }

    private static bool OpenChestPrefix(ItemData __0, MethodBase __originalMethod)
    {
      if (!ShouldProcess())
      {
        return true;
      }

      if (__0 == null)
      {
        return true;
      }

      if (!TryResolveBanishDecision(__0, out string canonical, out string reason))
      {
        return true;
      }

      if (!TryBanishItem(__0))
      {
        return true;
      }

      _banishedItems.Add(canonical);
      if (LogItemEvents || DebugLog)
      {
        int? remaining = GetRemainingBanishes();
        string remainingText = remaining.HasValue ? remaining.Value.ToString() : "unknown";
        MelonLogger.Msg($"{LogPrefix} Banished {canonical} ({reason}). Remaining={remainingText}.");
      }

      return false;
    }

    private static void GiveItemPostfix(object __0)
    {
      ItemData? itemData = __0 as ItemData;
      if (itemData == null)
      {
        return;
      }

      if (!TryGetItemName(itemData, out string name))
      {
        return;
      }

      if (!TryGetCanonicalName(name, out string canonical))
      {
        return;
      }

      int count = GetInventoryCount(canonical);
      _inventoryCounts[canonical] = count + 1;
      if (LogItemEvents)
      {
        MelonLogger.Msg($"{LogPrefix} Got {canonical} (count={count + 1}).");
      }
    }

    private static void UpdateSkipChestAnimationPostfix(int __0)
    {
      _skipChestAnimationEnabled = __0 != 0;
    }

    private static bool ShouldProcess()
    {
      if (!Enabled)
      {
        return false;
      }

      if (RespectSkipSetting)
      {
        if (_skipChestAnimationEnabled.HasValue)
        {
          if (!_skipChestAnimationEnabled.Value)
          {
            return false;
          }
        }
        else if (!_loggedSkipSettingMissing)
        {
          _loggedSkipSettingMissing = true;
          MelonLogger.Msg($"{LogPrefix} Skip chest animation setting not detected; running anyway.");
        }
      }

      return true;
    }

    private static bool TryResolveBanishDecision(ItemData item, out string canonical, out string reason)
    {
      canonical = string.Empty;
      reason = string.Empty;

      if (!TryGetItemName(item, out string name))
      {
        return false;
      }

      if (!TryGetCanonicalName(name, out canonical))
      {
        return false;
      }

      if (_banishedItems.Contains(canonical))
      {
        return false;
      }

      if (!ShouldBanishByRule(canonical, out reason))
      {
        return false;
      }

      int? remaining = GetRemainingBanishes();
      if (remaining.HasValue)
      {
        if (remaining.Value <= 0)
        {
          reason = "no banishes";
          return false;
        }

        int reserved = GetReservedBanishes(canonical);
        if (remaining.Value - 1 < reserved)
        {
          reason = "reserved for Anvil/Sucky Magnet";
          return false;
        }
      }

      return true;
    }

    private static bool ShouldBanishByRule(string canonical, out string reason)
    {
      reason = string.Empty;

      if (IsPrimaryInventoryRequired(canonical))
      {
        if (GetInventoryCount(canonical) >= 1)
        {
          reason = "duplicate";
          return true;
        }

        return false;
      }

      if (IsSoulHarvester(canonical))
      {
        if (GetInventoryCount(ItemSoulHarvester) >= 3)
        {
          reason = "count>=3";
          return true;
        }

        if (IsTier3BossRoom())
        {
          reason = "tier3-boss";
          return true;
        }

        return false;
      }

      if (IsSpikyShield(canonical))
      {
        if (!IsEpicMicrowaveAvailable())
        {
          reason = "no epic microwave";
          return true;
        }

        return false;
      }

      if (IsAdditionalItem(canonical))
      {
        if (HasGreenCreditCard())
        {
          return false;
        }

        if (IsGreenCreditCardAvailable())
        {
          return false;
        }

        reason = "no green card";
        return true;
      }

      return false;
    }

    private static bool IsPrimaryInventoryRequired(string canonical)
    {
      return NameEquals(canonical, ItemAnvil)
        || NameEquals(canonical, ItemSuckyMagnet)
        || NameEquals(canonical, ItemSpicyMeatball)
        || NameEquals(canonical, ItemPowerGloves)
        || NameEquals(canonical, ItemBigBonk);
    }

    private static bool IsSoulHarvester(string canonical)
    {
      return NameEquals(canonical, ItemSoulHarvester);
    }

    private static bool IsSpikyShield(string canonical)
    {
      return NameEquals(canonical, ItemSpikyShield);
    }

    private static bool IsAdditionalItem(string canonical)
    {
      return NameEquals(canonical, ItemScarf)
        || NameEquals(canonical, ItemSlurpGloves)
        || NameEquals(canonical, ItemTurboSkates)
        || NameEquals(canonical, ItemMirror);
    }

    private static bool HasGreenCreditCard()
    {
      return GetInventoryCount(ItemCreditCardGreen) > 0;
    }

    private static bool IsGreenCreditCardAvailable()
    {
      return GetVendorSnapshot().HasGreenCreditCard;
    }

    private static bool IsEpicMicrowaveAvailable()
    {
      return GetVendorSnapshot().HasEpicMicrowave;
    }

    private static int GetReservedBanishes(string current)
    {
      int reserved = 0;
      if (!NameEquals(current, ItemAnvil) && !_banishedItems.Contains(ItemAnvil))
      {
        reserved++;
      }

      if (!NameEquals(current, ItemSuckyMagnet) && !_banishedItems.Contains(ItemSuckyMagnet))
      {
        reserved++;
      }

      return reserved;
    }

    private static int GetInventoryCount(string canonical)
    {
      return _inventoryCounts.TryGetValue(canonical, out int count) ? count : 0;
    }

    private static bool TryGetCanonicalName(string name, out string canonical)
    {
      canonical = string.Empty;
      if (IsGreenCreditCardName(name))
      {
        canonical = ItemCreditCardGreen;
        return true;
      }

      if (NameContains(name, ItemAnvil))
      {
        canonical = ItemAnvil;
        return true;
      }

      if (NameContains(name, ItemSuckyMagnet))
      {
        canonical = ItemSuckyMagnet;
        return true;
      }

      if (NameContains(name, ItemSpicyMeatball))
      {
        canonical = ItemSpicyMeatball;
        return true;
      }

      if (NameContains(name, ItemPowerGloves))
      {
        canonical = ItemPowerGloves;
        return true;
      }

      if (NameContains(name, ItemBigBonk))
      {
        canonical = ItemBigBonk;
        return true;
      }

      if (NameContains(name, ItemSoulHarvester))
      {
        canonical = ItemSoulHarvester;
        return true;
      }

      if (NameContains(name, ItemSpikyShield))
      {
        canonical = ItemSpikyShield;
        return true;
      }

      if (NameContains(name, ItemScarf))
      {
        canonical = ItemScarf;
        return true;
      }

      if (NameContains(name, ItemSlurpGloves))
      {
        canonical = ItemSlurpGloves;
        return true;
      }

      if (NameContains(name, ItemTurboSkates))
      {
        canonical = ItemTurboSkates;
        return true;
      }

      if (NameContains(name, ItemMirror))
      {
        canonical = ItemMirror;
        return true;
      }

      return false;
    }

    private static bool IsGreenCreditCardName(string name)
    {
      return NameContains(name, "Credit Card") && NameContains(name, "Green");
    }

    private static bool NameContains(string? name, string expected)
    {
      return !string.IsNullOrWhiteSpace(name)
        && name.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool NameEquals(string? name, string expected)
    {
      return !string.IsNullOrWhiteSpace(name)
        && name.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetItemName(ItemData item, out string name)
    {
      name = string.Empty;
      try
      {
        name = item.GetName();
        return !string.IsNullOrWhiteSpace(name);
      }
      catch (Exception)
      {
        return false;
      }
    }

    private static VendorSnapshot GetVendorSnapshot()
    {
      float now = Time.realtimeSinceStartup;
      if (_vendorSnapshot.HasData && now - _lastVendorScanAt < VendorScanIntervalSeconds)
      {
        return _vendorSnapshot;
      }

      _vendorSnapshot = ScanVendors();
      _lastVendorScanAt = now;
      return _vendorSnapshot;
    }

    private static VendorSnapshot ScanVendors()
    {
      var snapshot = new VendorSnapshot
      {
        HasData = true
      };

      try
      {
        Il2CppArrayBase<InteractableShadyGuy> vendors = UnityEngine.Object.FindObjectsOfType<InteractableShadyGuy>();
        foreach (var vendor in vendors)
        {
          if (vendor == null || vendor.done || vendor.items == null)
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

            if (item.rarity == EItemRarity.Rare && TryGetItemName(item, out string name) && IsGreenCreditCardName(name))
            {
              snapshot.HasGreenCreditCard = true;
              break;
            }
          }

          if (snapshot.HasGreenCreditCard)
          {
            break;
          }
        }
      }
      catch (Exception)
      {
      }

      try
      {
        Il2CppArrayBase<InteractableMicrowave> microwaves = UnityEngine.Object.FindObjectsOfType<InteractableMicrowave>();
        foreach (var microwave in microwaves)
        {
          if (microwave == null || microwave.usesLeft <= 0)
          {
            continue;
          }

          if (microwave.rarity == EItemRarity.Epic)
          {
            snapshot.HasEpicMicrowave = true;
            break;
          }
        }
      }
      catch (Exception)
      {
      }

      return snapshot;
    }

    private static bool IsTier3BossRoom()
    {
      int? tier = null;
      try
      {
        var runConfig = MapController.runConfig;
        if (runConfig != null)
        {
          tier = runConfig.mapTierIndex + 1;
        }
      }
      catch (Exception)
      {
      }

      if (!tier.HasValue || tier.Value != 3)
      {
        return false;
      }

      string? stageName = null;
      try
      {
        var stage = MapController.currentStage;
        if (stage is UnityEngine.Object stageObj)
        {
          stageName = stageObj.name;
        }
        else
        {
          stageName = stage?.ToString();
        }
      }
      catch (Exception)
      {
      }

      if (string.IsNullOrWhiteSpace(stageName))
      {
        return false;
      }

      string lower = stageName.ToLowerInvariant();
      return lower.Contains("boss") || lower.Contains("final");
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

    private static int? TryGetMapSeed()
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

    private static bool TryBanishItem(ItemData item)
    {
      EnsureBanishMethods();
      MethodInfo? method = _banishItemMethod;
      object? argument = item;

      if (method == null)
      {
        if (!_banishMethodLogged)
        {
          _banishMethodLogged = true;
          MelonLogger.Msg($"{LogPrefix} Banish method not found.");
        }
        return false;
      }

      try
      {
        object? target = method.IsStatic ? null : TryGetSingletonInstance(method.DeclaringType);
        if (!method.IsStatic && target == null)
        {
          return false;
        }

        method.Invoke(target, new[] { argument });
        return true;
      }
      catch (Exception ex)
      {
        if (!_banishMethodLogged)
        {
          _banishMethodLogged = true;
          MelonLogger.Msg($"{LogPrefix} Banish invoke failed: {ex.GetType().Name} {ex.Message}");
        }
        return false;
      }
    }

    private static void EnsureBanishMethods()
    {
      if (_banishMethodsResolved)
      {
        return;
      }

      _banishMethodsResolved = true;

      MethodInfo? candidate = null;
      foreach (MethodInfo method in FindMethods("BanishItem", HasSingleParam))
      {
        ParameterInfo param = method.GetParameters()[0];
        if (!param.ParameterType.IsAssignableFrom(typeof(ItemData)))
        {
          continue;
        }

        if (method.IsStatic)
        {
          _banishItemMethod = method;
          return;
        }

        if (candidate == null)
        {
          candidate = method;
        }
      }

      if (candidate != null)
      {
        _banishItemMethod = candidate;
        return;
      }

      foreach (MethodInfo method in FindMethods("BanishUpgradable", HasSingleParam))
      {
        ParameterInfo param = method.GetParameters()[0];
        if (!param.ParameterType.IsAssignableFrom(typeof(ItemData)))
        {
          continue;
        }

        _banishItemMethod = method;
        return;
      }
    }

    private static int? GetRemainingBanishes()
    {
      EnsureBanishGetters();
      int? total = _getBanishes?.TryInvoke();
      int? used = _getBanishesUsed?.TryInvoke();
      if (!total.HasValue || !used.HasValue)
      {
        return null;
      }

      return Math.Max(0, total.Value - used.Value);
    }

    private static void EnsureBanishGetters()
    {
      if (_banishGetterResolved)
      {
        return;
      }

      _banishGetterResolved = true;
      _getBanishes = FindIntGetter("GetBanishes");
      _getBanishesUsed = FindIntGetter("GetBanishesUsed");

      if ((_getBanishes == null || _getBanishesUsed == null) && !_banishGetterLogged)
      {
        _banishGetterLogged = true;
        MelonLogger.Msg($"{LogPrefix} Banish counter getters not found; reservation checks disabled.");
      }
    }

    private static IntGetter? FindIntGetter(string name)
    {
      foreach (Type type in GetAssemblyTypes())
      {
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
          if (!string.Equals(method.Name, name, StringComparison.Ordinal))
          {
            continue;
          }

          if (method.GetParameters().Length != 0 || method.ReturnType != typeof(int))
          {
            continue;
          }

          if (method.IsStatic)
          {
            return new IntGetter(method, null);
          }

          Func<object?>? provider = TryGetSingletonProvider(type);
          if (provider != null)
          {
            return new IntGetter(method, provider);
          }
        }
      }

      return null;
    }

    private static bool AcceptsItemDataParam(MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length != 1)
      {
        return false;
      }

      return parameters[0].ParameterType.IsAssignableFrom(typeof(ItemData));
    }

    private static bool HasSingleParam(MethodInfo method)
    {
      return method.GetParameters().Length == 1;
    }

    private static bool HasSingleIntParam(MethodInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      return parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
    }

    private static IEnumerable<MethodInfo> FindMethods(string name, Func<MethodInfo, bool> predicate)
    {
      foreach (Type type in GetAssemblyTypes())
      {
        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
          if (!string.Equals(method.Name, name, StringComparison.Ordinal))
          {
            continue;
          }

          if (predicate(method))
          {
            yield return method;
          }
        }
      }
    }

    private static Type[] GetAssemblyTypes()
    {
      if (_assemblyTypes != null)
      {
        return _assemblyTypes;
      }

      Assembly asm = typeof(MapController).Assembly;
      try
      {
        _assemblyTypes = asm.GetTypes();
      }
      catch (ReflectionTypeLoadException ex)
      {
        _assemblyTypes = ex.Types.Where(type => type != null).ToArray()!;
      }

      return _assemblyTypes;
    }

    private static Func<object?>? TryGetSingletonProvider(Type type)
    {
      foreach (string name in SingletonHints)
      {
        PropertyInfo? property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null && type.IsAssignableFrom(property.PropertyType))
        {
          return () => property.GetValue(null);
        }
      }

      foreach (string name in SingletonHints)
      {
        FieldInfo? field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null && type.IsAssignableFrom(field.FieldType))
        {
          return () => field.GetValue(null);
        }
      }

      return null;
    }

    private static object? TryGetSingletonInstance(Type? type)
    {
      if (type == null)
      {
        return null;
      }

      Func<object?>? provider = TryGetSingletonProvider(type);
      return provider?.Invoke();
    }

    private struct VendorSnapshot
    {
      public bool HasGreenCreditCard;
      public bool HasEpicMicrowave;
      public bool HasData;
    }

    private sealed class IntGetter
    {
      private readonly MethodInfo _method;
      private readonly Func<object?>? _instanceProvider;

      public IntGetter(MethodInfo method, Func<object?>? instanceProvider)
      {
        _method = method;
        _instanceProvider = instanceProvider;
      }

      public int? TryInvoke()
      {
        try
        {
          object? target = _method.IsStatic ? null : _instanceProvider?.Invoke();
          if (!_method.IsStatic && target == null)
          {
            return null;
          }

          object? result = _method.Invoke(target, null);
          return result is int value ? value : null;
        }
        catch (Exception)
        {
          return null;
        }
      }
    }
#endif
  }
}
