using GrandIntelligence;

public static class AIController
{
	private static Agent[] agents = null;
	private static Bot[] bots = null;

	private static DarwinBgea system = null;
	private static NeuralBuilder prototype = null;

	public static NeuralBuilder BrainPrototype
	{
		get
		{
			if (prototype == null)
			{
				prototype = new NeuralBuilder(5u);
				prototype.FCLayer(8u, ActivationFunction.ELU);
				prototype.FCLayer(4u, ActivationFunction.ELU);
				prototype.FCLayer(3u, ActivationFunction.Sigmoid);
			}

			return prototype;
		}
	}
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
			mutation: 25.0f
		);
	}

	public static void Cleanup()
	{
		prototype?.Dispose();
		prototype = null;

		system?.Dispose();
		system = null;

		GICore.Release();
	}

	public static void CycleBegin()
	{
		for (var i = 0; i < agents.Length; ++i)
		{
			bots[i].Agent = agents[i];
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
