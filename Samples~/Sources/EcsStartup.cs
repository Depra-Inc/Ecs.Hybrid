using System;
using Depra.Ecs.Hybrid.Worlds;
using Depra.Ecs.Components;
using Depra.Ecs.Modular;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Samples
{
	internal sealed class EcsStartup : MonoBehaviour
	{
		private World _world;
		private IWorldSystems _systems;

		private void Start()
		{
			var modules = new EcsModules(
				new RuntimeSceneBaking(),
				new ReviewModule());

			_world = new World(modules.BuildRegistry());
			_systems = new WorldSystems(_world)
				.AddModules(modules)
				.Initialize();
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

			IWorldRegistry[] IEcsModule.Registries => new IWorldRegistry[] { new Registry(), };

			void IEcsModule.Initialize(IWorldSystems systems) { }

			private sealed class Registry : IWorldRegistry
			{
				void IWorldRegistry.Initialize(World world)
				{
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