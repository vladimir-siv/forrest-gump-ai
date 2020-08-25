using UnityEngine;
using GrandIntelligence;

public class Bot
{
	private readonly float[] outputs = new float[3];

	public Agent Agent { get; private set; } = null;
	public BasicBrain Brain { get; set; } = null;

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

	public void Init(Agent agent)
	{
		Agent = agent;
		agent.AgentPreDeath += AgentDeath;
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

		var pathway = agent.CurrentPathway.GetComponent<IPathway>();
		var distance = Vector3.Distance(agent.transform.position, pathway.ExitPoint);
		var progress = EvolutionTracker.Progress();
		var decay = agent.PathwaysEncountered * 10f;

		var reward = Mathf.Pow(progress / .17e2f, 4f);
		var penalty = Mathf.Pow(distance / .24e3f, 4f) * decay;

		Brain.EvolutionValue = Mathf.Max(reward - penalty, float.Epsilon);
	}
}
