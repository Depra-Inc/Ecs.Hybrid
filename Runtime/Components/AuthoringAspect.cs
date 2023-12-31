﻿// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Components
{
	public sealed class AuthoringAspect : MonoBehaviour, IAuthoring
	{
		[SerializeField] private GameObject _scope;

		IBaker IAuthoring.CreateBaker() => new Baker(_scope);

		private readonly struct Baker : IBaker
		{
			private readonly GameObject _scope;

			public Baker(GameObject scope) => _scope = scope;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				var components = _scope.GetComponents<IAuthoring>();
				foreach (var component in components)
				{
					component.CreateBaker().Bake(authoring, world);
					Destroy((Component) component);
				}
			}
		}
	}
}