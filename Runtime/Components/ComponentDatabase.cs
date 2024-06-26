﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Ecs.Entities;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.Worlds;
using Depra.SerializeReference.Extensions;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;

namespace Depra.Ecs.Hybrid.Components
{
	[CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class ComponentDatabase : ScriptableObject
	{
		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private IComponent[] _components;

		private const string FILE_NAME = nameof(ComponentDatabase);
		private const string MENU_NAME = MENU_PATH + FILE_NAME;

		public IEnumerable<IComponent> Components => _components;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Setup(World world, Entity entity)
		{
			foreach (var component in _components)
			{
				world.Pools[component.GetType()].Allocate(entity, component);
			}
		}
	}
}