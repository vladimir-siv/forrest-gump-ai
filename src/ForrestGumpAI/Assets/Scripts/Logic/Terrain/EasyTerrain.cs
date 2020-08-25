using UnityEngine;

public class EasyTerrain : ITerrainGenerator
{
	public static EasyTerrain Instance = new EasyTerrain();

	private EasyTerrain() { }

	public IPathway Generate()
	{
		var straight = ObjectActivator.Construct<StraightPathway>();
		straight.SetDimension(5f, Random.Range(10f, 20f));
		return straight;
	}
}
