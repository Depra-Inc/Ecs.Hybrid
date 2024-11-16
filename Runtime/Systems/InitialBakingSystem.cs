// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class InitialBakingSystem : IPreInitializationSystem
	{
		private static IEnumerable<T> FindOnActiveScene<T>(bool includeInactive = false)
		{
			var activeScene = SceneManager.GetActiveScene();
			if (activeScene.IsValid())
			{
				return SceneManager
					.GetActiveScene()
					.GetRootGameObjects()
					.SelectMany(gameObject => gameObject.GetComponentsInChildren<T>(includeInactive))
					.Reverse();
			}
#if ECS_DEBUG
			Debug.LogWarning("No valid active scene found.");
#endif
			return Enumerable.Empty<T>();
		}

		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds)
		{
			var entities = FindOnActiveScene<IAuthoringEntity>();
			foreach (var authoringEntity in entities)
			{
				authoringEntity.CreateBaker().Bake(authoringEntity, worlds.Default);
			}
		}
	}
}