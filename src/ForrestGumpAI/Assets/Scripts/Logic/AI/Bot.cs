using System;
using UnityEngine;
using GrandIntelligence;

public class Bot
{
	public const uint Inputs = 5u;

	private static NeuralBuilder prototype = null;
	public static NeuralBuilder BrainPrototype
	{
		get
		{
			if (prototype == null)
			{
				prototype = new NeuralBuilder(Shape.As2D(1u, Inputs));
				prototype.FCLayer(8u, ActivationFunction.ELU);
				prototype.FCLayer(4u, ActivationFunction.ELU);
				prototype.FCLayer(3u, ActivationFunction.Sigmoid);
			}

			return prototype;
		}
		set
		{
			if (value == prototype) return;
			prototype?.Dispose();
			prototype = value;
		}
	}

	private readonly float[] sensors = new float[Inputs];
	private readonly float[] outputs = new float[3];

	public Agent Agent { get; private set; } = null;
	public BasicBrain Brain { get; set; } = null;

	public Bot(BasicBrain brain) => Brain = brain ?? throw new ArgumentNullException(nameof(brain));
	public Bot()
	{
		Brain = new BasicBrain(BrainPrototype);
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
		for (var i = 0; i < sensors.Length; ++i) sensors[i] = 0f;
	}

	public void Think()
	{
		if (Agent == null || !Agent.CanSteer) return;

		RefreshSensors();

		Brain.NeuralNetwork.Input.Transfer(sensors);
		Brain.NeuralNetwork.Eval();
		Brain.NeuralNetwork.Output.Retrieve(outputs);

		var decision = outputs.ArgMax() - 1;
		if (decision != 0) Agent.Steer(decision);
	}

	private void RefreshSensors()
	{
		const float MaxRayDistance = 10f;

		Vector3 direction(int i)
		{
			switch (i)
			{
				case 0: return -Agent.transform.right;
				case 1: return (Agent.transform.forward - Agent.transform.right).normalized;
				case 2: return Agent.transform.forward;
				case 3: return (Agent.transform.forward + Agent.transform.right).normalized;
				case 4: return Agent.transform.right;
				default: throw new ArgumentException($"Invalid direction: {i}.");
			}
		}

		for (var i = 0; i < 5; ++i)
		{
			if (Physics.Raycast(Agent.transform.position, direction(i), out var hit, MaxRayDistance, Wall.Mask)) sensors[i] = hit.distance / MaxRayDistance;
			else sensors[i] = 1f;
		}
	}

	private void AgentDeath(Agent agent)
	{
		if (agent != Agent) return;
		Agent = null;

		var distance = Vector3.Distance(agent.transform.position, agent.LastPathway.ExitPoint);
		var time = EvolutionTracker.Time();

		var reward = Mathf.Pow(agent.Score / .17e3f, 4f);
		var penalty = Mathf.Pow(distance / .54e3f, 4f) * time;

		Brain.EvolutionValue = Mathf.Max(reward - penalty, float.Epsilon);
	}
}
