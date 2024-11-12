// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Ecs.Entities;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.Worlds;
using Depra.SerializeReference.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
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
	[CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class ComponentDatabase : ScriptableObject
	{
		[UnityEngine.SerializeReference]
		[ComponentSerializeReference(nameof(Ecs))]
		private object[] _components;

		[HideInInspector]
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		[FormerlySerializedAs("_components")]
		private IComponent[] _oldComponents;

		private const string FILE_NAME = nameof(ComponentDatabase);
		private const string MENU_NAME = MENU_PATH + FILE_NAME;

		public IEnumerable<object> Components => _components;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Setup(World world, Entity entity)
		{
			foreach (var component in _components)
			{
#if ECS_DEBUG
				if (component == null)
				{
					Debug.LogWarning("Component is null", this);
					continue;
				}
#endif
				var componentType = component.GetType();
#if ECS_DEBUG
				if (!world.Pools.Contains(componentType))
				{
					Debug.LogWarning($"Component pool for {componentType} is not found", this);
					continue;
				}
#endif
				world.Pools[componentType].Allocate(entity, component);
			}
		}

		private void OnValidate()
		{
			if (_oldComponents is not { Length: > 0 })
			{
				return;
			}

			var result = _components.ToList();
			result.AddRange(_oldComponents);
			_components = result.ToArray();
			_oldComponents = Array.Empty<IComponent>();
		}
	}
}