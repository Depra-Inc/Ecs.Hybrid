using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Depra.Ecs.Hybrid.Systems
{
	internal static class InterfaceService
	{
		public static IEnumerable<T> FindOnActiveScene<T>(bool includeInactive = false) => SceneManager
			.GetActiveScene()
			.GetRootGameObjects()
			.SelectMany(gameObject => gameObject.GetComponentsInChildren<T>(includeInactive));

		public static IEnumerable<T> FindOnAllScenes<T>(bool includeInactive = false) =>
			Enumerable.Range(0, SceneManager.sceneCount).SelectMany(sceneIndex =>
				SceneManager.GetSceneAt(sceneIndex)
					.GetRootGameObjects()
					.SelectMany(gameObject => gameObject.GetComponentsInChildren<T>(includeInactive)));

		public static IEnumerable<T> FindGeneral<T>(bool includeInactive = false) =>
			Object.FindObjectsOfType<MonoBehaviour>(includeInactive).OfType<T>();
	}
}