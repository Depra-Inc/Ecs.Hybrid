// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ContinuousBakingSystem : IPreInitializationSystem, IExecutionSystem
	{
		private IEntityQuery _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds)
		{
			var world = worlds.Default;
			_bakingEntities = world.Pool<BakingEntityRef>();
			_entities = new EntityQuery(typeof(BakingEntityRef)).Initialize(world);
		}

		void IExecutionSystem.Execute(float frameTime)
		{
			foreach (var entity in _entities)
			{
				var bakingObject = _bakingEntities[entity].Value;
				if (bakingObject && bakingObject.TryGetComponent(out IAuthoringEntity authoring))
				{
					authoring.CreateBaker().Bake(authoring, _bakingEntities.World);
				}

				_bakingEntities.World.DeleteEntity(entity);
			}
		}
	}
}