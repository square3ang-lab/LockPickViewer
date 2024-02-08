using KeyViewer.Migration.V2;

namespace KeyViewer.Migration;

public sealed class Migrator
{
	public static IMigrator V2(KeyManager manager, string keyCountsPath, string keySettingsPath, string settingsPath)
	{
		return new V2Migrator(manager, keyCountsPath, keySettingsPath, settingsPath);
	}

	public static IMigrator V2(KeyManager manager, V2MigratorArgument v2Arg)
	{
		return new V2Migrator(manager, v2Arg);
	}
}
