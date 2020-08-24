using System;
using UnityEngine;

public static class EvolutionTracker
{
	private static DateTime StartTime;

	public static void Begin()
	{
		StartTime = DateTime.Now;
	}

	public static float Progress()
	{
		var time = (float)(DateTime.Now - StartTime).TotalSeconds;
		return Mathf.Pow(time / 100f, 4f);
	}
}
