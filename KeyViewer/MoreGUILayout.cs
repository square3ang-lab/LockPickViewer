using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KeyViewer;

public static class MoreGUILayout
{
	public static (VertexGradient, VertexGradient) VertexGradientSlidersPair(VertexGradient gradient1, VertexGradient gradient2)
	{
		GUILayout.BeginHorizontal();
		VertexGradient item = VertexGradientSlider(gradient1);
		VertexGradient vertexGradient = VertexGradientSlider(gradient2);
		GUILayout.EndHorizontal();
		VertexGradient item2 = vertexGradient;
		return (item, item2);
	}

	public static VertexGradient VertexGradientSlider(VertexGradient gradient)
	{
		Color color = gradient.topLeft;
		Color color2 = gradient.topRight;
		Color color3 = gradient.bottomLeft;
		Color color4 = gradient.bottomRight;
		float num = Mathf.Round(color.r * 255f);
		float num2 = Mathf.Round(color.g * 255f);
		float num3 = Mathf.Round(color.b * 255f);
		float num4 = Mathf.Round(color.a * 255f);
		float num5 = Mathf.Round(color2.r * 255f);
		float num6 = Mathf.Round(color2.g * 255f);
		float num7 = Mathf.Round(color2.b * 255f);
		float num8 = Mathf.Round(color2.a * 255f);
		float num9 = Mathf.Round(color3.r * 255f);
		float num10 = Mathf.Round(color3.g * 255f);
		float num11 = Mathf.Round(color3.b * 255f);
		float num12 = Mathf.Round(color3.a * 255f);
		float num13 = Mathf.Round(color4.r * 255f);
		float num14 = Mathf.Round(color4.g * 255f);
		float num15 = Mathf.Round(color4.b * 255f);
		float num16 = Mathf.Round(color4.a * 255f);
		GUILayout.BeginVertical();
		GUILayout.Label("Top Left");
		GUILayout.BeginVertical();
		float num17 = NamedSlider("R:", num, 0f, 255f, 300f, 1f, 40f);
		float num18 = NamedSlider("G:", num2, 0f, 255f, 300f, 1f, 40f);
		float num19 = NamedSlider("B:", num3, 0f, 255f, 300f, 1f, 40f);
		float num20 = NamedSlider("A:", num4, 0f, 255f, 300f, 1f, 40f);
		GUILayout.EndVertical();
		GUILayout.Label("Top Right");
		GUILayout.BeginVertical();
		float num21 = NamedSlider("R:", num5, 0f, 255f, 300f, 1f, 40f);
		float num22 = NamedSlider("G:", num6, 0f, 255f, 300f, 1f, 40f);
		float num23 = NamedSlider("B:", num7, 0f, 255f, 300f, 1f, 40f);
		float num24 = NamedSlider("A:", num8, 0f, 255f, 300f, 1f, 40f);
		GUILayout.EndVertical();
		GUILayout.Label("Bottom Left");
		GUILayout.BeginVertical();
		float num25 = NamedSlider("R:", num9, 0f, 255f, 300f, 1f, 40f);
		float num26 = NamedSlider("G:", num10, 0f, 255f, 300f, 1f, 40f);
		float num27 = NamedSlider("B:", num11, 0f, 255f, 300f, 1f, 40f);
		float num28 = NamedSlider("A:", num12, 0f, 255f, 300f, 1f, 40f);
		GUILayout.EndVertical();
		GUILayout.Label("Bottom Right");
		GUILayout.BeginVertical();
		float num29 = NamedSlider("R:", num13, 0f, 255f, 300f, 1f, 40f);
		float num30 = NamedSlider("G:", num14, 0f, 255f, 300f, 1f, 40f);
		float num31 = NamedSlider("B:", num15, 0f, 255f, 300f, 1f, 40f);
		float num32 = NamedSlider("A:", num16, 0f, 255f, 300f, 1f, 40f);
		GUILayout.EndVertical();
		GUILayout.EndVertical();
		if ((double)num != (double)num17 || (double)num2 != (double)num18 || (double)num3 != (double)num19 || (double)num4 != (double)num20)
		{
			color = new Color(num17 / 255f, num18 / 255f, num19 / 255f, num20 / 255f);
		}
		if ((double)num5 != (double)num21 || (double)num6 != (double)num22 || (double)num7 != (double)num23 || (double)num8 != (double)num24)
		{
			color2 = new Color(num21 / 255f, num22 / 255f, num23 / 255f, num24 / 255f);
		}
		if ((double)num9 != (double)num25 || (double)num10 != (double)num26 || (double)num11 != (double)num27 || (double)num12 != (double)num28)
		{
			color3 = new Color(num25 / 255f, num26 / 255f, num27 / 255f, num28 / 255f);
		}
		if ((double)num13 != (double)num29 || (double)num14 != (double)num30 || (double)num15 != (double)num31 || (double)num16 != (double)num32)
		{
			color4 = new Color(num29 / 255f, num30 / 255f, num31 / 255f, num32 / 255f);
		}
		return new VertexGradient(color, color2, color3, color4);
	}

	public static void BeginIndent(float indentSize = 20f)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(indentSize);
		GUILayout.BeginVertical();
	}

	public static void EndIndent()
	{
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	public static Color ColorRgbSliders(Color color)
	{
		float num = Mathf.Round(color.r * 255f);
		float num2 = Mathf.Round(color.g * 255f);
		float num3 = Mathf.Round(color.b * 255f);
		float num4 = NamedSlider("R:", num, 0f, 255f, 300f, 1f, 40f);
		float num5 = NamedSlider("G:", num2, 0f, 255f, 300f, 1f, 40f);
		float num6 = NamedSlider("B:", num3, 0f, 255f, 300f, 1f, 40f);
		return ((double)num != (double)num4 || (double)num2 != (double)num5 || (double)num3 != (double)num6) ? new Color(num4 / 255f, num5 / 255f, num6 / 255f) : color;
	}

	public static Color ColorRgbaSliders(Color color)
	{
		float num = Mathf.Round(color.r * 255f);
		float num2 = Mathf.Round(color.g * 255f);
		float num3 = Mathf.Round(color.b * 255f);
		float num4 = Mathf.Round(color.a * 255f);
		float num5 = NamedSlider("R:", num, 0f, 255f, 300f, 1f, 40f);
		float num6 = NamedSlider("G:", num2, 0f, 255f, 300f, 1f, 40f);
		float num7 = NamedSlider("B:", num3, 0f, 255f, 300f, 1f, 40f);
		float num8 = NamedSlider("A:", num4, 0f, 255f, 300f, 1f, 40f);
		return ((double)num != (double)num5 || (double)num2 != (double)num6 || (double)num3 != (double)num7 || (double)num4 != (double)num8) ? new Color(num5 / 255f, num6 / 255f, num7 / 255f, num8 / 255f) : color;
	}

	public static (Color, Color) ColorRgbSlidersPair(Color color1, Color color2)
	{
		float num = Mathf.Round(color1.r * 255f);
		float num2 = Mathf.Round(color1.g * 255f);
		float num3 = Mathf.Round(color1.b * 255f);
		float num4 = Mathf.Round(color2.r * 255f);
		float num5 = Mathf.Round(color2.g * 255f);
		float num6 = Mathf.Round(color2.b * 255f);
		var (num7, num8) = NamedSliderPair("R:", "R:", num, num4, 0f, 255f, 300f, 1f, 40f);
		var (num9, num10) = NamedSliderPair("G:", "G:", num2, num5, 0f, 255f, 300f, 1f, 40f);
		var (num11, num12) = NamedSliderPair("B:", "B:", num3, num6, 0f, 255f, 300f, 1f, 40f);
		if ((double)num != (double)num7 || (double)num2 != (double)num9 || (double)num3 != (double)num11)
		{
			color1 = new Color(num7 / 255f, num9 / 255f, num11 / 255f);
		}
		if ((double)num4 != (double)num8 || (double)num5 != (double)num10 || (double)num6 != (double)num12)
		{
			color2 = new Color(num8 / 255f, num10 / 255f, num12 / 255f);
		}
		return (color1, color2);
	}

	public static (Color, Color) ColorRgbaSlidersPair(Color color1, Color color2)
	{
		float num = Mathf.Round(color1.r * 255f);
		float num2 = Mathf.Round(color1.g * 255f);
		float num3 = Mathf.Round(color1.b * 255f);
		float num4 = Mathf.Round(color1.a * 255f);
		float num5 = Mathf.Round(color2.r * 255f);
		float num6 = Mathf.Round(color2.g * 255f);
		float num7 = Mathf.Round(color2.b * 255f);
		float num8 = Mathf.Round(color2.a * 255f);
		var (num9, num10) = NamedSliderPair("R:", "R:", num, num5, 0f, 255f, 300f, 1f, 40f);
		var (num11, num12) = NamedSliderPair("G:", "G:", num2, num6, 0f, 255f, 300f, 1f, 40f);
		var (num13, num14) = NamedSliderPair("B:", "B:", num3, num7, 0f, 255f, 300f, 1f, 40f);
		var (num15, num16) = NamedSliderPair("A:", "A:", num4, num8, 0f, 255f, 300f, 1f, 40f);
		if ((double)num != (double)num9 || (double)num2 != (double)num11 || (double)num3 != (double)num13 || (double)num4 != (double)num15)
		{
			color1 = new Color(num9 / 255f, num11 / 255f, num13 / 255f, num15 / 255f);
		}
		if ((double)num5 != (double)num10 || (double)num6 != (double)num12 || (double)num7 != (double)num14 || (double)num8 != (double)num16)
		{
			color2 = new Color(num10 / 255f, num12 / 255f, num14 / 255f, num16 / 255f);
		}
		return (color1, color2);
	}

	public static float NamedSlider(string name, float value, float leftValue, float rightValue, float sliderWidth, float roundNearest = 0f, float labelWidth = 0f, string valueFormat = "{0}")
	{
		GUILayout.BeginHorizontal();
		double num = NamedSliderContent(name, value, leftValue, rightValue, sliderWidth, roundNearest, labelWidth, valueFormat);
		GUILayout.EndHorizontal();
		return (float)num;
	}

	public static (float, float) NamedSliderPair(string name1, string name2, float value1, float value2, float leftValue, float rightValue, float sliderWidth, float roundNearest = 0f, float labelWidth = 0f, string valueFormat = "{0}")
	{
		GUILayout.BeginHorizontal();
		double num = NamedSliderContent(name1, value1, leftValue, rightValue, sliderWidth, roundNearest, labelWidth, valueFormat);
		float num2 = NamedSliderContent(name2, value2, leftValue, rightValue, sliderWidth, roundNearest, labelWidth, valueFormat);
		GUILayout.EndHorizontal();
		double num3 = num2;
		return ((float)num, (float)num3);
	}

	public static float NamedSliderContent(string name, float value, float leftValue, float rightValue, float sliderWidth, float roundNearest = 0f, float labelWidth = 0f, string valueFormat = "{0}")
	{
		if ((double)labelWidth == 0.0)
		{
			GUILayout.Label(name);
			GUILayout.Space(4f);
		}
		else
		{
			GUILayout.Label(name, GUILayout.Width(labelWidth));
		}
		float result = GUILayout.HorizontalSlider(value, leftValue, rightValue, GUILayout.Width(sliderWidth));
		if ((double)roundNearest != 0.0)
		{
			result = Mathf.Round(result / roundNearest) * roundNearest;
		}
		GUILayout.Space(8f);
		if (valueFormat != "{0}")
		{
			GUILayout.Label(string.Format(valueFormat, result));
		}
		else
		{
			float.TryParse(GUILayout.TextField(result.ToString("F2")), out result);
		}
		GUILayout.FlexibleSpace();
		return result;
	}

	public static string NamedTextField(string name, string value, float fieldWidth = 0f, float labelWidth = 0f)
	{
		GUILayout.BeginHorizontal();
		string result = NamedTextFieldContent(name, value, fieldWidth, labelWidth);
		GUILayout.EndHorizontal();
		return result;
	}

	public static (string, string) NamedTextFieldPair(string name1, string name2, string value1, string value2, float fieldWidth, float labelWidth = 0f)
	{
		GUILayout.BeginHorizontal();
		string item = NamedTextFieldContent(name1, value1, fieldWidth, labelWidth);
		string text = NamedTextFieldContent(name2, value2, fieldWidth, labelWidth);
		GUILayout.EndHorizontal();
		string item2 = text;
		return (item, item2);
	}

	private static string NamedTextFieldContent(string name, string value, float fieldWidth = 0f, float labelWidth = 0f)
	{
		if ((double)labelWidth == 0.0)
		{
			GUILayout.Label(name);
			GUILayout.Space(4f);
		}
		else
		{
			GUILayout.Label(name, GUILayout.Width(labelWidth));
		}
		string result = ((!((double)fieldWidth > 0.0)) ? GUILayout.TextField(value) : GUILayout.TextField(value, GUILayout.Width(fieldWidth)));
		GUILayout.FlexibleSpace();
		return result;
	}

	public static void LabelPair(string text1, string text2, float textWidth = 0f)
	{
		GUILayout.BeginHorizontal();
		if ((double)textWidth == 0.0)
		{
			GUILayout.Label(text1);
			GUILayout.Space(4f);
		}
		else
		{
			GUILayout.Label(text1, GUILayout.Width(textWidth));
		}
		GUILayout.FlexibleSpace();
		GUILayout.Space(8f);
		if ((double)textWidth == 0.0)
		{
			GUILayout.Label(text2);
			GUILayout.Space(4f);
		}
		else
		{
			GUILayout.Label(text2, GUILayout.Width(textWidth));
		}
		GUILayout.FlexibleSpace();
		GUILayout.Space(20f);
		GUILayout.EndHorizontal();
	}

	public static bool ToggleList<T>(List<T> list, ref int selectedIndex, Func<T, string> nameFunc)
	{
		bool result = false;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < list.Count; i++)
		{
			T arg = list[i];
			string text = nameFunc(arg);
			GUILayout.BeginHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("▲") && i > 0)
			{
				num = i;
			}
			if (GUILayout.Button("▼") && i < list.Count - 1)
			{
				num2 = i;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);
			if (GUILayout.Toggle(selectedIndex == i, text) && selectedIndex != i)
			{
				selectedIndex = i;
				result = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		if (num != -1)
		{
			result = true;
			T value = list[num];
			list[num] = list[num - 1];
			list[num - 1] = value;
			if (num - 1 == selectedIndex)
			{
				selectedIndex++;
			}
			else if (num == selectedIndex)
			{
				selectedIndex--;
			}
		}
		else if (num2 != -1)
		{
			result = true;
			T value2 = list[num2];
			list[num2] = list[num2 + 1];
			list[num2 + 1] = value2;
			if (num2 + 1 == selectedIndex)
			{
				selectedIndex--;
			}
			else if (num2 == selectedIndex)
			{
				selectedIndex++;
			}
		}
		return result;
	}

	public static void HorizontalLine(float thickness, float length = 0f)
	{
		GUIContent none = GUIContent.none;
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.margin = new RectOffset(8, 8, 4, 4);
		gUIStyle.padding = new RectOffset();
		gUIStyle.fixedHeight = thickness;
		gUIStyle.fixedWidth = length;
		gUIStyle.normal.background = Texture2D.whiteTexture;
		GUILayoutOption[] options = Array.Empty<GUILayoutOption>();
		GUILayout.Box(none, gUIStyle, options);
	}
}
