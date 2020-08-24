using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject WallPrototype = null;
	public GameObject PathwayConnectorPrototype = null;
	public GameObject StraightPathwayPrototype = null;
	public GameObject AgentPrototype = null;
	public GameObject[] AgentModels = null;
	public Agent[] Agents = null;

	[SerializeField] Vector3 SpawnPoint = Vector3.zero;
	[SerializeField] float SpawnRotation = 0f;

	public int AgentsLeft { get; private set; }
	private readonly TerrainGenerator terrain = new TerrainGenerator();

	private void Start()
	{
		Random.InitState(6326);
		Dependency.Create(this);
		Restart();
	}
	private void Restart()
	{
		var spawn = terrain.Begin(SpawnPoint, SpawnRotation);

		for (var i = 0; i < Agents.Length; ++i)
		{
			Agents[i] = ObjectActivator.Construct<Agent>();
			Agents[i].AgentDeath += OnAgentDeath;
			Agents[i].transform.position = spawn.transform.position - spawn.transform.forward * 2.5f;
			Agents[i].transform.rotation = spawn.transform.rotation;
		}

		AgentsLeft = Agents.Length;
	}

	public void OnAgentDeath(Agent agent)
	{
		--AgentsLeft;
		StartCoroutine("CollectRagdoll", agent);
	}
	private IEnumerator CollectRagdoll(object agent)
	{
		yield return Timing.RagdollTimeout;
		ObjectActivator.Destruct((Agent)agent);
		if (AgentsLeft == 0) Restart();
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			Agents[0].Run();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Agents[0].Die();
		}
	}
}
