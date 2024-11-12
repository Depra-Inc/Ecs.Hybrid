// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	[AddComponentMenu(MENU_PATH + nameof(AuthoringComponentDatabase), DEFAULT_ORDER)]
	public sealed class AuthoringComponentDatabase : MonoBehaviour, IAuthoring
	{
		[SerializeField] private ComponentDatabase[] _components;

		public IEnumerable<ComponentDatabase> Enumerate() => _components;

		IBaker IAuthoring.CreateBaker() => new Baker(this);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly AuthoringComponentDatabase _database;

			public Baker(AuthoringComponentDatabase database) => _database = database;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
#if ECS_DEBUG
				if (((IAuthoringEntity)authoring).Unpack(out var entity) == false)
				{
					Debug.LogWarning($"Failed to unpack entity from {nameof(GameObject)}.", _database);
					return;
				}
#endif
				foreach (var component in _database._components)
				{
#if ECS_DEBUG
					if (component == null)
					{
						Debug.LogWarning($"{nameof(ComponentDatabase)} is null.", _database);
						continue;
					}
#endif
					component.Setup(world, entity);
				}
			}
		}
	}
}