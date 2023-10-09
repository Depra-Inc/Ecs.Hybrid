// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Entities;
using Depra.Ecs.Systems;
using UnityEngine;

namespace Depra.Ecs.Baking.Systems
{
	public readonly struct InitialBakingSystem : IPreInitializeSystem
	{
		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			foreach (var authoringEntity in Object.FindObjectsOfType<AuthoringEntity>())
			{
				new AuthoringEntityBaker(authoringEntity).Bake(systems.World);
			}
		}
	}
}