// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Components;
using Depra.Ecs.Entities;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	[RequireComponent(typeof(AuthoringEntity))]
	public abstract class AuthoringComponent<TComponent> : MonoBehaviour, IAuthoring where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		public virtual IBaker CreateBaker(PackedEntityWithWorld entity) => new Baker(entity);

		private readonly struct Baker : IBaker
		{
			private readonly PackedEntityWithWorld _entity;

			public Baker(PackedEntityWithWorld entity) => _entity = entity;

			void IBaker.Bake(IAuthoring authoring)
			{
				if (_entity.Unpack(out var world, out var entity))
				{
					world.Pool<TComponent>().Replace(entity, ((AuthoringComponent<TComponent>) authoring)._value);
				}
			}
		}
	}
}