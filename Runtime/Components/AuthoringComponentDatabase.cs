// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<ComponentDatabase> Enumerate() => _components;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IBaker IAuthoring.CreateBaker() => new Baker(this, name);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly string _ownerName;
			private readonly AuthoringComponentDatabase _database;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Baker(AuthoringComponentDatabase database, string ownerName)
			{
				_database = database;
				_ownerName = ownerName;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (((IAuthoringEntity)authoring).Unpack(out var entity) == false)
				{
#if ECS_DEBUG
					Debug.LogWarning($"Failed to unpack entity from '{_ownerName}'", _database);
#endif
					return;
				}

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