using System;
using Depra.Ecs.Baking.Worlds;
using Depra.Ecs.Components;
using Depra.Ecs.Modular;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Baking.Samples
{
	internal sealed class EcsStartup : MonoBehaviour
	{
		private World _world;
		private IWorldSystems _systems;

		private void Start()
		{
			var modules = new EcsModules(
				new BakingModule(),
				new ReviewModule());

			_world = new World(modules.BuildRegistry());
			var systems = new WorldSystems(_world);
			modules.Initialize(systems);
			_systems = systems.Initialize();
		}

		private void Update() => _systems?.Execute(Time.deltaTime);

		private void OnDestroy()
		{
			_systems?.Destroy();
			_world?.Destroy();
		}

		private sealed class ReviewModule : IEcsModule
		{
			IEcsModule[] IEcsModule.Modules => Array.Empty<IEcsModule>();
			IWorldRegistry[] IEcsModule.Registries => new IWorldRegistry[] { new Registry() };

			void IEcsModule.Initialize(WorldSystems systems) { }

			private sealed class Registry : IWorldRegistry
			{
				void IWorldRegistry.Initialize(World world)
				{
					IWorldRegistry backingWorldRegistry = new BackingWorldRegistry();
					backingWorldRegistry.Initialize(world);
					world.AddRegistry(backingWorldRegistry);
					world.AddRegistry(this);

					world.AddPool(new ComponentPool<Health>());
					world.AddPool(new ComponentPool<Damage>());
					world.AddPool(new ComponentPool<DeadTag>());
				}

				void IWorldRegistry.PostInitialize() { }
			}
		}
	}
}