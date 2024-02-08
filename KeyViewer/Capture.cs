using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace KeyViewer;

public class Capture
{
	private static bool RequireSkip(CaptureMode mode, Type t, string name, Predicate<string> exclude)
	{
		if (t == null || (exclude != null && exclude(name)))
		{
			return true;
		}
		return mode switch
		{
			CaptureMode.Class => !t.IsClass, 
			CaptureMode.Struct => !t.IsValueType && !t.IsPrimitive, 
			_ => false, 
		};
	}

	public static Capture<T> CaptureValues<T>(T t, bool includePrivate = false, CaptureMode captureMode = CaptureMode.ClassAndStruct, Predicate<string> exclude = null)
	{
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
		if (includePrivate)
		{
			bindingFlags |= BindingFlags.NonPublic;
		}
		if (object.Equals(t, null))
		{
			return null;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		FieldInfo[] fields = Capture<T>.Type.GetFields(bindingFlags);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!RequireSkip(captureMode, fieldInfo.FieldType, fieldInfo.Name, exclude))
			{
				dictionary.Add(fieldInfo.Name, fieldInfo.GetValue(t));
			}
		}
		PropertyInfo[] properties = Capture<T>.Type.GetProperties(bindingFlags);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (!RequireSkip(captureMode, propertyInfo.PropertyType, propertyInfo.Name, exclude) && !(propertyInfo.GetGetMethod(includePrivate) == null) && propertyInfo.GetIndexParameters().Length == 0)
			{
				dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(t));
			}
		}
		return new Capture<T>(t, includePrivate, captureMode, exclude, dictionary);
	}

	public static Capture<T> CaptureClasses<T>(T t, bool includePrivate = false, Predicate<string> exclude = null)
	{
		return CaptureValues(t, includePrivate, CaptureMode.Class, exclude);
	}

	public static Capture<T> CaptureStructs<T>(T t, bool includePrivate = false, Predicate<string> exclude = null)
	{
		return CaptureValues(t, includePrivate, CaptureMode.Struct, exclude);
	}

	public static void UncaptureValues<T>(Capture<T> capture, T target)
	{
		BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
		if (capture.includePrivate)
		{
			bindingFlags |= BindingFlags.NonPublic;
		}
		FieldInfo[] fields = Capture<T>.Type.GetFields(bindingFlags);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (capture.values.TryGetValue(fieldInfo.Name, out var value))
			{
				fieldInfo.SetValue(target, value);
			}
		}
		PropertyInfo[] properties = Capture<T>.Type.GetProperties(bindingFlags);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (!(propertyInfo.GetSetMethod(capture.includePrivate) == null) && propertyInfo.GetIndexParameters().Length == 0 && capture.values.TryGetValue(propertyInfo.Name, out var value2))
			{
				propertyInfo.SetValue(target, value2);
			}
		}
	}
}
public class Capture<T>
{
	public static readonly Type Type = typeof(T);

	public readonly T original;

	public readonly bool includePrivate;

	public readonly CaptureMode captureMode;

	public readonly Predicate<string> excludePredicate;

	public readonly Dictionary<string, object> values;

	internal Capture(T original, bool includePrivate, CaptureMode captureMode, Predicate<string> excludePredicate, Dictionary<string, object> values)
	{
		this.original = original;
		this.includePrivate = includePrivate;
		this.captureMode = captureMode;
		this.excludePredicate = excludePredicate;
		this.values = values;
	}

	public static implicit operator Capture<T>(T t)
	{
		return Capture.CaptureValues(t);
	}

	public static implicit operator T(Capture<T> capture)
	{
		ConstructorInfo constructor = Type.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty, null, Type.EmptyTypes, null);
		T val = ((!(constructor != null)) ? ((T)FormatterServices.GetUninitializedObject(Type)) : ((T)constructor.Invoke(null)));
		Capture.UncaptureValues(capture, val);
		return val;
	}
}
