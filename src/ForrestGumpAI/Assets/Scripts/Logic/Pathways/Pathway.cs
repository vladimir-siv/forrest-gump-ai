using System.Collections.Generic;
using UnityEngine;

public abstract class Pathway : MonoBehaviour, IPathway, IPoolableObject
{
	private bool noEntry = true;
	private HashSet<Runner> agents = new HashSet<Runner>();

	public IPathway Next { get; protected set; } = null;
	public abstract float Difficulty { get; }
	public abstract Vector3 ExitPoint { get; }
	
	public virtual void OnConstruct()
	{
		noEntry = true;
		agents.Clear();
		Next = null;
		gameObject.SetActive(true);
	}
	public virtual void OnDestruct()
	{
		noEntry = true;
		agents.Clear();
		Next?.Disconnect();
		Next = null;
		gameObject.SetActive(false);
	}

	public void OnEnter(Runner agent)
	{
		if (noEntry)
		{
			noEntry = false;
			Dependency.Controller.GenerateTerrain();
		}

		agents.Add(agent);
	}
	public void OnExit(Runner agent)
	{
		agents.Remove(agent);
	}

	public abstract void ConnectTo(Vector3 position, float rotation);
	public abstract void ConnectOn(IPathway pathway);
	public abstract void Disconnect();

	public void Destruct()
	{
		foreach (var agent in agents) agent.Die();
		ObjectActivator.Destruct(this);
	}
}
