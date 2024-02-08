using System.Collections.Generic;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace KeyViewer;

public class Settings : UnityModManager.ModSettings
{
	public int ProfileIndex;

	public List<Profile> Profiles = new List<Profile>();

	public LanguageType Language = LanguageType.English;

	public int BackupInterval = 10;

	[XmlIgnore]
	public Profile CurrentProfile => Profiles[ProfileIndex];

	public override void Save(UnityModManager.ModEntry modEntry)
	{
		UnityModManager.ModSettings.Save(this, modEntry);
	}
}
