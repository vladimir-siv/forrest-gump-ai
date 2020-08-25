using UnityEngine;

public class MediumTerrain : ITerrainGenerator
{
	public static MediumTerrain Instance = new MediumTerrain();

	private MediumTerrain() { }

	public IPathway Generate(float connectorAngle)
	{
		switch (Random.Range(0, 3))
		{
			case 0:
				var straight = ObjectActivator.Construct<StraightPathway>();
				straight.SetDimension(5f, Random.Range(10f, 20f));
				return straight;
			case 1:
				var rigged = ObjectActivator.Construct<RiggedPathway>();
				rigged.Depth = Random.Range(5, 10) * 2;
				return rigged;
			case 2:
				var spread = ObjectActivator.Construct<SpreadPathway>();
				spread.Scale = Random.Range(10f, 20f);
				if (Mathf.Abs(connectorAngle) >= 15f) spread.Opened = SpreadPathway.Gate.North;
				else spread.Opened = (SpreadPathway.Gate)(int)Mathf.Pow(2, Random.Range(0, 3));
				return spread;
		}

		return null;
	}
}
