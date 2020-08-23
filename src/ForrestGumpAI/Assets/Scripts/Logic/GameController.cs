using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject WallPrototype = null;
	public GameObject StraightPathwayPrototype = null;
	public GameObject AgentPrototype = null;
	public GameObject[] AgentModels = null;
	public Agent[] Agents = null;

	public int AgentsLeft { get; private set; }

	[SerializeField] private PathwayConnector Spawn = null;

	private void Start()
	{
		Dependency.Create(this);
		Restart();
	}
	private void Restart()
	{
		for (var i = 0; i < Agents.Length; ++i)
		{
			Agents[i] = ObjectActivator.Construct<Agent>();
			Agents[i].AgentDeath += OnAgentDeath;
			Agents[i].transform.position = Spawn.transform.position - Spawn.transform.forward * 2.5f;
			Agents[i].transform.rotation = Spawn.transform.rotation;
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
