using UnityEngine;
using static Depra.Ecs.Hybrid.RuntimeSceneBakeModule;

namespace Depra.Ecs.Hybrid
{
	[AddComponentMenu(MENU_PATH + "Authoring Recipe", DEFAULT_ORDER)]
	internal sealed class AuthoringRecipe : MonoBehaviour, IAuthoring
	{
		[SerializeField] private EntityRecipe _recipe;

		// Only for migration purposes. Will be removed in the future.
		internal EntityRecipe _Recipe
		{
			get => _recipe;
			set => _recipe = value;
		}

		IBaker IAuthoring.CreateBaker() => _recipe?.CreateBaker();
	}
}