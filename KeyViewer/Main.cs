using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using HarmonyLib;
using KeyViewer.Migration;
using KeyViewer.Migration.V2;
using KeyViewer.Patches;
using SFB;
using TMPro;
using UnityEngine;
using UnityModManagerNet;

namespace KeyViewer;

public static class Main
{
	public static V2MigratorArgument v2Arg = new V2MigratorArgument();

	public static GUIStyle bold;

	public static GUIStyle Bigbold;

	public static string MigrateErrorString = string.Empty;

	public static bool IsMigrating = false;

	public static readonly KeyCode[] KeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

	public static readonly Array languages = Enum.GetValues(typeof(LanguageType));

	public static readonly ISet<KeyCode> SKIPPED_KEYS = new HashSet<KeyCode>
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4,
		KeyCode.Mouse5,
		KeyCode.Mouse6,
		KeyCode.Escape
	};

	private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

	public static readonly MethodInfo GCVAN = typeof(Enum).GetMethod("GetCachedValuesAndNames", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty);

	public static readonly AccessTools.FieldRef<object, string[]> VAN_Names = AccessTools.FieldRefAccess<string[]>(typeof(Enum).GetNestedType("ValuesAndNames", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty), "Names");

	public static readonly AccessTools.FieldRef<object, ulong[]> VAN_Values = AccessTools.FieldRefAccess<ulong[]>(typeof(Enum).GetNestedType("ValuesAndNames", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty), "Values");

	public static bool IsEnabled { get; private set; }

	public static UnityModManager.ModEntry Mod { get; private set; }

	public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

	public static Settings Settings { get; private set; }

	public static Harmony Harmony { get; private set; }

	public static Sprite KeyOutline { get; private set; }

	public static Sprite KeyBackground { get; private set; }

	public static KeyManager KeyManager { get; private set; }

	public static LangManager Lang { get; private set; }

	public static bool IsListening { get; private set; }

	public static void Load(UnityModManager.ModEntry modEntry)
	{
		Mod = modEntry;
		Logger = modEntry.Logger;
		modEntry.OnToggle = OnToggle;
		modEntry.OnGUI = OnGUI;
		modEntry.OnSaveGUI = OnSaveGUI;
		modEntry.OnUpdate = OnUpdate;
		modEntry.OnShowGUI = OnShowGUI;
		modEntry.OnHideGUI = OnHideGUI;
		AssetBundle assetBundle = AssetBundle.LoadFromFile("Mods/LockPickViewer/keyviewer.assets");
		KeyOutline = assetBundle.LoadAsset<Sprite>("Assets/KeyOutline.png");
		KeyBackground = assetBundle.LoadAsset<Sprite>("Assets/KeyBackground.png");
	}

	public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
	{
		if (value)
		{
			Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			List<Profile> profiles = Settings.Profiles;
			int profileIndex = Settings.ProfileIndex;
			if (!profiles.Any())
			{
				profiles.Add(new Profile());
			}
			if (profiles.Count <= profileIndex || profileIndex < 0)
			{
				Settings.ProfileIndex = 0;
			}
			Lang = new LangManager(File.ReadAllText("Mods/LockPickViewer/Language.json"));
			Lang.ChangeLanguage(Settings.Language);
			Harmony = new Harmony(modEntry.Info.Id);
			Harmony.PatchAll(Assembly.GetExecutingAssembly());
			KeyManager = new GameObject("LockPickViewer KeyManager").AddComponent<KeyManager>();
			profiles.ForEach(delegate(Profile p)
			{
				p.Init(KeyManager);
			});
			KeyManager.Init(Settings.CurrentProfile);
			if (Settings.CurrentProfile.ResetWhenStart)
			{
				KeyManager.ClearCounts();
			}
			JudgementColorPatch.Init();
			IsEnabled = true;
		}
		else
		{
			IsEnabled = false;
			OnSaveGUI(modEntry);
			KPSCalculator.Stop();
			KeyManager?.Dispose();
			KeyManager = null;
			Harmony.UnpatchAll(Harmony.Id);
			Harmony = null;
		}
		return true;
	}

	public static void OnGUI(UnityModManager.ModEntry modEntry)
	{
		GUILayout.Label("LockPickViewer by square3ang");
		if (IsMigrating)
		{
			DrawMigrateMenu();
			return;
		}
		GUILayout.BeginHorizontal();
		foreach (LanguageType language in languages)
		{
			if (GUILayout.Button(Lang.GetLanguageName(language)))
			{
				Lang.ChangeLanguage(language);
				Settings.Language = language;
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Settings Backup Interval (s): ");
		int.TryParse(GUILayout.TextField(Settings.BackupInterval.ToString()), out Settings.BackupInterval);
		GUILayout.Label("s");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		DrawProfileSettingsGUI();
		GUILayout.Space(12f);
		MoreGUILayout.HorizontalLine(1f, 400f);
		GUILayout.BeginHorizontal();
		Settings.CurrentProfile.ResetWhenStart = GUILayout.Toggle(Settings.CurrentProfile.ResetWhenStart, Lang.GetString("RESET_WHEN_START"));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(8f);
		DrawGroupSettingsGUI();
		GUILayout.Space(8f);
		DrawKeyRegisterSettingsGUI();
		GUILayout.Space(8f);
		DrawKeyViewerSettingsGUI();
	}

	public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
	{
		Settings.Save(modEntry);
	}

	public static void OnShowGUI(UnityModManager.ModEntry modEntry)
	{
		BackupManager.Start();
	}

	public static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime)
	{
		bool flag = true;
		if ((bool)(UnityEngine.Object)(object)scrController.instance && (bool)(UnityEngine.Object)(object)scrConductor.instance)
		{
			KeyManager.isPlaying = !scrController.instance.paused && scrConductor.instance.isGameWorld;
		}
		if (KeyManager.Profile.ViewerOnlyGameplay)
		{
			flag = KeyManager.isPlaying;
		}
		if (flag != KeyManager.gameObject.activeSelf)
		{
			KeyManager.gameObject.SetActive(flag);
		}
		if (!IsListening)
		{
			return;
		}
		bool flag2 = false;
		KeyCode[] keyCodes = KeyCodes;
		foreach (KeyCode keyCode in keyCodes)
		{
			KeyCode code = keyCode;
			if (Input.GetKeyDown(code) && (!SKIPPED_KEYS.Contains(code) || KeyManager.Profile.IgnoreSkippedKeys))
			{
				int index;
				if ((index = KeyManager.Profile.ActiveKeys.FindIndex((Key.Config c) => c.Code == code)) != -1)
				{
					KeyManager.Profile.ActiveKeys.RemoveAt(index);
					flag2 = true;
				}
				else
				{
					KeyManager.Profile.ActiveKeys.Add(new Key.Config(KeyManager, code));
					flag2 = true;
				}
			}
		}
		if (flag2)
		{
			KeyManager.UpdateKeys();
		}
	}

	public static void OnHideGUI(UnityModManager.ModEntry modEntry)
	{
		BackupManager.Stop();
		IsListening = false;
	}

	private static void DrawMigrateMenu()
	{
		if (bold == null || Bigbold == null)
		{
			bold = new GUIStyle
			{
				fontSize = 30,
				fontStyle = FontStyle.Bold
			};
			bold.normal.textColor = Color.white;
			Bigbold = new GUIStyle
			{
				fontSize = 40,
				fontStyle = FontStyle.Bold
			};
			Bigbold.normal.textColor = Color.red;
		}
		GUILayout.Label(Lang.GetString("MIGRATION_WARNING"), Bigbold);
		GUILayout.BeginVertical();
		GUILayout.Space(4f);
		GUILayout.EndVertical();
		GUILayout.Label(Lang.GetString("MIGRATE_FROM_V2_KEYVIEWER"), bold);
		GUILayout.BeginHorizontal();
		GUILayout.Label("KeyCounts.kc:");
		v2Arg.keyCountsPath = GUILayout.TextField(v2Arg.keyCountsPath);
		if (GUILayout.Button("Choose"))
		{
			v2Arg.keyCountsPath = StandaloneFileBrowser.OpenFilePanel("Select KeyCounts", "", new ExtensionFilter[1]
			{
				new ExtensionFilter("KeyCounts", "kc")
			}, multiselect: false)[0];
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("KeySettings.ks:");
		v2Arg.keySettingsPath = GUILayout.TextField(v2Arg.keySettingsPath);
		if (GUILayout.Button("Choose"))
		{
			v2Arg.keySettingsPath = StandaloneFileBrowser.OpenFilePanel("Select KeySettings", "", new ExtensionFilter[1]
			{
				new ExtensionFilter("KeySettings", "ks")
			}, multiselect: false)[0];
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Settings.xml (Must Not Be Null!):");
		v2Arg.settingsPath = GUILayout.TextField(v2Arg.settingsPath);
		if (GUILayout.Button("Choose"))
		{
			v2Arg.settingsPath = StandaloneFileBrowser.OpenFilePanel("Select Settings", "", new ExtensionFilter[1]
			{
				new ExtensionFilter("Settings", "xml")
			}, multiselect: false)[0];
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(Lang.GetString("MIGRATE")))
		{
			try
			{
				Settings = Migrator.V2(KeyManager, v2Arg).Migrate();
				Lang.ChangeLanguage(Settings.Language);
				KeyManager.Profile = Settings.CurrentProfile;
				MigrateErrorString = string.Empty;
			}
			catch (FileNotFoundException ex)
			{
				MigrateErrorString = "File Not Found: '" + ex.FileName + "'";
			}
			catch (InvalidOperationException ex2)
			{
				MigrateErrorString = ex2.Message;
			}
		}
		GUILayout.Label(MigrateErrorString);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(Lang.GetString("GO_BACK")))
		{
			IsMigrating = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private static void DrawGroupSettingsGUI()
	{
		if (!(KeyManager.Profile.EditingKeyGroups = GUILayout.Toggle(KeyManager.Profile.EditingKeyGroups, Lang.GetString("KEY_GROUPS"))))
		{
			return;
		}
		MoreGUILayout.BeginIndent();
		GUILayout.BeginHorizontal();
		List<Group> keyGroups = KeyManager.Profile.KeyGroups;
		if (GUILayout.Button(Lang.GetString("NEW")))
		{
			keyGroups.Add(new Group(KeyManager, $"Group {keyGroups.Count}"));
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		for (int i = 0; i < keyGroups.Count; i++)
		{
			Group group = keyGroups[i];
			if (group.Editing = GUILayout.Toggle(group.Editing, group.Name))
			{
				group.RenderGUI();
			}
		}
		MoreGUILayout.EndIndent();
	}

	private static void DrawProfileSettingsGUI()
	{
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(Lang.GetString("MIGRATE_MENU")))
		{
			IsMigrating = true;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(4f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(Lang.GetString("NEW")))
		{
			Settings.Profiles.Add(new Profile());
			Settings.ProfileIndex = Settings.Profiles.Count - 1;
			Profile currentProfile = Settings.CurrentProfile;
			currentProfile.Name = currentProfile.Name + "Profile " + Settings.Profiles.Count;
			KeyManager.Profile = Settings.CurrentProfile;
		}
		if (GUILayout.Button(Lang.GetString("DUPLICATE")))
		{
			Settings.Profiles.Add(Settings.CurrentProfile.Copy());
			Settings.ProfileIndex = Settings.Profiles.Count - 1;
			Settings.CurrentProfile.Name += " Copy";
			KeyManager.Profile = Settings.CurrentProfile;
		}
		if (GUILayout.Button(Lang.GetString("IMPORT")))
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Profile", Persistence.GetLastUsedFolder(), "xml", multiselect: false);
			if (array.Length == 0)
			{
				return;
			}
			Profile profile = (Profile)new XmlSerializer(typeof(Profile)).Deserialize(File.OpenRead(array[0]));
			if (Settings.Profiles.FirstOrDefault((Profile p) => p.Name == profile.Name) != null)
			{
				profile.Name += "_Duplicate";
			}
			profile.Init(KeyManager);
			Settings.Profiles.Add(profile);
		}
		if (GUILayout.Button(Lang.GetString("EXPORT")))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Profile));
			string[] array2 = StandaloneFileBrowser.OpenFolderPanel("Profile", Persistence.GetLastUsedFolder(), multiselect: false);
			if (array2.Length == 0)
			{
				return;
			}
			xmlSerializer.Serialize(File.OpenWrite(Path.Combine(array2[0], KeyManager.Profile.Name + ".xml")), KeyManager.Profile);
		}
		if (Settings.Profiles.Count > 1 && GUILayout.Button(Lang.GetString("DELETE")))
		{
			Settings.Profiles.RemoveAt(Settings.ProfileIndex);
			Settings.ProfileIndex = Math.Min(Settings.ProfileIndex, Settings.Profiles.Count - 1);
			KeyManager.Profile = Settings.CurrentProfile;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(4f);
		Settings.CurrentProfile.Name = MoreGUILayout.NamedTextField(Lang.GetString("PROFILE_NAME"), Settings.CurrentProfile.Name, 400f);
		GUILayout.Label(Lang.GetString("PROFILES"));
		int selectedIndex = Settings.ProfileIndex;
		if (MoreGUILayout.ToggleList(Settings.Profiles, ref selectedIndex, (Profile p) => p.Name))
		{
			Settings.ProfileIndex = selectedIndex;
			KeyManager.Profile = Settings.CurrentProfile;
			KPSCalculator.Start(Settings.CurrentProfile);
		}
	}

	private static void DrawKeyRegisterSettingsGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(Lang.GetString("REGISTERED_KEYS"));
		KeyManager.Profile.IgnoreSkippedKeys = GUILayout.Toggle(KeyManager.Profile.IgnoreSkippedKeys, Lang.GetString("IGNORE_SKIPPED_KEYS"));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Space(8f);
		GUILayout.EndVertical();
		List<Key.Config> activeKeys = KeyManager.Profile.ActiveKeys;
		for (int i = 0; i < activeKeys.Count; i++)
		{
			Key.Config config = activeKeys[i];
			if (config.SpecialType != 0)
			{
				if (GUILayout.Button(config.SpecialType.ToString()))
				{
					KeyManager.Profile.ActiveKeys.RemoveAt(i);
					KeyManager.UpdateKeys();
				}
			}
			else if (GUILayout.Button(config.Code.ToString()))
			{
				KeyManager.Profile.ActiveKeys.RemoveAt(i);
				KeyManager.UpdateKeys();
			}
			GUILayout.Space(8f);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(12f);
		GUILayout.BeginHorizontal();
		if (IsListening)
		{
			if (GUILayout.Button(Lang.GetString("DONE")))
			{
				IsListening = false;
			}
			GUILayout.Label(Lang.GetString("PRESS_KEY_REGISTER"));
		}
		else if (GUILayout.Button(Lang.GetString("CHANGE_KEYS")))
		{
			IsListening = true;
		}
		if (GUILayout.Button(Lang.GetString("CLEAR_KEY_COUNT")))
		{
			KeyManager.ClearCounts();
		}
		if (!KeyManager.specialKeys.Keys.Contains(SpecialKeyType.KPS) && GUILayout.Button("Register KPS Key"))
		{
			KeyManager.Profile.ActiveKeys.Add(new Key.Config(KeyManager, SpecialKeyType.KPS));
			KeyManager.UpdateKeys();
		}
		if (!KeyManager.specialKeys.Keys.Contains(SpecialKeyType.Total) && GUILayout.Button("Register Total Key"))
		{
			KeyManager.Profile.ActiveKeys.Add(new Key.Config(KeyManager, SpecialKeyType.Total));
			KeyManager.UpdateKeys();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		Profile profile = KeyManager.Profile;
		profile.MakeBarSpecialKeys = GUILayout.Toggle(profile.MakeBarSpecialKeys, Lang.GetString("MAKE_BAR_SPECIAL_KEYS"));
	}

	private static void DrawKeyViewerSettingsGUI()
	{
		MoreGUILayout.BeginIndent();
		KeyManager.Profile.ViewerOnlyGameplay = GUILayout.Toggle(KeyManager.Profile.ViewerOnlyGameplay, Lang.GetString("VIEWER_ONLY_GAMEPLAY"));
		KeyManager.Profile.AnimateKeys = GUILayout.Toggle(KeyManager.Profile.AnimateKeys, Lang.GetString("ANIMATE_KEYS"));
		if (KeyManager.Profile.LimitNotRegisteredKeys = GUILayout.Toggle(KeyManager.Profile.LimitNotRegisteredKeys, Lang.GetString("LIMIT_NOT_REGISTERED_KEYS")))
		{
			KeyManager.Profile.LimitNotRegisteredKeysOnCLS = GUILayout.Toggle(KeyManager.Profile.LimitNotRegisteredKeysOnCLS, Lang.GetString("LIMIT_NOT_REGISTERED_KEYS_ON_CLS"));
			KeyManager.Profile.LimitNotRegisteredKeysOnMain = GUILayout.Toggle(KeyManager.Profile.LimitNotRegisteredKeysOnMain, Lang.GetString("LIMIT_NOT_REGISTERED_KEYS_ON_MAIN"));
		}
		bool flag = GUILayout.Toggle(KeyManager.Profile.ShowKeyPressTotal, Lang.GetString("SHOW_KEY_PRESS_TOTAL"));
		if (flag != KeyManager.Profile.ShowKeyPressTotal)
		{
			KeyManager.Profile.ShowKeyPressTotal = flag;
			KeyManager.UpdateLayout();
		}
		float num = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_SIZE"), KeyManager.Profile.KeyViewerSize, 10f, 200f, 300f, 1f);
		if ((double)num != (double)KeyManager.Profile.KeyViewerSize)
		{
			KeyManager.Profile.KeyViewerSize = num;
			KeyManager.UpdateLayout();
		}
		float num2 = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_X_POS"), KeyManager.Profile.KeyViewerXPos, -1f, 1f, 300f, 0.01f, 0f, "{0:0.##}");
		if ((double)num2 != (double)KeyManager.Profile.KeyViewerXPos)
		{
			KeyManager.Profile.KeyViewerXPos = num2;
			KeyManager.UpdateLayout();
		}
		float num3 = MoreGUILayout.NamedSlider(Lang.GetString("KEY_VIEWER_Y_POS"), KeyManager.Profile.KeyViewerYPos, -1f, 1f, 300f, 0.01f, 0f, "{0:0.##}");
		if ((double)num3 != (double)KeyManager.Profile.KeyViewerYPos)
		{
			KeyManager.Profile.KeyViewerYPos = num3;
			KeyManager.UpdateLayout();
		}
		GUILayout.Space(8f);
		GUILayout.BeginHorizontal();
		GUILayout.Label(Lang.GetString("KPS_UPDATE_RATE"));
		int.TryParse(GUILayout.TextField(KeyManager.Profile.KPSUpdateRateMs.ToString()), out KeyManager.Profile.KPSUpdateRateMs);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		bool flag2 = GUILayout.Toggle(KeyManager.Profile.ApplyWithOffset, "Apply Config With Offset");
		if (flag2 != KeyManager.Profile.ApplyWithOffset)
		{
			KeyManager.Profile.ApplyWithOffset = flag2;
			if (!KeyManager.Profile.EditEachKeys)
			{
				ApplyEachKeys(KeyManager.Profile.GlobalConfig);
			}
		}
		bool flag3 = GUILayout.Toggle(Settings.CurrentProfile.EditEachKeys, Lang.GetString("EDIT_SETTINGS_EACH_KEYS"));
		if (flag3)
		{
			foreach (Key value in KeyManager.keys.Values)
			{
				value.RenderGUI();
			}
		}
		else
		{
			Key.DrawGlobalConfig(KeyManager.Profile.GlobalConfig, ApplyEachKeys);
		}
		if (flag3 != Settings.CurrentProfile.EditEachKeys)
		{
			Settings.CurrentProfile.EditEachKeys = flag3;
			ApplyEachKeys(KeyManager.Profile.GlobalConfig);
			KeyManager.UpdateKeys();
		}
		foreach (Key value2 in KeyManager.specialKeys.Values)
		{
			value2.RenderGUI();
		}
		MoreGUILayout.EndIndent();
	}

	public static void ApplyEachKeys(Key.Config keyConfig)
	{
		foreach (Key value in KeyManager.keys.Values)
		{
			if (KeyManager.Profile.ApplyWithOffset)
			{
				value.config.ApplyConfig(keyConfig);
			}
			else
			{
				value.config.ApplyConfigWithoutOffset(keyConfig);
			}
		}
		KeyManager.UpdateLayout();
	}

	public static bool Equals(this VertexGradient left, VertexGradient right)
	{
		return left.topLeft == right.topLeft && left.topRight == right.topRight && left.bottomLeft == right.bottomLeft && left.bottomRight == right.bottomRight;
	}

	public static bool Inequals(this VertexGradient left, VertexGradient right)
	{
		return left.topLeft != right.topLeft || left.topRight != right.topRight || left.bottomLeft != right.bottomLeft || left.bottomRight != right.bottomRight;
	}

	public static void CheckNull(this object obj, string identifier)
	{
		Logger.Log(identifier + " Is " + ((obj == null) ? "Null" : "Not Null"));
	}

	public static T LogObject<T>(this T obj)
	{
		Logger.Log(obj.ToString());
		return obj;
	}

	public static uint Sum(this IEnumerable<uint> enumerable)
	{
		uint num = 0u;
		foreach (uint item in enumerable)
		{
			num += item;
		}
		return num;
	}

	public static Sprite GetSprite(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		if (spriteCache.TryGetValue(path, out var value))
		{
			return value;
		}
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.LoadImage(File.ReadAllBytes(path));
		return spriteCache[path] = texture2D.ToSprite();
	}

	public static bool DrawEnum<T>(string title, ref T @enum) where T : Enum
	{
		T[] array = (T[])Enum.GetValues(typeof(T));
		string[] values = array.Select((T x) => x.ToString()).ToArray();
		int selected = Array.IndexOf(array, @enum);
		int num = (UnityModManager.UI.PopupToggleGroup(ref selected, values, title, null) ? 1 : 0);
		@enum = array[selected];
		return num != 0;
	}

	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
	{
		key = pair.Key;
		value = pair.Value;
	}

	public static void LogDifference<T>(Capture<T> oldCapture, Capture<T> newCapture)
	{
		if ((object)oldCapture.original != (object)oldCapture.original)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Comparing {oldCapture.original} Difference");
		foreach (string item in oldCapture.values.Keys.Union(newCapture.values.Keys))
		{
			stringBuilder.Append(' ', 4).Append(item + " => ");
			if (oldCapture.values.TryGetValue(item, out var value))
			{
				stringBuilder.Append(string.Format("Old Value: {0}, ", value ?? "null"));
			}
			else
			{
				stringBuilder.Append("Old Value: Not Exists, ");
			}
			if (newCapture.values.TryGetValue(item, out var value2))
			{
				stringBuilder.Append(string.Format("New Value: {0} ", value2 ?? "null"));
			}
			else
			{
				stringBuilder.Append("New Value: Not Exists ");
			}
			if (object.Equals(value, value2))
			{
				stringBuilder.AppendLine("(Old == New)");
			}
			else
			{
				stringBuilder.AppendLine("<color=green>(Old != New)</color>");
			}
		}
		Logger.Log($"\n{stringBuilder}");
	}
}
