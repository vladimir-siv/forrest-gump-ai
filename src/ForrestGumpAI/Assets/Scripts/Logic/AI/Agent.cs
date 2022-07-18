using System;
using UnityEngine;
using GrandIntelligence;

public class Agent
{
	public const uint Inputs = 5u;
	public const uint Outputs = 3u;

	private readonly float[] sensors = new float[Inputs];
	private readonly float[] outputs = new float[Outputs];

	private Runner runner = null;
	private float score = 0f;

	public BasicBrain Brain { get; set; } = null;

	public void AssignAgent(Runner runner)
	{
		this.runner = runner;
		runner.PathwayEntered += pathway => score += pathway.Difficulty;
		runner.RunnerDeath += RunnerDeath;
		for (var i = 0; i < sensors.Length; ++i) sensors[i] = 0f;
		score = 0f;
	}

	public void Think()
	{
		RefreshSensors();

		Brain.NeuralNetwork.Input.Transfer(sensors);
		Brain.NeuralNetwork.Eval();
		Brain.NeuralNetwork.Output.Retrieve(outputs);

		var decision = outputs.ArgMax() - 1;
		if (decision != 0) runner.Steer(decision);
	}

	private void RefreshSensors()
	{
		const float MaxRayDistance = 10f;

		Vector3 direction(int i)
		{
			switch (i)
			{
				case 0: return -runner.transform.right;
				case 1: return (runner.transform.forward - runner.transform.right).normalized;
				case 2: return runner.transform.forward;
				case 3: return (runner.transform.forward + runner.transform.right).normalized;
				case 4: return runner.transform.right;
				default: throw new ArgumentException($"Invalid direction: {i}.");
			}
		}

		for (var i = 0; i < Inputs; ++i)
		{
			var hit = runner.Raycast(direction(i), MaxRayDistance);
			if (hit != null) sensors[i] = hit.Value.distance / MaxRayDistance;
			else sensors[i] = 1f;
		}
	}

	private void RunnerDeath()
	{
		var distance = Vector3.Distance(runner.transform.position, runner.LastPathway.ExitPoint);
		var time = (float)runner.TimeSinceSpawned.TotalSeconds;

		var reward = Mathf.Pow(score / .17e3f, 4f);
		var penalty = Mathf.Pow(distance / .54e3f, 4f) * time;

		Brain.EvolutionValue = Mathf.Max(reward - penalty, float.Epsilon);
	}
}
