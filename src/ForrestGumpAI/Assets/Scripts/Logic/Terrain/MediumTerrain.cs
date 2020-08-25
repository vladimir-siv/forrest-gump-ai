using UnityEngine;

public class MediumTerrain : ITerrainGenerator
{
	public static MediumTerrain Instance = new MediumTerrain();

	private MediumTerrain() { }

	public IPathway Generate()
	{
		switch (Random.Range(0, 2))
		{
			case 0:
				var straight = ObjectActivator.Construct<StraightPathway>();
				straight.SetDimension(5f, Random.Range(10f, 20f));
				return straight;
			case 1:
				var rigged = ObjectActivator.Construct<RiggedPathway>();
				rigged.Depth = Random.Range(5, 10) * 2;
				return rigged;
		}

		return null;
	}
}
