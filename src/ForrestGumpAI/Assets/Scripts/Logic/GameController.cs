using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GrandIntelligence;

public class GameController : MonoBehaviour
{
	public GameObject WallPrototype = null;
	public GameObject PathwayConnectorPrototype = null;
	public GameObject StraightPathwayPrototype = null;
	public GameObject RiggedPathwayPrototype = null;
	public GameObject SpreadPathwayPrototype = null;
	public GameObject RunnerPrototype = null;
	public GameObject[] RunnerModels = null;
	public Runner[] Runners = null;

	[SerializeField] private Vector3 SpawnPoint = Vector3.zero;
	[SerializeField] private float SpawnRotation = 0f;
	[SerializeField] private uint TerrainLevel = 0u;
	[SerializeField] private CameraController CameraController = null;
	[SerializeField] private Text GenerationDisplay = null;
	[SerializeField] private Text TimeDisplay = null;
	[SerializeField] private Text AgentInfoDisplay = null;

	public int AgentsAlive { get; private set; }
	public int AgentsLeft { get; private set; }

	private Agent[] agents = null;
	private TerrainGenerator terrain = null;
	private DateTime startTime;

	private Simulation simulation = new Simulation();
	private uint generation = 0u;

	private void Start()
	{
		switch (TerrainLevel)
		{
			case 0u: terrain = TerrainGenerator.Easy; break;
			case 1u: terrain = TerrainGenerator.Medium; break;
			case 2u: terrain = TerrainGenerator.Hard; break;
			default: terrain = TerrainGenerator.Medium; break;
		}

		Dependency.Create(this);
		GICore.Init(new Spec(GrandIntelligence.DeviceType.Cpu));

		agents = new Agent[Runners.Length];
		for (var i = 0; i < agents.Length; ++i)
			agents[i] = new Agent();

		simulation.Begin(agents);
		Restart();

		StartCoroutine("UpdateDynamicInfo");
	}
	private void Restart()
	{
		simulation.EpisodeStart();

		var spawn = terrain.Begin(SpawnPoint, SpawnRotation);

		terrain.Generate();
		terrain.Generate();
		terrain.Generate();

		for (var i = 0; i < Runners.Length; ++i)
		{
			var runner = ObjectActivator.Construct<Runner>();
			agents[i].AssignAgent(runner);
			Runners[i] = runner;
			Runners[i].RunnerDeath += () => StartCoroutine("RunnerDeath", runner);
			Runners[i].transform.position = spawn.transform.position - spawn.transform.forward * 2.5f;
			Runners[i].transform.rotation = spawn.transform.rotation;
			Runners[i].Run();
		}

		AgentsAlive = AgentsLeft = Runners.Length;

		startTime = DateTime.Now;
		++generation;

		UpdateAgentCount();
	}

	private IEnumerator RunnerDeath(object runner)
	{
		--AgentsAlive;
		UpdateAgentCount();
		simulation.RunnerTerminated(AgentsAlive);

		yield return Timing.RagdollTimeout;

		ObjectActivator.Destruct((Runner)runner);

		if (--AgentsLeft == 0)
		{
			Restart();
		}
	}

	private void ForceNextGeneration()
	{
		for (var i = 0; i < Runners.Length; ++i)
		{
			Runners[i].Die();
		}
	}

	public void GenerateTerrain()
	{
		terrain.Generate();
	}

#if AI_PLAYER
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			ForceNextGeneration();
		}
	}
	private void FixedUpdate()
	{
		for (var i = 0; i < agents.Length; ++i)
			if (!Runners[i].IsDead && Runners[i].IsRunning && Runners[i].CanSteer)
				agents[i].Think();
	}
	private void LateUpdate()
	{
		var diff = (DateTime.Now - terrain.LastGenerationTime).TotalSeconds;
		if (diff > 30f) ForceNextGeneration();
	}
#endif

	private void OnApplicationQuit()
	{
		simulation.End();
		GICore.Release();
	}

	#region UI

	private void UpdateAgentCount()
	{
		GenerationDisplay.text = $"Generation:\t{generation:0000}\nAgents alive:\t{AgentsAlive:0000}";
	}
	private IEnumerator UpdateDynamicInfo()
	{
		while (true)
		{
			yield return Timing.DynamicInfoRefreshTimeout;
			TimeDisplay.text = $"Time:  {(float)(DateTime.Now - startTime).TotalSeconds:0000.0000}s";
			AgentInfoDisplay.text = $"Agent: {CameraController.FollowingIndex + 1:0000}";
		}
	}

	#endregion
}
