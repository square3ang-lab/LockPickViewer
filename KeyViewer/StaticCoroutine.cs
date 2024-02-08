using System.Collections;
using UnityEngine;

namespace KeyViewer;

public class StaticCoroutine : MonoBehaviour
{
	private static StaticCoroutine instance;

	public static StaticCoroutine Instance
	{
		get
		{
			if ((bool)instance)
			{
				return instance;
			}
			instance = new GameObject("KeyViewer_StaticCoroutine").AddComponent<StaticCoroutine>();
			return instance;
		}
	}

	public static Coroutine Run(IEnumerator coroutine)
	{
		return Instance.StartCoroutine(coroutine);
	}
}
