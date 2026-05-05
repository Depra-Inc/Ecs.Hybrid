// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.SerializeReference.Extensions;
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
	public sealed class EntityRecipe : ScriptableObject
	{
		[SerializeField] private List<ComponentDatabase> _componentBundles = new();

		[SerializeReferenceDropdown]
		[UnityEngine.SerializeReference]
		private List<IAuthoring> _componentSources;

		private const string FILE_NAME = "Entity Recipe";
		private const string MENU_NAME = MENU_PATH + FILE_NAME;

		internal List<IAuthoring> ComponentSources
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _componentSources;
		}

		internal List<ComponentDatabase> ComponentBundles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _componentBundles;
		}

		internal void Add(ComponentDatabase bundle)
		{
			if (bundle && !_componentBundles.Contains(bundle))
			{
				_componentBundles.Add(bundle);
			}
		}

		internal void Remove(ComponentDatabase database) => _componentBundles.Remove(database);
	}
}