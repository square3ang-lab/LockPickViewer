using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.API;

public static class InputAPI
{
	internal static readonly Dictionary<KeyCode, bool> APIFlags = new Dictionary<KeyCode, bool>();

	private static bool active;

	public static bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
			EventActive = value;
			APIFlags.Clear();
		}
	}

	public static bool EventActive { get; set; }

	public static event Action<KeyCode> OnKeyPressed;

	public static event Action<KeyCode> OnKeyReleased;

	public static void PressKey(KeyCode key)
	{
		APIFlags[key] = true;
	}

	public static void ReleaseKey(KeyCode key)
	{
		APIFlags[key] = false;
	}

	internal static void KeyPress(KeyCode key)
	{
		InputAPI.OnKeyPressed(key);
	}

	internal static void KeyRelease(KeyCode key)
	{
		InputAPI.OnKeyReleased(key);
	}

	static InputAPI()
	{
		InputAPI.OnKeyPressed = delegate
		{
		};
		InputAPI.OnKeyReleased = delegate
		{
		};
	}
}
