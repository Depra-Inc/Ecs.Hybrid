// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Components
{
	[RequireComponent(typeof(AuthoringEntity))]
	public abstract class AuthoringComponent<TComponent> : MonoBehaviour, IAuthoring where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		public virtual IBaker CreateBaker(World world) => new Baker(world, this);

		private readonly struct Baker : IBaker
		{
			private readonly World _world;
			private readonly AuthoringComponent<TComponent> _authoringComponent;

			public Baker(World world, AuthoringComponent<TComponent> authoringComponent)
			{
				_world = world;
				_authoringComponent = authoringComponent;
			}

			void IBaker.Bake(IAuthoring authoring)
			{
				var authoringEntity = (IAuthoringEntity) authoring;
				if (authoringEntity.TryGetEntity(out var entity))
				{
					_world.Pool<TComponent>().Replace(entity, _authoringComponent._value);
				}
			}
		}
	}
}