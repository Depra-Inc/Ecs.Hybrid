using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Components
{
	public sealed class AuthoringAspect : MonoBehaviour, IAuthoring
	{
		[SerializeField] private GameObject _scope;

		IBaker IAuthoring.CreateBaker(World world) => new Baker(world, _scope);

		private readonly struct Baker : IBaker
		{
			private readonly World _world;
			private readonly GameObject _scope;

			public Baker(World world, GameObject scope)
			{
				_world = world;
				_scope = scope;
			}

			void IBaker.Bake(IAuthoring authoring)
			{
				foreach (var authoringComponents in _scope.GetComponents<IAuthoring>())
				{
					authoringComponents.CreateBaker(_world).Bake(authoring);
					Destroy((Component) authoringComponents);
				}
			}
		}
	}
}