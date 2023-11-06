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

		public virtual IBaker CreateBaker() => new Baker(this);

		private readonly struct Baker : IBaker
		{
			private readonly AuthoringComponent<TComponent> _authoringComponent;

			public Baker(AuthoringComponent<TComponent> authoringComponent)
			{
				_authoringComponent = authoringComponent;
			}

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				var authoringEntity = (IAuthoringEntity) authoring;
				if (authoringEntity.TryGetEntity(out var entity))
				{
					world.Pool<TComponent>().Replace(entity, _authoringComponent._value);
				}
			}
		}
	}
}