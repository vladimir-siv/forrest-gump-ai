using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator
{
	public static TerrainGenerator Easy   => new TerrainGenerator(EasyTerrain.Instance);
	public static TerrainGenerator Medium => new TerrainGenerator(MediumTerrain.Instance);
	public static TerrainGenerator Hard   => new TerrainGenerator(HardTerrain.Instance);

	private readonly LinkedList<IPathway> pathways = new LinkedList<IPathway>();
	private void Enqueue(IPathway pathway) => pathways.AddLast(pathway);
	private void Dequeue() => pathways.RemoveFirst();
	private IPathway Peek() => pathways.First.Value;
	private IPathway Last() => pathways.Last.Value;

	private readonly ITerrainGenerator generator = null;

	public DateTime LastGenerationTime { get; private set; } = DateTime.Now;

	public TerrainGenerator(ITerrainGenerator generator) => this.generator = generator;

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
		LastGenerationTime = DateTime.Now;

		if (pathways.Count > 8)
		{
			var front = Peek();
			Dequeue();
			front.Destruct();
		}

		IPathway pathway;
		var last = Last();

		if (last is PathwayConnector lastConnector)
		{
			pathway = generator.Generate(lastConnector.Angle);
		}
		else
		{
			var connector = ObjectActivator.Construct<PathwayConnector>();
			connector.Angle = Random.Range(-90f, 90f);
			pathway = connector;
		}

		last.ConnectOn(pathway);
		Enqueue(pathway);
	}
}
