﻿using System.Collections.Generic;
using UnityEngine;

public abstract class Pathway : MonoBehaviour, IPathway, IPoolableObject
{
	private bool noEntry = true;
	private HashSet<Agent> agents = new HashSet<Agent>();

	public IPathway Next { get; protected set; } = null;
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

	public void OnEnter(Agent agent)
	{
		if (noEntry)
		{
			noEntry = false;
			Dependency.Controller.GenerateTerrain();
		}

		agents.Add(agent);
	}
	public void OnExit(Agent agent)
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
