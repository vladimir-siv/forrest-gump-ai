using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public GameObject WallPrototype = null;
	public GameObject PathwayConnectorPrototype = null;
	public GameObject StraightPathwayPrototype = null;
	public GameObject RiggedPathwayPrototype = null;
	public GameObject SpreadPathwayPrototype = null;
	public GameObject AgentPrototype = null;
	public GameObject[] AgentModels = null;
	public Agent[] Agents = null;

	[SerializeField] private Vector3 SpawnPoint = Vector3.zero;
	[SerializeField] private float SpawnRotation = 0f;
	[SerializeField] private Text GenerationDisplay = null;
	[SerializeField] private Text TimeDisplay = null;
	[SerializeField] private uint TerrainLevel = 0u;

	public int AgentsAlive { get; private set; }
	public int AgentsLeft { get; private set; }

	private TerrainGenerator terrain = null;

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
		AIController.Setup();
		Restart();

		StartCoroutine("UpdateTime");
	}
	private void Restart()
	{
		var spawn = terrain.Begin(SpawnPoint, SpawnRotation);

		terrain.Generate();
		terrain.Generate();
		terrain.Generate();

		for (var i = 0; i < Agents.Length; ++i)
		{
			Agents[i] = ObjectActivator.Construct<Agent>();
			Agents[i].AgentDeath += agent => StartCoroutine("AgentDeath", agent);
			Agents[i].transform.position = spawn.transform.position - spawn.transform.forward * 2.5f;
			Agents[i].transform.rotation = spawn.transform.rotation;
			Agents[i].Run();
		}

		AgentsAlive = AgentsLeft = Agents.Length;
		
		AIController.CycleBegin();

		UpdateAgentCount();
	}

	private IEnumerator AgentDeath(object agent)
	{
		--AgentsAlive;
		UpdateAgentCount();

		yield return Timing.RagdollTimeout;

		ObjectActivator.Destruct((Agent)agent);
		
		if (--AgentsLeft == 0)
		{
			AIController.CycleEnd();
			Restart();
		}
	}

	public void GenerateTerrain()
	{
		terrain.Generate();
	}

#if AI_PLAYER
	private void Update()
	{
		AIController.Loop();
	}
	private void FixedUpdate()
	{
		AIController.FixedLoop();
	}
	private void LateUpdate()
	{
		var diff = (DateTime.Now - terrain.LastGenerationTime).TotalSeconds;
		if (diff > 30f) AIController.ForceNextGeneration();
	}
#endif

	private void OnApplicationQuit() => AIController.Cleanup();

	#region UI

	private void UpdateAgentCount()
	{
		GenerationDisplay.text = $"Generation:\t{AIController.Generation:0000}\nAgents alive:\t{AgentsAlive:0000}";
	}
	private IEnumerator UpdateTime()
	{
		while (true)
		{
			yield return Timing.TimeRefreshTimeout;
			TimeDisplay.text = $"Time:  {EvolutionTracker.Time():0000.0000}s";
		}
	}

	#endregion
}
