// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Entities;
using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Baking.Systems
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ContinuousBakingSystem : IPreInitializeSystem, IExecuteSystem
	{
		private World _world;
		private EntityGroup _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			_world = systems.World;
			_entities = _world.Group<BakingEntityRef>().End();
			_bakingEntities = _world.Pool<BakingEntityRef>();
		}

		void IExecuteSystem.Execute(float frameTime)
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