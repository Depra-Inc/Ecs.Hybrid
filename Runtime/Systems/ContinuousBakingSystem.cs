// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Components;
using Depra.Ecs.Filters;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Baking.Runtime.Systems
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ContinuousBakingSystem : IPreInitializeSystem, IExecuteSystem
	{
		private World _world;
		private EntityFilter _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			_world = systems.World;
			_entities = _world.Filter<BakingEntityRef>().End();
			_bakingEntities = _world.Pool<BakingEntityRef>();
		}

		void IExecuteSystem.Execute(float frameTime)
		{
			foreach (int entity in _entities)
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