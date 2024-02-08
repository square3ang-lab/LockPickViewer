using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace KeyViewer.Patches;

[HarmonyPatch(typeof(scrController), "CountValidKeysPressed")]
public static class CountValidKeysPressedPatch
{
	public static readonly List<KeyCode> AlwaysBoundKeys = new List<KeyCode>
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
		KeyCode.Mouse2,
		KeyCode.Mouse3,
		KeyCode.Mouse4
	};

	public static bool Prefix(ref int __result)
	{
		if (Main.IsListening)
		{
			__result = 0;
			return false;
		}
		Profile profile = Main.KeyManager.Profile;
		if (!profile.LimitNotRegisteredKeys || (!profile.LimitNotRegisteredKeysOnCLS && ADOBase.isCLS) || (!profile.LimitNotRegisteredKeysOnMain && !scrController.instance.gameworld && !ADOBase.isCLS))
		{
			return true;
		}
		int num = 0;
		int b = ((!AsyncInputManager.isActive) ? (num + (profile.ActiveKeys.Count((Key.Config k) => Input.GetKeyDown(k.Code)) + AlwaysBoundKeys.Count(Input.GetKeyDown))) : (num + (profile.ActiveKeys.Count((Key.Config k) => AsyncInputCompat.GetKeyDown(k.Code)) + AlwaysBoundKeys.Count(AsyncInputCompat.GetKeyDown))));
		__result = Mathf.Min(4, b);
		return false;
	}
}
