// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Baking.Runtime.Internal;
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
		private ComponentPool<ConvertibleEntityRef> _convertibles;

		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			_world = systems.World;
			_entities = _world.Filter<ConvertibleEntityRef>().End();
			_convertibles = _world.Pool<ConvertibleEntityRef>();
		}

		void IExecuteSystem.Execute(float frameTime)
		{
			foreach (int entity in _entities)
			{
				ref var convertible = ref _convertibles[entity];
				if (convertible.Value && convertible.Value.TryGetComponent(out AuthoringEntity authoring))
				{
					BakingUtility.Bake(authoring, _convertibles.World);
				}

				_world.DeleteEntity(entity);
			}
		}
	}
}