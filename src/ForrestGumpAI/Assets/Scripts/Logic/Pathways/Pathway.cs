using UnityEngine;

public abstract class Pathway : MonoBehaviour, IPathway, IPoolableObject
{
	protected int exited = 0;

	public bool WaitingOnAgents => exited == 0 || Dependency.Controller.AgentsLeft > exited;
	public IPathway Next { get; protected set; } = null;
	
	public abstract Vector3 ExitPoint { get; }
	
	public virtual void OnConstruct()
	{
		exited = 0;
		gameObject.SetActive(true);
	}
	public virtual void OnDestruct()
	{
		Next?.Disconnect();
		Next = null;
		gameObject.SetActive(false);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Agent"))
		{
			var agent = other.GetComponent<Agent>();

			if (agent.LastPathway == Next)
			{
				++exited;
				Dependency.Controller.DestructTerrain();
			}
		}
	}

	public abstract void ConnectTo(Vector3 position, float rotation);
	public abstract void ConnectOn(IPathway pathway);
	public abstract void Disconnect();
	
	public void Destruct() => ObjectActivator.Destruct(this);
}
