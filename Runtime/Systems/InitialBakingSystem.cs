// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Systems
{
	public readonly struct InitialBakingSystem : IPreInitializationSystem
	{
		void IPreInitializationSystem.PreInitialize(World world)
		{
			var authoringEntities = Object.FindObjectsOfType<AuthoringEntity>();
			foreach (var authoringEntity in authoringEntities)
			{
				new AuthoringEntityBaker(authoringEntity).Bake(world);
			}
		}
	}
}