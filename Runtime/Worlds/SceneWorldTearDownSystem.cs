using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
	public readonly struct SceneWorldTearDownSystem : ITearDownSystem
	{
		void ITearDownSystem.TearDown(IWorldSystems systems) => SceneWorld.Destroy();
	}
}