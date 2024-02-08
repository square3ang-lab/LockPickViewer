using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace KeyViewer.Migration.V2;

public class V2Migrator : IMigrator
{
	public Dictionary<KeyCode, int> KeyCounts;

	public Dictionary<KeyCode, KeySetting> KeySettings;

	public KeyViewerSettings Settings;

	public KeyManager manager;

	public V2Migrator(KeyManager manager, V2MigratorArgument args)
		: this(manager, args.keyCountsPath, args.keySettingsPath, args.settingsPath)
	{
	}

	public V2Migrator(KeyManager manager, string keyCountsPath, string keySettingsPath, string settingsPath)
	{
		KeyCounts = (string.IsNullOrWhiteSpace(keyCountsPath) ? new Dictionary<KeyCode, int>() : JsonConvert.DeserializeObject<Dictionary<KeyCode, int>>(File.ReadAllText(keyCountsPath)));
		KeySettings = (string.IsNullOrWhiteSpace(keySettingsPath) ? new Dictionary<KeyCode, KeySetting>() : JsonConvert.DeserializeObject<Dictionary<KeyCode, KeySetting>>(File.ReadAllText(keySettingsPath)));
		if (string.IsNullOrWhiteSpace(settingsPath))
		{
			throw new InvalidOperationException("Settings Path Cannot Be Null!");
		}
		Settings = (KeyViewerSettings)new XmlSerializer(typeof(KeyViewerSettings)).Deserialize(File.Open(settingsPath, FileMode.Open));
		this.manager = manager;
	}

	public Settings Migrate()
	{
		List<Profile> list = new List<Profile>();
		foreach (KeyViewerProfile profile2 in Settings.Profiles)
		{
			Profile profile = new Profile();
			profile.Name = profile2.Name;
			profile.MakeBarSpecialKeys = false;
			profile.ViewerOnlyGameplay = profile2.ViewerOnlyGameplay;
			profile.KeyViewerSize = profile2.KeyViewerSize;
			profile.KeyViewerXPos = profile2.KeyViewerXPos;
			profile.KeyViewerYPos = profile2.KeyViewerYPos;
			profile.AnimateKeys = profile2.AnimateKeys;
			profile.ShowKeyPressTotal = profile2.ShowKeyPressTotal;
			profile.IgnoreSkippedKeys = Settings.IgnoreSkippedKeys;
			profile.KPSUpdateRateMs = Settings.UpdateRate;
			profile.ActiveKeys = profile2.ActiveKeys.Select(delegate(KeyCode code)
			{
				Key.Config result;
				switch (code)
				{
				case KeyCode.None:
					return new Key.Config(manager, SpecialKeyType.KPS);
				default:
					result = new Key.Config(manager, code);
					break;
				case KeyCode.Joystick1Button0:
					result = new Key.Config(manager, SpecialKeyType.Total);
					break;
				}
				return result;
			}).ToList();
			MigrateProfile(profile2, profile.ActiveKeys);
			list.Add(profile);
		}
		Settings settings = new Settings();
		Settings settings2 = settings;
		settings2.Language = Settings.Language switch
		{
			LanguageEnum.ENGLISH => LanguageType.English, 
			LanguageEnum.KOREAN => LanguageType.Korean, 
			LanguageEnum.SPANISH => LanguageType.Spanish, 
			LanguageEnum.POLISH => LanguageType.Polish, 
			LanguageEnum.FRENCH => LanguageType.French, 
			LanguageEnum.VIETNAMESE => LanguageType.Vietnamese, 
			LanguageEnum.CHINESE_SIMPLIFIED => LanguageType.SimplifiedChinese, 
			_ => LanguageType.English, 
		};
		settings.Profiles = list;
		settings.ProfileIndex = Settings.ProfileIndex;
		return settings;
	}

	private void MigrateProfile(KeyViewerProfile pf, List<Key.Config> keyConfs)
	{
		foreach (Key.Config keyConf in keyConfs)
		{
			if (KeyCounts.TryGetValue(keyConf.Code, out var value))
			{
				keyConf.Count = (uint)value;
			}
			Dictionary<KeyCode, KeySetting> keySettings = KeySettings;
			if (keySettings.TryGetValue(keyConf.SpecialType switch
			{
				SpecialKeyType.KPS => KeyCode.None, 
				SpecialKeyType.Total => KeyCode.Joystick1Button0, 
				_ => keyConf.Code, 
			}, out var value2))
			{
				PoSize ps = value2.ps;
				Point pos = ps.Pos;
				keyConf.OffsetX = pos.x;
				keyConf.OffsetY = pos.y;
				Point size = ps.Size;
				keyConf.Width = size.x;
				keyConf.Height = size.y;
				Point textPos = value2.TextPos;
				keyConf.TextOffsetX = textPos.x;
				keyConf.TextOffsetY = textPos.y;
				Point cTextPos = value2.CTextPos;
				keyConf.CountTextOffsetX = cTextPos.x;
				keyConf.CountTextOffsetY = cTextPos.y;
				keyConf.TextFontSize = value2.TextF;
				keyConf.CountTextFontSize = value2.CTextF;
			}
			keyConf.ChangeBgColorJudge = Settings.ColorAsJudge;
			keyConf.TooEarlyColor = Settings.TE;
			keyConf.VeryEarlyColor = Settings.VE;
			keyConf.EarlyPerfectColor = Settings.EP;
			keyConf.PerfectColor = Settings.P;
			keyConf.LatePerfectColor = Settings.LP;
			keyConf.VeryLateColor = Settings.VL;
			keyConf.TooLateColor = Settings.TL;
			keyConf.PressedBackgroundColor = pf.PressedBackgroundColor;
			keyConf.ReleasedBackgroundColor = pf.ReleasedBackgroundColor;
			keyConf.PressedOutlineColor = pf.PressedOutlineColor;
			keyConf.ReleasedOutlineColor = pf.ReleasedOutlineColor;
			keyConf.PressedTextColor = new VertexGradient(pf.PressedTextColor);
			keyConf.ReleasedTextColor = new VertexGradient(pf.ReleasedTextColor);
			keyConf.Ease = Settings.ease;
			keyConf.EaseDuration = Settings.ed;
			keyConf.ShrinkFactor = Settings.sf;
		}
	}
}
