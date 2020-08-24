using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class AIController
{
	private static Agent[] agents;
	private static Bot[] bots;

	public static void Setup()
	{
		agents = Dependency.Controller.Agents;
		bots = new Bot[agents.Length];

		for (var i = 0; i < bots.Length; ++i)
		{
			bots[i] = new Bot();
		}
	}

	public static void CycleBegin()
	{
		for (var i = 0; i < agents.Length; ++i)
		{
			bots[i].Agent = agents[i];
		}
	}

	public static void Loop()
	{
		for (var i = 0; i < agents.Length; ++i)
		{
			bots[i].Think();
		}
	}

	public static void CycleEnd()
	{

	}
}
