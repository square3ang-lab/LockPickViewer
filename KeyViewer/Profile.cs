using System.Collections.Generic;
using System.Linq;

namespace KeyViewer;

public class Profile
{
	public string Name = "Default Profile";

	public Key.Config GlobalConfig = new Key.Config();

	public List<Group> KeyGroups = new List<Group>();

	public List<Key.Config> ActiveKeys = new List<Key.Config>();

	public bool MakeBarSpecialKeys = true;

	public bool IgnoreSkippedKeys;

	public bool ViewerOnlyGameplay;

	public bool EditingKeyGroups;

	public bool AnimateKeys = true;

	public bool ShowKeyPressTotal = true;

	public bool LimitNotRegisteredKeys;

	public float KeyViewerSize = 100f;

	public float KeyViewerXPos = 0.89f;

	public float KeyViewerYPos = 0.03f;

	public int KPSUpdateRateMs = 1000;

	public bool EditEachKeys;

	public bool ResetWhenStart;

	public bool ApplyWithOffset;

	internal bool LimitNotRegisteredKeysOnCLS;

	internal bool LimitNotRegisteredKeysOnMain;

	public Profile Copy()
	{
		return new Profile
		{
			Name = Name,
			GlobalConfig = GlobalConfig.Copy(),
			IgnoreSkippedKeys = IgnoreSkippedKeys,
			MakeBarSpecialKeys = MakeBarSpecialKeys,
			KeyGroups = KeyGroups.Select((Group g) => g.Copy()).ToList(),
			ActiveKeys = ActiveKeys.Select((Key.Config c) => c.Copy()).ToList(),
			ViewerOnlyGameplay = ViewerOnlyGameplay,
			AnimateKeys = AnimateKeys,
			ShowKeyPressTotal = ShowKeyPressTotal,
			LimitNotRegisteredKeys = LimitNotRegisteredKeys,
			KeyViewerSize = KeyViewerSize,
			KeyViewerXPos = KeyViewerXPos,
			KeyViewerYPos = KeyViewerYPos,
			ApplyWithOffset = ApplyWithOffset
		};
	}

	public void Init(KeyManager manager)
	{
		GlobalConfig.keyManager = manager;
		KeyGroups.ForEach(delegate(Group g)
		{
			g.keyManager = manager;
		});
		ActiveKeys.ForEach(delegate(Key.Config k)
		{
			k.keyManager = manager;
		});
	}
}
