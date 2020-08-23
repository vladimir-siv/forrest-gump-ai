using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject AgentPrototype = null;
	public GameObject[] AgentModels = null;

	public Agent[] Agents = null;

	private void Start()
	{
		Dependency.Create(this);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (Agents[0] == null)
			{
				Agents[0] = ObjectActivator.Construct<Agent>();
			}
			else
			{
				ObjectActivator.Destruct(Agents[0]);
				Agents[0] = null;
			}
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			Agents[0]?.Run();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Agents[0]?.Die();
		}
	}
}
