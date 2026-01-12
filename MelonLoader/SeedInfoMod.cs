using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;
#if !MELONLOADER_STUBS
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Chests;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Interactables;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppTMPro;
using UnityEngine.UI;
#endif

[assembly: MelonInfo(typeof(SeedInfoMod.SeedInfoModClass), "Seed info for Megabonk", "0.0.2", "iamoverit")]
[assembly: MelonGame("Ved", "Megabonk")]

namespace SeedInfoMod
{
  public static class SeedInfoApi
  {
    private static string _latestMarkup = string.Empty;

    public static event Action<string>? Updated;

    public static string LatestMarkup => _latestMarkup;

    internal static void SetLatestMarkup(string markup)
    {
      _latestMarkup = markup ?? string.Empty;
      try
      {
        Updated?.Invoke(_latestMarkup);
      }
      catch
      {
      }
    }
  }

  public static class ConsoleManager
  {
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetConsoleWindow();

    public static void CreateConsole()
    {
      if (GetConsoleWindow() != IntPtr.Zero)
      {
        return;
      }

      AllocConsole();
      Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
      Console.SetIn(new StreamReader(Console.OpenStandardInput()));
      Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
      Console.Title = "Seed Info Terminal";
      Console.Clear();
    }

    public static void DestroyConsole()
    {
      if (GetConsoleWindow() == IntPtr.Zero)
      {
        return;
      }

      FreeConsole();
    }

    public static void PrintColoredToConsole(string text)
    {
      Console.Clear();
      const string Pattern = "([CLERFNO])\\[(.+?)\\]";
      MatchCollection matches = Regex.Matches(text, Pattern);
      int lastIndex = 0;
      foreach (Match match in matches)
      {
        Console.Write(text.Substring(lastIndex, match.Index - lastIndex));
        string code = match.Groups[1].Value;
        string value = match.Groups[2].Value;
        switch (code)
        {
          case "L":
          case "O":
            Console.ForegroundColor = ConsoleColor.Yellow;
            break;
          case "E":
            Console.ForegroundColor = ConsoleColor.Magenta;
            break;
          case "R":
          case "F":
            Console.ForegroundColor = ConsoleColor.Blue;
            break;
          case "C":
          case "N":
            Console.ForegroundColor = ConsoleColor.White;
            break;
          default:
            Console.ResetColor();
            break;
        }

        Console.Write(value ?? string.Empty);
        Console.ResetColor();
        lastIndex = match.Index + match.Length;
      }

      if (lastIndex < text.Length)
      {
        Console.Write(text.Substring(lastIndex));
      }

      Console.WriteLine();
    }
  }

#if !MELONLOADER_STUBS
  [HarmonyPatch]
  public static class InteractablePatch
  {
    private static IEnumerable<MethodBase> TargetMethods()
    {
      yield return AccessTools.Method(typeof(InteractableShrineCursed), "Interact");
      yield return AccessTools.Method(typeof(InteractableShrineMagnet), "Interact");
      yield return AccessTools.Method(typeof(InteractableShrineMoai), "Interact");
      yield return AccessTools.Method(typeof(InteractableShrineChallenge), "Interact");
      yield return AccessTools.Method(typeof(InteractableShrineGreed), "Interact");
      yield return AccessTools.Method(typeof(InteractableShadyGuy), "OnDestroy");
      yield return AccessTools.Method(typeof(InteractableMicrowave), "Explode");
      yield return AccessTools.Method(typeof(ChargeShrine), "Complete");
      yield return AccessTools.Method(typeof(InteractableChest), "Interact");
      yield return AccessTools.Method(typeof(OpenChest), "OnTriggerStay");
      yield return AccessTools.Method(typeof(OpenChest), "Awake");
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
      SeedInfoModClass.Instance?.InvokeLootableChanged();
    }
  }

  [HarmonyPatch(typeof(InventoryUtility), "GetRandomItemsShadyGuy")]
  public static class MoaiPatch
  {
    [HarmonyPostfix]
    public static void Postfix()
    {
      SeedInfoModClass.Instance?.InvokeLootableChanged();
    }
  }
#endif

  public sealed class SeedInfoModClass : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private bool _overlayVisible = false;
    private TextMeshProUGUI? _overlayText;
    private GameObject? _textObject;
    private string _text = string.Empty;

    public static SeedInfoModClass? Instance { get; private set; }

    public event Action? OnLootableChanged;

    public void InvokeLootableChanged()
    {
      OnLootableChanged?.Invoke();
    }

    public override void OnInitializeMelon()
    {
      Instance = this;
      OnLootableChanged += UpdateSeedInfo;
      MelonLogger.Msg("SeedInfo initialized");
      MelonLogger.Msg("SeedInfo tested only on game version 1.0.12 and melon version 0.7.1");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
      _overlayText = null;
      _textObject = null;
    }

    public override void OnUpdate()
    {
      if (_textObject == null && GameObject.Find("HUD") != null)
      {
        CreateSeedInfo();
      }

      if (Input.GetKeyDown(KeyCode.F11))
      {
        ToggleOverlay();
      }
    }

    private void CreateSeedInfo()
    {
      GameObject hud = GameObject.Find("HUD");
      if (hud == null)
      {
        return;
      }

      _textObject = new GameObject("SeedInfo");
      _textObject.transform.SetParent(hud.transform, false);

      _overlayText = _textObject.AddComponent<TextMeshProUGUI>();
      _overlayText.fontSize = 20f;
      _overlayText.color = Color.white;
      _overlayText.alignment = (TextAlignmentOptions)4097;
      _overlayText.enableWordWrapping = false;
      _overlayText.raycastTarget = false;

      RectTransform rectTransform = _overlayText.rectTransform;
      rectTransform.anchorMin = new Vector2(0f, 0.5f);
      rectTransform.anchorMax = new Vector2(0f, 0.5f);
      rectTransform.pivot = new Vector2(0f, 0.5f);
      rectTransform.anchoredPosition = new Vector2(30f, 0f);

      MelonLogger.Msg("SeedInfo created");
      _textObject.SetActive(_overlayVisible);
      UpdateSeedInfo();
      ConsoleManager.CreateConsole();
      Console.WriteLine("SeedInfoMod initialized.");
    }

    private void UpdateSeedInfo()
    {
      _text = string.Empty;
      if (_overlayText == null)
      {
        return;
      }

      Il2CppArrayBase<ChargeShrine> shrines = UnityEngine.Object.FindObjectsOfType<ChargeShrine>();
      Il2CppArrayBase<InteractableShrineMoai> moaiShrines = UnityEngine.Object.FindObjectsOfType<InteractableShrineMoai>();
      Il2CppArrayBase<InteractableShadyGuy> shadyGuys = UnityEngine.Object.FindObjectsOfType<InteractableShadyGuy>();
      Il2CppArrayBase<InteractableShrineCursed> cursedShrines =
        UnityEngine.Object.FindObjectsOfType<InteractableShrineCursed>();
      Il2CppArrayBase<InteractableMicrowave> microwaves = UnityEngine.Object.FindObjectsOfType<InteractableMicrowave>();
      Il2CppArrayBase<InteractableChest> interactableChests = UnityEngine.Object.FindObjectsOfType<InteractableChest>();
      Il2CppArrayBase<OpenChest> openChests = UnityEngine.Object.FindObjectsOfType<OpenChest>();
      Il2CppArrayBase<InteractableShrineMagnet> magnetShrines =
        UnityEngine.Object.FindObjectsOfType<InteractableShrineMagnet>();
      Il2CppArrayBase<InteractableShrineGreed> greedShrines = UnityEngine.Object.FindObjectsOfType<InteractableShrineGreed>();
      Il2CppArrayBase<InteractableShrineChallenge> challengeShrines =
        UnityEngine.Object.FindObjectsOfType<InteractableShrineChallenge>();

      int shrineCommon = shrines.Count(x => !x.completed && !x.isGolden);
      int shrineLegendary = shrines.Count(x => !x.completed && x.isGolden);
      int moaiCount = moaiShrines.Count(x => !x.done);
      int vendorCommon = shadyGuys.Count(x => !x.done && (int)x.rarity == 0);
      int vendorRare = shadyGuys.Count(x => !x.done && (int)x.rarity == 1);
      int vendorEpic = shadyGuys.Count(x => !x.done && (int)x.rarity == 2);
      int vendorLegendary = shadyGuys.Count(x => !x.done && (int)x.rarity == 3);
      int microwaveCommon = microwaves.Count(x => x.usesLeft > 0 && (int)x.rarity == 0);
      int microwaveRare = microwaves.Count(x => x.usesLeft > 0 && (int)x.rarity == 1);
      int microwaveEpic = microwaves.Count(x => x.usesLeft > 0 && (int)x.rarity == 2);
      int microwaveLegendary = microwaves.Count(x => x.usesLeft > 0 && (int)x.rarity == 3);
      int cursedCount = cursedShrines.Count(x => !x.done);
      int magnetCount = magnetShrines.Count(x => !x.done);
      int greedCount = greedShrines.Count(x => !x.done);
      int challengeCount = challengeShrines.Count(x => !x.done);
      int chestCommon = interactableChests.Count(x => (int)x.chestType == 0);
      int chestRare = interactableChests.Count(x => (int)x.chestType == 2);
      int chestCurse = interactableChests.Count(x => (int)x.chestType == 1);
      int chestOpened = openChests.Count(x => !x.pickedup);

      _text += $"shrines: C[{shrineCommon}] L[{shrineLegendary}] greed: {greedCount}\n" +
               $"moai: {moaiCount} cursed: {cursedCount}\n" +
               $"magnet: {magnetCount} challenge: {challengeCount}\n";

      _text += string.Format(
        "{0,-10} {1,-5} {2,-5} {3,-5} {4,-5}\n" +
        "{5,-10} {6,-5} {7,-5} {8,-5} {9,-5}\n" +
        "{10,-10} {11,-5} {12,-5} {13,-5} {14,-5}\n",
        "vendor:", $"C[{vendorCommon}]", $"R[{vendorRare}]", $"E[{vendorEpic}]", $"L[{vendorLegendary}]",
        "microwave:", $"C[{microwaveCommon}]", $"R[{microwaveRare}]", $"E[{microwaveEpic}]", $"L[{microwaveLegendary}]",
        "chest:", $"N[{chestCommon}]", $"F[{chestRare}]", $"C[{chestCurse}]", $"O[{chestOpened}]");

      foreach (InteractableShadyGuy shadyGuy in shadyGuys)
      {
        string vendorRaritySymbol = GetRaritySymbol(shadyGuy.rarity);
        _text += $"{vendorRaritySymbol}[{shadyGuy.GetName()}]\n";

        var enumerator = shadyGuy.items.GetEnumerator();
        while (enumerator.MoveNext())
        {
          ItemData itemData = enumerator.Current;
          string itemRaritySymbol = GetRaritySymbol(itemData.rarity);
          _text += $"{itemRaritySymbol}[ - {itemData.GetName()}]\n";
        }
      }

      SeedInfoApi.SetLatestMarkup(_text);
      ConsoleManager.PrintColoredToConsole(_text);
      _overlayText.text = _text;
    }

    private static string GetRaritySymbol(EItemRarity rarity)
    {
      return (int)rarity switch
      {
        0 => "C",
        1 => "R",
        2 => "E",
        3 => "L",
        _ => string.Empty,
      };
    }

    private void ToggleOverlay()
    {
      if (_overlayText == null || _textObject == null)
      {
        return;
      }

      _overlayVisible = !_overlayVisible;
      _textObject.SetActive(_overlayVisible);
      MelonLogger.Msg("Overlay " + (_overlayVisible ? "shown" : "hidden"));
    }

#endif
  }
}
