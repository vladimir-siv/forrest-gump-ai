using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject AgentPrototype = null;
	public GameObject[] AgentModels = null;

	private Agent Player = null;

	private void Start()
	{
		Dependency.Create(this);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (Player == null)
			{
				Player = ObjectActivator.Construct<Agent>();
			}
			else
			{
				ObjectActivator.Destruct(Player);
				Player = null;
			}
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			Player?.Run();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			Player?.Die();
		}
	}
}
