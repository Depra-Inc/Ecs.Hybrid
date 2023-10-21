using Depra.Ecs.Baking.Entities;
using Depra.Ecs.Components;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Baking.Worlds
{
	public sealed class BackingWorldRegistry : IWorldRegistry
	{
		internal ComponentPool<BakingEntityRef> BakingEntities { get; private set; }

		void IWorldRegistry.Initialize(World world)
		{
			world.AddRegistry(this);
			world.AddPool(BakingEntities = new ComponentPool<BakingEntityRef>());
		}

		void IWorldRegistry.PostInitialize() { }
	}
}