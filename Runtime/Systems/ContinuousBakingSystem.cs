// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;
using Unity.IL2CPP.CompilerServices;

namespace Depra.Ecs.Hybrid.Systems
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
			_bakingEntities = world.Pools.Get<BakingEntityRef>();
			_entities = new EntityQuery(typeof(BakingEntityRef)).Initialize(world);
		}

		void IExecutionSystem.Execute(float frameTime) => _entities.ForEach(entity =>
		{
			var bakingObject = _bakingEntities[entity].Value;
			if (bakingObject && bakingObject.TryGetComponent(out IAuthoringEntity authoring))
			{
				authoring.CreateBaker().Bake(authoring, _bakingEntities.World);
			}

			_bakingEntities.World.DeleteEntity(entity);
		});
	}
}