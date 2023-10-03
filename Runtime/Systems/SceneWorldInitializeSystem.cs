// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Baking.Runtime.Internal;
using Depra.Ecs.Systems;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Systems
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