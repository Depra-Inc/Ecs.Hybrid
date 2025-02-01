// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Ecs.Hybrid.Internal
{
	internal static class SceneUtility
	{
		public static IEnumerable<T> FindOnActiveScene<T>(bool includeInactive = false)
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
	}
}