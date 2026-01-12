using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MelonLoader;
using SeedInfoMod;
using UnityEngine;
#if !MELONLOADER_STUBS
using Il2CppTMPro;
#endif

[assembly: MelonInfo(typeof(SeedInfoHud.SeedInfoHudMod), "Seed Info HUD", "0.0.1", "Strei")]
[assembly: MelonGame("Ved", "Megabonk")]

namespace SeedInfoHud
{
  public sealed class SeedInfoHudMod : MelonMod
  {
#if MELONLOADER_STUBS
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
    }
#else
    private static readonly Regex SeedInfoMarkupRegex = new Regex("([CLERFNO])\\[(.+?)\\]",
      RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly string[] ColumnLabels = { "vendor:", "microwave:", "chest:" };
    private static readonly char[] VendorColumnOrder = { 'C', 'R', 'E', 'L' };
    private static readonly char[] ChestColumnOrder = { 'N', 'F', 'C', 'O' };

    private const float FontSize = 18f;
    private static readonly Vector2 MenuOffset = new Vector2(30f, 420f);
    private static readonly Vector2 MenuSize = new Vector2(600f, 520f);

    private bool _visible = true;
    private TextMeshProUGUI? _hudText;
    private GameObject? _hudRoot;
    private bool _dirty = true;
    private string _latestMarkup = string.Empty;
    private string _latestRichText = string.Empty;

    public override void OnInitializeMelon()
    {
      SeedInfoApi.Updated += OnSeedInfoUpdated;
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
      _hudText = null;
      _hudRoot = null;
      _dirty = true;
      _latestMarkup = string.Empty;
      _latestRichText = string.Empty;
    }

    public override void OnUpdate()
    {
      if (_hudText == null)
      {
        TryCreateHud();
      }

      if (_hudText != null && _dirty)
      {
        UpdateHudText();
      }
    }

    private void OnSeedInfoUpdated(string markup)
    {
      _latestMarkup = markup ?? string.Empty;
      _dirty = true;
    }

    private void TryCreateHud()
    {
      GameObject hud = GameObject.Find("HUD");
      if (hud == null)
      {
        return;
      }

      _hudRoot = new GameObject("SeedInfoHud");
      _hudRoot.transform.SetParent(hud.transform, false);

      _hudText = _hudRoot.AddComponent<TextMeshProUGUI>();
      _hudText.fontSize = FontSize;
      _hudText.color = Color.white;
      _hudText.alignment = TextAlignmentOptions.BottomLeft;
      _hudText.enableWordWrapping = false;
      _hudText.raycastTarget = false;

      RectTransform rectTransform = _hudText.rectTransform;
      rectTransform.anchorMin = new Vector2(0f, 0f);
      rectTransform.anchorMax = new Vector2(0f, 0f);
      rectTransform.pivot = new Vector2(0f, 0f);
      rectTransform.anchoredPosition = MenuOffset;
      rectTransform.sizeDelta = MenuSize;

      _hudRoot.SetActive(_visible);

      _latestMarkup = SeedInfoApi.LatestMarkup;
      _dirty = true;
    }

    private void UpdateHudText()
    {
      _dirty = false;
      if (_hudText == null)
      {
        return;
      }

      string richText = ConvertMarkupToRichText(_latestMarkup);
      if (richText == _latestRichText)
      {
        return;
      }

      _latestRichText = richText;
      _hudText.text = _latestRichText;
    }

    private static string ConvertMarkupToRichText(string markup)
    {
      if (string.IsNullOrEmpty(markup))
      {
        return string.Empty;
      }

      var builder = new StringBuilder();
      string[] lines = markup.Split('\n');
      for (int i = 0; i < lines.Length; i++)
      {
        builder.Append(ProcessLine(lines[i]));
        if (i < lines.Length - 1)
        {
          builder.AppendLine();
        }
      }

      return builder.ToString();
    }

    private static string ProcessLine(string line)
    {
      if (string.IsNullOrEmpty(line))
      {
        return string.Empty;
      }

      foreach (string prefix in ColumnLabels)
      {
        if (line.StartsWith(prefix, StringComparison.Ordinal))
        {
          return FormatColumnLine(prefix, prefix == "chest:" ? ChestColumnOrder : VendorColumnOrder, line);
        }
      }

      return ColorizeInlineLine(line);
    }

    private const string ColumnSpacing = "  ";

    private static string FormatColumnLine(string label, char[] order, string line)
    {
      var values = new Dictionary<char, int>();
      foreach (Match match in SeedInfoMarkupRegex.Matches(line))
      {
        char code = match.Groups[1].Value[0];
        if (int.TryParse(match.Groups[2].Value, out int numeric))
        {
          values[code] = numeric;
        }
      }

      var formattedValues = new List<string>();
      foreach (char code in order)
      {
        values.TryGetValue(code, out int numeric);
        formattedValues.Add(ColorizeDigit(code, numeric));
      }

      string labelText = EscapeRichText(label).PadRight(10);
      return $"{labelText}{string.Join(ColumnSpacing, formattedValues)}";
    }

    private static string ColorizeInlineLine(string line)
    {
      var builder = new StringBuilder();
      int lastIndex = 0;
      foreach (Match match in SeedInfoMarkupRegex.Matches(line))
      {
        builder.Append(EscapeRichText(line.Substring(lastIndex, match.Index - lastIndex)));
        builder.Append(ColorizeValue(match));
        lastIndex = match.Index + match.Length;
      }

      if (lastIndex < line.Length)
      {
        builder.Append(EscapeRichText(line.Substring(lastIndex)));
      }

      return builder.ToString();
    }

    private static string ColorizeValue(Match match)
    {
      char code = match.Groups[1].Value[0];
      string value = EscapeRichText(match.Groups[2].Value);
      string hex = GetHexForCode(code);

      if (string.IsNullOrWhiteSpace(hex))
      {
        return value;
      }

      return $"<color=#{hex}>{value}</color>";
    }

    private static string ColorizeDigit(char code, int value)
    {
      string text = EscapeRichText(value.ToString());
      string hex = GetHexForCode(code);
      return string.IsNullOrWhiteSpace(hex) ? text : $"<color=#{hex}>{text}</color>";
    }

    private static string GetHexForCode(char code)
    {
      return code switch
      {
        'L' => "EDC933", // legendary
        'O' => "EDC933", // opened
        'E' => "FF4DFF", // epic
        'R' => "3DA5FF", // rare
        'F' => "3DA5FF", // fancy
        'C' => "FFFFFF", // common
        'N' => "FFFFFF", // normal
        _ => string.Empty,
      };
    }

    private static string EscapeRichText(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      return text
        .Replace("&", "&amp;")
        .Replace("<", "&lt;")
        .Replace(">", "&gt;");
    }

#endif
  }
}
