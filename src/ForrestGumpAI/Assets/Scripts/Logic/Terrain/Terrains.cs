public static class Terrains
{
	public static ITerrainGenerator Easy => EasyTerrain.Instance;
	public static ITerrainGenerator Medium => MediumTerrain.Instance;
}
