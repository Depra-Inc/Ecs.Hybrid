﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.QoL;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ContinuousEntityBakingSystem : IPreInitializationSystem, IExecutionSystem
	{
		private IEntityQuery _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds)
		{
			var world = worlds.Default;
			_bakingEntities = world.Pool<BakingEntityRef>();
			_entities = new EntityQuery(typeof(BakingEntityRef)).Initialize(world);
		}

		void IExecutionSystem.Execute()
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