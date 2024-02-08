using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer;

public class Group
{
	internal KeyManager keyManager;

	public List<KeyCode> codes = new List<KeyCode>();

	public Key.Config groupConfig;

	public string Name = "Group";

	public bool Editing;

	private bool isResolved;

	private List<Key.Config> configs = new List<Key.Config>();

	public Group()
	{
	}

	public Group(KeyManager keyManager, string name)
	{
		this.keyManager = keyManager;
		Name = name;
		groupConfig = new Key.Config(keyManager);
	}

	public void AddConfig(Key.Config config)
	{
		configs.Add(config);
		codes.Add(config.Code);
	}

	public bool RemoveConfig(Key.Config config)
	{
		return configs.Remove(config) & codes.Remove(config.Code);
	}

	public void Resolve()
	{
		configs.Clear();
		for (int i = 0; i < codes.Count; i++)
		{
			configs.Add(keyManager.keys[codes[i]].config);
		}
		groupConfig.keyManager = keyManager;
		isResolved = true;
	}

	public bool IsAdded(Key.Config config)
	{
		return codes.Contains(config.Code);
	}

	public void RenderGUI()
	{
		if (!isResolved)
		{
			Resolve();
		}
		MoreGUILayout.BeginIndent();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(Main.Lang.GetString("DUPLICATE")))
		{
			Main.Settings.CurrentProfile.KeyGroups.Add(Copy());
		}
		if (GUILayout.Button(Main.Lang.GetString("DELETE")))
		{
			Main.Settings.CurrentProfile.KeyGroups.Remove(this);
		}
		if (GUILayout.Button("Add All Codes"))
		{
			foreach (KeyCode code in keyManager.Codes)
			{
				if (!codes.Contains(code))
				{
					AddConfig(keyManager[code].config);
				}
			}
		}
		GUILayout.Label("Codes: ");
		for (int i = 0; i < configs.Count; i++)
		{
			GUILayout.Label($"{configs[i].Code} ");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		Name = MoreGUILayout.NamedTextField(Main.Lang.GetString("KEY_GROUP_NAME"), Name, 400f);
		GUILayout.BeginHorizontal();
		float num = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("OFFSET_X"), groupConfig.OffsetX, -Screen.width, Screen.width, 300f);
		float num2 = MoreGUILayout.NamedSliderContent(Main.Lang.GetString("OFFSET_Y"), groupConfig.OffsetY, -Screen.height, Screen.height, 300f);
		if ((double)num != (double)groupConfig.OffsetX)
		{
			groupConfig.OffsetX = num;
			configs.ForEach(delegate(Key.Config conf)
			{
				if (keyManager.Profile.ApplyWithOffset)
				{
					conf.ApplyOffsetRelative(groupConfig);
				}
			});
			keyManager.UpdateLayout();
		}
		if ((double)num2 != (double)groupConfig.OffsetY)
		{
			groupConfig.OffsetY = num2;
			configs.ForEach(delegate(Key.Config conf)
			{
				if (keyManager.Profile.ApplyWithOffset)
				{
					conf.ApplyOffsetRelative(groupConfig);
				}
			});
			keyManager.UpdateLayout();
		}
		GUILayout.EndHorizontal();
		Key.DrawGlobalConfig(groupConfig, delegate(Key.Config c)
		{
			configs.ForEach(delegate(Key.Config conf)
			{
				if (keyManager.Profile.ApplyWithOffset)
				{
					conf.ApplyConfig(c);
				}
				else
				{
					conf.ApplyConfigWithoutOffset(c);
				}
			});
			keyManager.UpdateLayout();
		});
		MoreGUILayout.EndIndent();
	}

	public Group Copy()
	{
		Group group = new Group(keyManager, Name + " Copy");
		group.configs.AddRange(configs);
		return group;
	}
}
