using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Systems;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
	public readonly struct SceneWorldInitializeSystem : IPreInitializeSystem
	{
		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			foreach (var convertible in Object.FindObjectsOfType<ConvertibleEntity>())
			{
				SceneEntity.TryConvert(convertible.gameObject, systems.World);
			}

			SceneWorld.Initialize(systems.World);
		}
	}
}