using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
	private readonly LinkedList<IPathway> pathways = new LinkedList<IPathway>();
	private void Enqueue(IPathway pathway) => pathways.AddLast(pathway);
	private void Dequeue() => pathways.RemoveFirst();
	private IPathway Peek() => pathways.First.Value;
	private IPathway Last() => pathways.Last.Value;

	public TerrainGenerator() { }

	public PathwayConnector Begin() => Begin(Vector3.zero, 0f);
	public PathwayConnector Begin(Vector3 spawnPoint, float spawnRotation)
	{
		foreach (var pathway in pathways) pathway.Destruct();
		pathways.Clear();

		var spawn = ObjectActivator.Construct<PathwayConnector>();
		spawn.Angle = 0f;
		spawn.transform.position = spawnPoint;
		spawn.transform.rotation = Quaternion.Euler(0f, spawnRotation, 0f);
		Enqueue(spawn);

		return spawn;
	}

	public void Generate()
	{
		IPathway pathway;
		var last = Last();

		if (last is PathwayConnector)
		{
			var straight = ObjectActivator.Construct<StraightPathway>();
			straight.SetDimension(5f, 10f/*Random.Range(10f, 20f)*/);
			pathway = straight;
		}
		else
		{
			var connector = ObjectActivator.Construct<PathwayConnector>();
			connector.Angle = 30f; //Random.Range(-90f, 90f);
			pathway = connector;
		}

		last.ConnectOn(pathway);
		Enqueue(pathway);
	}
	public bool Remove()
	{
		if (pathways.Count == 0) return false;
		var front = Peek();
		if (front.AgentsInside != 0) return false;
		Dequeue();
		front.Destruct();
		return true;
	}
}
