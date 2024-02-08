using System;
using System.Collections.Generic;
using System.Linq;
using KeyViewer.API;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer;

public class KeyManager : MonoBehaviour, IDisposable
{
	private Profile profile;

	internal Canvas keysCanvas;

	internal Dictionary<KeyCode, Key> keys;

	internal Dictionary<SpecialKeyType, Key> specialKeys;

	internal RectTransform keysCanvasRt;

	public bool isPlaying;

	public Key this[KeyCode code]
	{
		get
		{
			return keys[code];
		}
		set
		{
			keys[code] = value;
		}
	}

	public Key this[SpecialKeyType type]
	{
		get
		{
			return specialKeys[type];
		}
		set
		{
			specialKeys[type] = value;
		}
	}

	public Profile Profile
	{
		get
		{
			return profile;
		}
		set
		{
			profile = value;
			KPSCalculator.Start(value);
			UpdateKeys();
		}
	}

	public IEnumerable<KeyCode> Codes => keys.Keys;

	public IEnumerable<Key> Keys => keys.Values;

	public void Init(Profile profile)
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		base.gameObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		base.gameObject.AddComponent<CanvasScaler>().referenceResolution = new Vector2(1280f, 720f);
		Profile = profile;
	}

	public void UpdateKeys()
	{
		if ((bool)keysCanvas)
		{
			UnityEngine.Object.Destroy(keysCanvas.gameObject);
		}
		GameObject gameObject = new GameObject("Canvas");
		gameObject.transform.SetParent(base.transform);
		keysCanvas = gameObject.AddComponent<Canvas>();
		keysCanvasRt = keysCanvas.GetComponent<RectTransform>();
		keys = new Dictionary<KeyCode, Key>();
		specialKeys = new Dictionary<SpecialKeyType, Key>();
		foreach (Key.Config item in Profile.ActiveKeys.Where((Key.Config c) => c.Code != KeyCode.None))
		{
			if (!keys.ContainsKey(item.Code))
			{
				keys.Add(item.Code, new GameObject($"{item.Code} Key").AddComponent<Key>().Init(this, item));
				continue;
			}
			KeyCode keyCode = Main.KeyCodes.First((KeyCode k) => Profile.ActiveKeys.FirstOrDefault((Key.Config kc) => kc.Code == k) == null);
			keys.Add(keyCode, new GameObject($"{item.Code} Key").AddComponent<Key>().Init(this, item));
			Main.Logger.Log($"{item.Code} Is Duplicate! One Of {item.Code} Key Was Changed To {keyCode}!");
		}
		foreach (Key.Config item2 in Profile.ActiveKeys.Where((Key.Config c) => c.SpecialType != SpecialKeyType.None))
		{
			specialKeys.Add(item2.SpecialType, new GameObject($"{item2.SpecialType} Key").AddComponent<Key>().Init(this, item2));
		}
		if (!Main.Settings.CurrentProfile.EditEachKeys)
		{
			Main.ApplyEachKeys(Profile.GlobalConfig);
		}
		UpdateLayout();
	}

	public void UpdateLayout()
	{
		EventAPI.UpdateLayout(this);
		int count = keys.Count;
		float y = (Profile.ShowKeyPressTotal ? 150f : 100f);
		float num = 10f;
		float x = (float)(count * 100) + (float)(count - 1) * num;
		Vector2 vector = new Vector2(Profile.KeyViewerXPos, Profile.KeyViewerYPos);
		keysCanvasRt.anchorMin = vector;
		keysCanvasRt.anchorMax = vector;
		keysCanvasRt.pivot = vector;
		keysCanvasRt.sizeDelta = new Vector2(x, y);
		keysCanvasRt.anchoredPosition = Vector2.zero;
		keysCanvasRt.localScale = new Vector3(1f, 1f, 1f) * Profile.KeyViewerSize / 100f;
		float x2 = 0f;
		float tempX = 0f;
		int num2 = 0;
		foreach (Key value in keys.Values)
		{
			value.UpdateLayout(ref x2, ref tempX, num2++);
		}
		float tempX2 = 0f;
		int num3 = 0;
		foreach (Key value2 in specialKeys.Values)
		{
			value2.UpdateLayout(ref x2, ref tempX2, num3++);
		}
	}

	public void ClearCounts()
	{
		EventAPI.ClearCounts();
		foreach (Key value2 in keys.Values)
		{
			value2.Count = 0u;
			value2.CountText.text = "0";
		}
		if (specialKeys.TryGetValue(SpecialKeyType.Total, out var value))
		{
			value.Count = 0u;
			value.CountText.text = "0";
		}
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		keys.Clear();
		keys = null;
		GC.SuppressFinalize(this);
	}
}
