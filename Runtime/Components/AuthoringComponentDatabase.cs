﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;

namespace Depra.Ecs.Hybrid.Components
{
	[AddComponentMenu(MENU_PATH + nameof(AuthoringComponentDatabase), DEFAULT_ORDER)]
	public sealed class AuthoringComponentDatabase : MonoBehaviour, IAuthoring
	{
		[SerializeField] private ComponentDatabase[] _components;

		public IEnumerable<ComponentDatabase> Enumerate() => _components;

		IBaker IAuthoring.CreateBaker() => new Baker(_components);

		private readonly struct Baker : IBaker
		{
			private readonly ComponentDatabase[] _components;

			public Baker(ComponentDatabase[] components) => _components = components;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (((IAuthoringEntity) authoring).Unpack(out var entity) == false)
				{
					return;
				}

				foreach (var component in _components)
				{
					component.Setup(world, entity);
				}
			}
		}
	}
}