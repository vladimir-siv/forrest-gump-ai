using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainGenerator
{
	private Queue<IPathway> pathways = new Queue<IPathway>();

	public TerrainGenerator() { }

	public PathwayConnector Begin() => Begin(Vector3.zero, 0f);
	public PathwayConnector Begin(Vector3 spawnPoint, float spawnRotation)
	{
		foreach (var pathway in pathways) pathway.Destruct();
		pathways.Clear();

		var spawn = ObjectActivator.Construct<PathwayConnector>();
		spawn.transform.position = spawnPoint;
		spawn.transform.rotation = Quaternion.Euler(0f, spawnRotation, 0f);
		pathways.Enqueue(spawn);

		return spawn;
	}
}
