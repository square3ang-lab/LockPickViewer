using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace KeyViewer;

public static class EnumPatcher<T> where T : Enum
{
	private static readonly Type thisType;

	private static readonly Dictionary<string, ulong> addedFields;

	public static void AddField(string name, ulong value)
	{
		addedFields[name] = value;
	}

	static EnumPatcher()
	{
		thisType = typeof(T);
		addedFields = new Dictionary<string, ulong>();
		Main.Harmony.Patch(Main.GCVAN, null, new HarmonyMethod(typeof(EnumPatcher<T>).GetMethod("GCVAN_Patch", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty)));
	}

	private static void GCVAN_Patch(Type enumType, object __result)
	{
		if (!(enumType != thisType))
		{
			IEnumerable<string> source = Main.VAN_Names(__result).Concat(addedFields.Keys);
			IEnumerable<ulong> source2 = Main.VAN_Values(__result).Concat(addedFields.Values);
			Main.VAN_Names(__result) = source.ToArray();
			Main.VAN_Values(__result) = source2.ToArray();
		}
	}
}
