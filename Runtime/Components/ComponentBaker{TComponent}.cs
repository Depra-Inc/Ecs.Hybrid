using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	public abstract class ComponentBaker<TComponent> : ComponentBaker where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		internal override void Bake(int entity, World world)
		{
			var pool = world.Pool<TComponent>();
			if (pool.Has(entity))
			{
				pool.Delete(entity);
			}

			pool.Allocate(entity) = _value;
		}
	}
}