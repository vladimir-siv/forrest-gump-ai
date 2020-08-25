public static class Dependency
{
	public static GameController Controller { get; private set; }

	private static bool _PoolsInitialized = false;
	private static void InitPools()
	{
		if (_PoolsInitialized) return;
		_PoolsInitialized = true;

		ObjectActivator.CreatePool<Agent>(Controller.AgentPrototype);
		ObjectActivator.CreatePool<Wall>(Controller.WallPrototype);
		ObjectActivator.CreatePool<PathwayConnector>(Controller.PathwayConnectorPrototype);
		ObjectActivator.CreatePool<StraightPathway>(Controller.StraightPathwayPrototype);
		ObjectActivator.CreatePool<RiggedPathway>(Controller.RiggedPathwayPrototype);
	}

	public static void Create(GameController controller)
	{
		Controller = controller;
		InitPools();
	}
}
