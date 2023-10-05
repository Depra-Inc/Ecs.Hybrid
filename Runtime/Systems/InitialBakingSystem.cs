using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Baking.Runtime.Internal;
using Depra.Ecs.Systems;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Systems
{
	public readonly struct InitialBakingSystem : IPreInitializeSystem
	{
		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			foreach (var authoringEntity in Object.FindObjectsOfType<AuthoringEntity>())
			{
				BakingUtility.Bake(authoringEntity, systems.World);
			}
		}
	}
}