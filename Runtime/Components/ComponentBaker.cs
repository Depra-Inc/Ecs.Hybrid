// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Entities;

namespace Depra.Ecs.Baking.Runtime.Components
{
	public sealed class ComponentBaker : IComponentBaker
	{
		private readonly PackedEntityWithWorld _entity;

		public ComponentBaker(PackedEntityWithWorld entity) => _entity = entity;

		void IComponentBaker.Bake(AuthoringComponent authoring)
		{
			if (_entity.Unpack(out var world, out var entity))
			{
				world.PoolByType(authoring.ComponentType).Replace(entity, authoring.Data);
			}
		}
	}
}