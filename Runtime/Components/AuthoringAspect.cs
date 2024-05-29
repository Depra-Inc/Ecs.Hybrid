// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Ecs.Worlds;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;

namespace Depra.Ecs.Hybrid.Components
{
	[AddComponentMenu(MENU_PATH + nameof(AuthoringAspect), DEFAULT_ORDER)]
	public sealed class AuthoringAspect : MonoBehaviour, IAuthoring
	{
		[SerializeField] private GameObject _scope;

		public IEnumerable<IAuthoring> Scoped => _scope
			? _scope.GetComponents<IAuthoring>()
			: Array.Empty<IAuthoring>();

		IBaker IAuthoring.CreateBaker() => new Baker(this);

		private readonly struct Baker : IBaker
		{
			private readonly AuthoringAspect _aspect;

			public Baker(AuthoringAspect aspect) => _aspect = aspect;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				foreach (var component in _aspect.Scoped)
				{
					component.CreateBaker().Bake(authoring, world);
					Destroy((Component) component);
				}
			}
		}
	}
}