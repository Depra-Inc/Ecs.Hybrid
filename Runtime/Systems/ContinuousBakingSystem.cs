// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Hybrid.Worlds;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Systems
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ContinuousBakingSystem : IPreInitializationSystem, IExecutionSystem
	{
		private World _world;
		private IEntityIterator _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializationSystem.PreInitialize(World world)
		{
			_world = world;
			_bakingEntities = _world.Registry<BackingWorldRegistry>().BakingEntities;
			_entities = new EntityIterator(typeof(BakingEntityRef)).Initialize(_world);
		}

		void IExecutionSystem.Execute(float frameTime)
		{
			foreach (var entity in _entities)
			{
				ref var bakingEntity = ref _bakingEntities[entity];
				if (bakingEntity.Value && bakingEntity.Value.TryGetComponent(out AuthoringEntity authoring))
				{
					new AuthoringEntityBaker(authoring).Bake(_bakingEntities.World);
				}

				_world.DeleteEntity(entity);
			}
		}
	}
}