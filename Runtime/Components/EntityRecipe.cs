using System.Collections.Generic;
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
	[CreateAssetMenu(menuName = MENU_NAME, fileName = FILE_NAME, order = DEFAULT_ORDER)]
	public sealed class EntityRecipe : ScriptableObject, IAuthoring
	{
		[SerializeField] private List<ComponentDatabase> _sets = new();

		private const string FILE_NAME = nameof(EntityRecipe);
		private const string MENU_NAME = MENU_PATH + FILE_NAME;

		public void Add(ComponentDatabase database)
		{
			if (database && !_sets.Contains(database))
			{
				_sets.Add(database);
			}
		}

		public void Remove(ComponentDatabase database) => _sets.Remove(database);

		public IBaker CreateBaker() => new Baker(this);

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

				var sets = _recipe._sets;
				for (int index = 0, count = sets.Count; index < count; index++)
				{
					sets[index].Modify(world, entity);
				}
			}
		}
	}
}