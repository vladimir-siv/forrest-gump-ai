using UnityEngine;

public class HardTerrain : ITerrainGenerator
{
	public static HardTerrain Instance = new HardTerrain();

	private HardTerrain() { }

	public IPathway Generate(float connectorAngle)
	{
		switch (Random.Range(0, 3))
		{
			case 0:
				var straight = ObjectActivator.Construct<StraightPathway>();
				straight.Depth = Random.Range(5f, 20f);
				return straight;
			case 1:
				var rigged = ObjectActivator.Construct<RiggedPathway>();
				rigged.Depth = Random.Range(2, 10) * 2;
				return rigged;
			case 2:
				var spread = ObjectActivator.Construct<SpreadPathway>();
				spread.Scale = Random.Range(15f, 40f);
				spread.MiddleWalls = Random.Range(0f, 100f) < 50f;
				if (connectorAngle >= 25f) spread.Opened = (SpreadPathway.Gate)(int)Mathf.Pow(2, Random.Range(0, 2));
				else if (connectorAngle <= -25f) spread.Opened = (SpreadPathway.Gate)(int)Mathf.Pow(2, Random.Range(1, 3));
				else spread.Opened = (SpreadPathway.Gate)(int)Mathf.Pow(2, Random.Range(0, 3));
				return spread;
		}

		return null;
	}
}
