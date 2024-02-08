using UnityEngine;

namespace KeyViewer;

public static class KeyInput
{
	public static bool AnyKey => (!AsyncInputManager.isActive) ? Input.anyKey : AsyncInputCompat.AnyKey;

	public static bool AnyKeyDown => (!AsyncInputManager.isActive) ? Input.anyKeyDown : AsyncInputCompat.AnyKeyDown;

	public static bool GetKey(KeyCode code)
	{
		return AsyncInputManager.isActive ? AsyncInputCompat.GetKey(code) : Input.GetKey(code);
	}

	public static bool GetKeyUp(KeyCode code)
	{
		return AsyncInputManager.isActive ? AsyncInputCompat.GetKeyUp(code) : Input.GetKeyUp(code);
	}

	public static bool GetKeyDown(KeyCode code)
	{
		return AsyncInputManager.isActive ? AsyncInputCompat.GetKeyDown(code) : Input.GetKeyDown(code);
	}
}
