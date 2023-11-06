// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Entities;
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
		private IEntityIterator _entities;
		private ComponentPool<BakingEntityRef> _bakingEntities;

		void IPreInitializationSystem.PreInitialize(World world)
		{
			_entities = new EntityIterator(typeof(BakingEntityRef))
				.Initialize(world);

			_bakingEntities = world.Pool<BakingEntityRef>();
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