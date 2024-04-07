// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Components
{
	public abstract class AuthoringComponent<TComponent> : MonoBehaviour, IAuthoring where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		public virtual IBaker CreateBaker() => new Baker(this);

		private readonly struct Baker : IBaker
		{
			private readonly AuthoringComponent<TComponent> _component;

			public Baker(AuthoringComponent<TComponent> component) => _component = component;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (((IAuthoringEntity) authoring).Unpack(out var entity))
				{
					world.Pools.Get<TComponent>().Replace(entity, _component._value);
				}
			}
		}
	}
}