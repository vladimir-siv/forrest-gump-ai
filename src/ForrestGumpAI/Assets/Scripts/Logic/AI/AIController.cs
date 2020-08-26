using GrandIntelligence;

public static class AIController
{
	private static Agent[] agents = null;
	private static Bot[] bots = null;

	private static DarwinBgea system = null;

	public static uint Generation => system?.CurrentGeneration ?? 0u;

	public static void Setup()
	{
		GICore.Init(new Spec(DeviceType.Cpu));

		agents = Dependency.Controller.Agents;
		bots = new Bot[agents.Length];

		var first = new Population((uint)bots.Length);

		for (var i = 0; i < bots.Length; ++i)
		{
			bots[i] = new Bot();
			first.Add(bots[i].Brain);
		}

		system = new DarwinBgea
		(
			first,
			Selection.RandFit(1u),
			BasicBrain.Mating(first.Size, ((BasicBrain)first[0u]).NeuralNetwork.Params),
			generations: 1000u,
			mutation: 15.0f
		);
	}

	public static void Cleanup()
	{
		system?.Dispose();
		system = null;

		Bot.BrainPrototype = null;

		GICore.Release();
	}

	public static void CycleBegin()
	{
		for (var i = 0; i < agents.Length; ++i)
		{
			bots[i].Init(agents[i]);
		}

		EvolutionTracker.Begin();
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
		if (system == null) return;

		system.Cycle();

		for (var i = 0; i < bots.Length; ++i)
		{
			bots[i].Brain = (BasicBrain)system.Generation[(uint)i];
		}
	}
}
