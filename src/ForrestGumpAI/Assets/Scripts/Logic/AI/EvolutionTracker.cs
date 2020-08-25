using System;

public static class EvolutionTracker
{
	private static DateTime StartTime;

	public static void Begin()
	{
		StartTime = DateTime.Now;
	}

	public static float Progress()
	{
		return (float)(DateTime.Now - StartTime).TotalSeconds;
	}
}
