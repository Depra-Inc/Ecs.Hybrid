// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using UnityEngine;
using static Depra.Ecs.Hybrid.RuntimeSceneBakeModule;
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

		private const string FILE_NAME = nameof(ComponentDatabase);
		private const string MENU_NAME = MENU_PATH + FILE_NAME;

		public IReadOnlyList<object> Components => _components;

		public void Apply(World world, in Entity entity)
		{
			for (int index = 0, count = _components.Length; index < count; index++)
			{
				var component = _components[index];
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
				var pool = world.Pools[componentType];
				if (pool.Contains(entity))
				{
					pool.Set(entity, component);
				}
				else
				{
					world.Pools[componentType].Allocate(entity, component);
				}
			}
		}

		[Obsolete("Use Apply(World, Entity) instead")]
		public void Setup(World world, Entity entity) => Apply(world, entity);

		[Obsolete("Use Apply(World, Entity) instead")]
		public void Modify(World world, Entity entity) => Apply(world, entity);
	}
}