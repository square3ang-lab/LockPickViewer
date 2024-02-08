using System;

namespace KeyViewer.API;

public static class EventAPI
{
	public static event Action<KeyManager> OnUpdateKeysLayout;

	public static event Action<Key> OnUpdateKeyLayout;

	public static event Action OnClearCounts;

	internal static void UpdateLayout(KeyManager manager)
	{
		EventAPI.OnUpdateKeysLayout(manager);
	}

	internal static void UpdateLayout(Key key)
	{
		EventAPI.OnUpdateKeyLayout(key);
	}

	internal static void ClearCounts()
	{
		EventAPI.OnClearCounts();
	}

	static EventAPI()
	{
		EventAPI.OnUpdateKeysLayout = delegate
		{
		};
		EventAPI.OnUpdateKeyLayout = delegate
		{
		};
		EventAPI.OnClearCounts = delegate
		{
		};
	}
}
