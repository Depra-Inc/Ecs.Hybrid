// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
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
	[AddComponentMenu(MENU_PATH + "Authoring Entity Recipe", DEFAULT_ORDER)]
	public sealed class AuthoringEntityRecipe : MonoBehaviour, IAuthoring
	{
		[SerializeField] private EntityRecipe _recipe;

		public EntityRecipe Recipe
		{
			get => _recipe;
			set => _recipe = value;
		}

		IBaker IAuthoring.CreateBaker() => new Baker(_recipe);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly EntityRecipe _recipe;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Baker(EntityRecipe recipe) => _recipe = recipe;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (!((IAuthoringEntity)authoring).Unpack(out var entity))
				{
#if ECS_DEBUG
					Debug.LogWarning($"Failed to unpack entity by recipe '{_recipe.name}'", _recipe);
#endif
					return;
				}

				var batches = _recipe.ComponentBundles;
				for (int index = 0, count = batches.Count; index < count; index++)
				{
					batches[index].Apply(entity, world);
				}

				var authorings = _recipe.ComponentSources;
				for (int index = 0, count = authorings.Count; index < count; index++)
				{
					authorings[index].CreateBaker().Bake(authoring, world);
				}
			}
		}
	}
}