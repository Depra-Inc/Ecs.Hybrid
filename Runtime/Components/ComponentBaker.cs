using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	[RequireComponent(typeof(ConvertibleEntity))]
	public abstract class ComponentBaker : MonoBehaviour
	{
		internal abstract void Bake(int entity, World world);
	}
}