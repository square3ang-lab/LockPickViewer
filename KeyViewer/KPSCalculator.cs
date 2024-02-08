using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace KeyViewer;

public static class KPSCalculator
{
	private static Profile Profile;

	private static Thread CalculatingThread;

	private static int PressCount;

	public static int Kps;

	public static int Max;

	public static double Average;

	public static void Start(Profile profile)
	{
		try
		{
			Profile = profile;
			if (CalculatingThread == null)
			{
				(CalculatingThread = GetCalculateThread()).Start();
			}
		}
		catch
		{
		}
	}

	public static void Stop()
	{
		try
		{
			CalculatingThread.Abort();
		}
		catch
		{
		}
	}

	public static void Press()
	{
		PressCount++;
	}

	private static Thread GetCalculateThread()
	{
		return new Thread((ThreadStart)delegate
		{
			try
			{
				LinkedList<int> linkedList = new LinkedList<int>();
				int num = 0;
				long num2 = 0L;
				Stopwatch stopwatch = Stopwatch.StartNew();
				while (true)
				{
					bool flag = true;
					while (stopwatch.ElapsedMilliseconds < Profile.KPSUpdateRateMs)
					{
					}
					int pressCount = PressCount;
					PressCount = 0;
					int num3 = pressCount;
					foreach (int item in linkedList)
					{
						num3 += item;
					}
					Max = Math.Max(num3, Max);
					if (num3 != 0)
					{
						Average = (Average * (double)num2 + (double)num3) / ((double)num2 + 1.0);
						num2++;
						num += pressCount;
					}
					linkedList.AddFirst(pressCount);
					if (linkedList.Count >= 1000 / Profile.KPSUpdateRateMs)
					{
						linkedList.RemoveLast();
					}
					Kps = num3;
					stopwatch.Restart();
					Thread.Sleep(Math.Max(Profile.KPSUpdateRateMs - 1, 0));
				}
			}
			finally
			{
				CalculatingThread = null;
			}
		});
	}
}
