using System;
using Depra.Ecs.Baking.Systems;
using Depra.Ecs.Modular;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Baking.Worlds
{
	public readonly struct BakingModule : IEcsModule
	{
		IEcsModule[] IEcsModule.Modules => Array.Empty<IEcsModule>();

		IWorldRegistry[] IEcsModule.Registries => new IWorldRegistry[]
		{
			new BackingWorldRegistry()
		};

		void IEcsModule.Initialize(WorldSystems systems) => systems
			.Add(new BakingServiceSystem())
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}