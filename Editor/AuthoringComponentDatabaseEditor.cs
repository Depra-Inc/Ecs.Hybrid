// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Editor.Migration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Editor
{
	[CustomEditor(typeof(AuthoringComponentDatabase))]
	internal sealed class AuthoringComponentDatabaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			var component = (AuthoringComponentDatabase)target;
			var go = component.gameObject;
			var isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(go);
			var isPrefabMode = PrefabStageUtility.GetCurrentPrefabStage();
			var canMigrate = isPrefabAsset || isPrefabMode;
			using (new EditorGUI.DisabledScope(!canMigrate))
			{
				if (GUILayout.Button("Migrate"))
				{
					TryMigrateToRecipe(component);
				}
			}
		}

		private static void TryMigrateToRecipe(AuthoringComponentDatabase component)
		{
			var stage = PrefabStageUtility.GetCurrentPrefabStage();
			var prefabPath = stage ? stage.assetPath : AssetDatabase.GetAssetPath(component.gameObject);

			if (string.IsNullOrEmpty(prefabPath))
			{
				Debug.LogWarning("Cannot resolve prefab path for migration.");
				return;
			}

			if (AuthoringComponentDatabaseMigration.MigratePrefab(prefabPath))
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				Debug.Log($"✓ Migrated: {prefabPath}");
			}
		}
	}
}