using GrandIntelligence;

public class Bot
{
	private readonly float[] outputs = new float[3];

	private Agent agent = null;
	public Agent Agent
	{
		get
		{
			return agent;
		}
		set
		{
			if (value == agent) return;
			agent = value;
			if (agent == null) return;
			agent.AgentPreDeath += AgentDeath;
		}
	}

	private BasicBrain brain = null;
	public BasicBrain Brain
	{
		get
		{
			return brain;
		}
		set
		{
			brain = value;
		}
	}

	public Bot()
	{
		Brain = new BasicBrain(AIController.BrainPrototype);
		using (var randomize = Device.Active.Prepare("randomize"))
		using (var it = new NeuralIterator())
		{
			randomize.Set('U');
			randomize.Set(-1.0f, 0);
			randomize.Set(+1.0f, 1);

			for (var param = it.Begin(Brain.NeuralNetwork); param != null; param = it.Next())
			{
				randomize.Set(param.Memory);
				API.Wait(API.Invoke(randomize.Handle));
			}
		}
	}

	public void Think()
	{
		if (Agent == null) return;

		Brain.NeuralNetwork.Input.Transfer(Agent.RayValues);
		Brain.NeuralNetwork.Eval();
		Brain.NeuralNetwork.Output.Retrieve(outputs);

		var decision = outputs.ArgMax() - 1;
		if (decision != 0) Agent.Steer(decision);
	}

	private void AgentDeath(Agent agent)
	{
		if (agent != Agent) return;
		Agent = null;
		Brain.EvolutionValue = EvolutionTracker.Progress();
	}
}
