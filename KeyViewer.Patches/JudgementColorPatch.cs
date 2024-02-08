using System;
using System.Linq;
using HarmonyLib;
using KeyViewer.API;
using SkyHook;
using UnityEngine;

namespace KeyViewer.Patches;

[HarmonyPatch]
public static class JudgementColorPatch
{
	private static bool initialized;

	private static ShiftStack<KeyCode> keys;

	private static bool stackFlushed;

	public static void Init()
	{
		if (initialized)
		{
			return;
		}
		keys = new ShiftStack<KeyCode>(delegate
		{
			KeyManager keyManager = Main.KeyManager;
			return Math.Max((keyManager != null) ? keyManager.keys.Count : 0, 2);
		}, KeyCode.None);
		SkyHookManager.KeyUpdated.AddListener(delegate(SkyHookEvent she)
		{
			if (AsyncInputManager.isActive && !(Main.KeyManager == null) && she.Type != SkyHook.EventType.KeyReleased && Main.IsEnabled)
			{
				scrController instance = scrController.instance;
				if (!((UnityEngine.Object)(object)instance != null))
				{
					if (0 == 0)
					{
						return;
					}
				}
				else if (!instance.gameworld)
				{
					return;
				}
				KeyCode keyCode = AsyncInputCompat.Convert(she.Label);
				if (Main.KeyManager.Codes.Contains(keyCode))
				{
					keys.Push(keyCode);
				}
			}
		});
		initialized = true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(scrController), "ValidInputWasTriggered")]
	public static void SyncInput_StackPushPatch(scrController __instance)
	{
		if (AsyncInputManager.isActive)
		{
			return;
		}
		foreach (KeyCode code in Main.KeyManager.Codes)
		{
			if (Input.GetKey(code))
			{
				keys.Push(code);
			}
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(scrController), "Update")]
	public static void StackFlushPatch(scrController __instance)
	{
		if (!stackFlushed && __instance.state == States.PlayerControl)
		{
			keys.Clear();
			stackFlushed = true;
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(scrController), "Awake_Rewind")]
	public static void StackFlushPatch2(scrController __instance)
	{
		stackFlushed = false;
	}

	[Comment("Change Key's Background Color")]
	[HarmonyPrefix]
	[HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
	public static void ChangeColorPatch(HitMargin hit)
	{
		KeyCode code;
		while ((code = keys.Pop()) != 0)
		{
			Main.KeyManager[code].ChangeHitMarginColor(hit);
		}
	}
}
