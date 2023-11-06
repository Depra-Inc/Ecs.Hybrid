using Depra.Ecs.Components;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Worlds
{
	public sealed class SceneBakingRegistry : IWorldRegistry
	{
		void IWorldRegistry.Initialize(World world)
		{
			world.AddRegistry(this);
			world.AddPool(new ComponentPool<BakingEntityRef>());
		}

		void IWorldRegistry.PostInitialize() { }
	}
}