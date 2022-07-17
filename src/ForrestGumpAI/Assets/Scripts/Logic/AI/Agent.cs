using System;
using UnityEngine;
using GrandIntelligence;

public class Agent
{
	public const uint Inputs = 5u;
	public const uint Outputs = 3u;

	private readonly float[] sensors = new float[Inputs];
	private readonly float[] outputs = new float[Outputs];

	private Runner Runner { get; set; } = null;
	public NeatBrain Brain { get; set; } = null;

	public Agent() => Brain = new NeatBrain(Inputs, 0u, Outputs, ActivationFunction.ELU, ActivationFunction.Sigmoid);

	public void AssignAgent(Runner runner)
	{
		Runner = runner;
		runner.RunnerDeath += RunnerDeath;
		Brain.Compile();
		for (var i = 0; i < sensors.Length; ++i) sensors[i] = 0f;
	}

	public void Think()
	{
		RefreshSensors();

		Brain.NeuralNetwork.Input.Transfer(sensors);
		Brain.NeuralNetwork.Eval();
		Brain.NeuralNetwork.Output.Retrieve(outputs);

		var decision = outputs.ArgMax() - 1;
		if (decision != 0) Runner.Steer(decision);
	}

	private void RefreshSensors()
	{
		const float MaxRayDistance = 10f;

		Vector3 direction(int i)
		{
			switch (i)
			{
				case 0: return -Runner.transform.right;
				case 1: return (Runner.transform.forward - Runner.transform.right).normalized;
				case 2: return Runner.transform.forward;
				case 3: return (Runner.transform.forward + Runner.transform.right).normalized;
				case 4: return Runner.transform.right;
				default: throw new ArgumentException($"Invalid direction: {i}.");
			}
		}

		for (var i = 0; i < 5; ++i)
		{
			if (Physics.Raycast(Runner.transform.position, direction(i), out var hit, MaxRayDistance, Wall.Mask)) sensors[i] = hit.distance / MaxRayDistance;
			else sensors[i] = 1f;
		}
	}

	private void RunnerDeath()
	{
		var distance = Vector3.Distance(Runner.transform.position, Runner.LastPathway.ExitPoint);
		var time = (float)Runner.TimeSinceSpawned.TotalSeconds;

		var reward = Mathf.Pow(Runner.Score / .17e3f, 4f);
		var penalty = Mathf.Pow(distance / .54e3f, 4f) * time;

		Brain.EvolutionValue = Mathf.Max(reward - penalty, float.Epsilon);
	}
}
