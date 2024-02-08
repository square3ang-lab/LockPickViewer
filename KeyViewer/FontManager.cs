using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace KeyViewer;

public static class FontManager
{
	private static TMP_FontAsset DefaultTMPFont;

	private static Font DefaultFont;

	private static bool initialized;

	private static FontData defaultFont;

	private static Dictionary<string, FontData> Fonts = new Dictionary<string, FontData>();

	public static bool Initialized => initialized;

	public static string[] OSFonts { get; private set; }

	public static string[] OSFontPaths { get; private set; }

	public static ReadOnlyCollection<FontData> FallbackFontDatas { get; private set; }

	public static ReadOnlyCollection<Font> FallbackFonts { get; private set; }

	public static ReadOnlyCollection<TMP_FontAsset> FallbackTMPFonts { get; private set; }

	public static FontData GetFont(string name)
	{
		FontData font;
		return (!TryGetFont(name, out font)) ? defaultFont : font;
	}

	public static bool TryGetFont(string name, out FontData font)
	{
		if (!initialized)
		{
			DefaultFont = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
			DefaultTMPFont = TMP_FontAsset.CreateFontAsset(DefaultFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
			FallbackFontDatas = RDString.AvailableLanguages.Select(RDString.GetFontDataForLanguage).ToList().AsReadOnly();
			FallbackFonts = FallbackFontDatas.Select((FontData f) => f.font).ToList().AsReadOnly();
			FallbackTMPFonts = FallbackFontDatas.Select((FontData f) => f.fontTMP).ToList().AsReadOnly();
			DefaultTMPFont.fallbackFontAssetTable = FallbackTMPFonts.ToList();
			defaultFont = RDString.fontData;
			defaultFont.lineSpacing = 1f;
			defaultFont.lineSpacingTMP = 1f;
			defaultFont.fontScale = 0.5f;
			defaultFont.font = DefaultFont;
			defaultFont.fontTMP = DefaultTMPFont;
			OSFonts = Font.GetOSInstalledFontNames();
			OSFontPaths = Font.GetPathsToOSFonts();
			Fonts = new Dictionary<string, FontData>();
			initialized = true;
		}
		if (string.IsNullOrEmpty(name))
		{
			font = defaultFont;
			return false;
		}
		if (name == "Default")
		{
			font = defaultFont;
			return true;
		}
		if (Fonts.TryGetValue(name, out var value))
		{
			font = value;
			return true;
		}
		if (File.Exists(name))
		{
			FontData fontData = defaultFont;
			Font font2 = new Font(name);
			TMP_FontAsset tMP_FontAsset = TMP_FontAsset.CreateFontAsset(font2);
			if ((bool)tMP_FontAsset)
			{
				tMP_FontAsset.fallbackFontAssetTable = FallbackTMPFonts.ToList();
			}
			fontData.font = font2;
			fontData.fontTMP = tMP_FontAsset ?? defaultFont.fontTMP;
			Fonts.Add(name, fontData);
			font = fontData;
			return true;
		}
		int num = Array.IndexOf(OSFonts, name);
		if (num != -1)
		{
			FontData fontData2 = defaultFont;
			Font font3 = Font.CreateDynamicFontFromOSFont(name, defaultFont.font.fontSize);
			TMP_FontAsset tMP_FontAsset2 = TMP_FontAsset.CreateFontAsset(new Font(OSFontPaths[num]));
			if ((bool)tMP_FontAsset2)
			{
				tMP_FontAsset2.fallbackFontAssetTable = FallbackTMPFonts.ToList();
			}
			fontData2.font = font3;
			fontData2.fontTMP = tMP_FontAsset2 ?? defaultFont.fontTMP;
			Fonts.Add(name, fontData2);
			font = fontData2;
			return true;
		}
		font = defaultFont;
		return false;
	}
}
