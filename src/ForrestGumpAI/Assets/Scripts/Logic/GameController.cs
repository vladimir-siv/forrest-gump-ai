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

	[SerializeField] Vector3 SpawnPoint = Vector3.zero;
	[SerializeField] float SpawnRotation = 0f;
	[SerializeField] Text GenerationDisplay = null;

	public int AgentsAlive { get; private set; }
	public int AgentsLeft { get; private set; }
	private readonly TerrainGenerator terrain = new TerrainGenerator(Terrains.Medium);

	private void Start()
	{
		Dependency.Create(this);
		AIController.Setup();
		Restart();
	}
	private void Restart()
	{
		var spawn = terrain.Begin(SpawnPoint, SpawnRotation);

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

		UpdateDisplay();
	}

	private IEnumerator AgentDeath(object agent)
	{
		--AgentsAlive;
		UpdateDisplay();

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
		terrain.Generate();
	}
	public void DestructTerrain()
	{
		while (terrain.Remove()) ;
	}

#if AI_PLAYER
	private void Update() => AIController.Loop();
	private void FixedUpdate() => AIController.FixedLoop();
#endif

	private void OnApplicationQuit() => AIController.Cleanup();

	private void UpdateDisplay()
	{
		GenerationDisplay.text = $"Generation:\t{AIController.Generation:0000}\nAgents alive:\t{AgentsAlive:0000}";
	}
}
