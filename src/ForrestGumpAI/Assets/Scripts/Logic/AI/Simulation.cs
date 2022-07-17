using GrandIntelligence;

public class Simulation
{
	private Agent[] agents = null;
	private DarwinBgea evolution = null;

	public void Begin(Agent[] agents)
	{
		this.agents = agents;

		var first = new Population((uint)agents.Length);
		for (var i = 0; i < agents.Length; ++i)
			first.Add(agents[i].Brain);

		evolution = new DarwinBgea
		(
			first,
			Selection.SpeciatedRandFit(20f),
			NeatBrain.RandomUniformCrossover(),
			generations: 1000u,
			mutation: 10.0f
		);
	}

	public void Start()
	{

	}

	public void End()
	{
		evolution?.Dispose();
		evolution = null;
	}

	public void CycleEnd()
	{
		if (evolution == null) return;

		evolution.Cycle();

		NeatBrain best = null;

		for (var i = 0; i < agents.Length; ++i)
		{
			agents[i].Brain = (NeatBrain)evolution.Generation[(uint)i];

			var prev = (NeatBrain)evolution.Previous[(uint)i];
			if (best == null || prev.EvolutionValue > best.EvolutionValue) best = prev;
		}
	}
}
