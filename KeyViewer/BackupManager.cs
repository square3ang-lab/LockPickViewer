using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace KeyViewer;

public static class BackupManager
{
	private static CancellationTokenSource cts;

	public static void Start()
	{
		if (!Directory.Exists(Path.Combine(Main.Mod.Path, "Backups")))
		{
			Directory.CreateDirectory(Path.Combine(Main.Mod.Path, "Backups"));
		}
		cts = new CancellationTokenSource();
		new Thread(delegate(object t)
		{
			BackupThread((CancellationToken)t);
		}).Start(cts.Token);
	}

	public static void Stop()
	{
		cts.Cancel();
		cts = null;
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: false);
	}

	private static void BackupThread(CancellationToken cancelTok)
	{
		WriteBackup();
		while (!cancelTok.IsCancellationRequested)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (stopwatch.Elapsed.Seconds < Main.Settings.BackupInterval)
			{
			}
			stopwatch.Stop();
			WriteBackup();
		}
	}

	private static void WriteBackup()
	{
		DateTime now = DateTime.Now;
		using FileStream stream = new FileStream(Path.Combine(Main.Mod.Path, "Backups", $"{now.Year}-{now.Month}-{now.Day} {now.Hour:D2}h{now.Minute:D2}m{now.Second:D2}s Backup.xml"), FileMode.OpenOrCreate);
		new XmlSerializer(typeof(Settings), (XmlAttributeOverrides)null).Serialize(stream, Main.Settings);
	}
}
