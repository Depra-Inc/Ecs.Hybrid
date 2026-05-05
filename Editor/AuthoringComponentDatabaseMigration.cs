// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Ecs.Hybrid.Editor.Migration
{
	/// <summary>
	/// TEMPORARY: Migration utility for AuthoringComponentDatabase -> AuthoringRecipe
	/// TODO: Remove this file after migration is complete.
	/// </summary>
	internal static class AuthoringComponentDatabaseMigration
	{
		private const string FILE_POSFIX = " Recipe.asset";
		private const string MIGRATION_KEY = "AuthoringComponentDatabase_Migration_v1_Completed";

		[MenuItem("Ecs/Migration/Authoring Component Database/1. Preview Changes")]
		public static void PreviewMigration()
		{
			var targets = FindMigrationTargets().ToArray();
			if (targets.Length == 0)
			{
				EditorUtility.DisplayDialog("Migration Preview",
					"No prefabs found with AuthoringComponentDatabase.", "OK");
				return;
			}

			var message = $"Found {targets.Length} prefab(s) to migrate:\n\n";
			foreach (var path in targets)
			{
				message += $"• {path}\n";
			}

			Debug.Log("[MIGRATION PREVIEW]\n" + message);
			EditorUtility.DisplayDialog("Migration Preview", message, "OK");
		}

		[MenuItem("Ecs/Migration/Authoring Component Database/2. Run Migration")]
		public static void RunMigration()
		{
			if (EditorPrefs.GetBool(MIGRATION_KEY, false))
			{
				if (!EditorUtility.DisplayDialog("Migration Warning",
					    "Migration was already completed before. Run again?",
					    "Yes, Run Again", "Cancel"))
				{
					return;
				}
			}

			var targets = FindMigrationTargets().ToArray();
			if (targets.Length == 0)
			{
				EditorUtility.DisplayDialog("Migration", "No prefabs to migrate.", "OK");
				return;
			}

			if (!EditorUtility.DisplayDialog("Confirm Migration",
				    $"This will modify {targets.Length} prefab(s).\n\n" +
				    "Make sure you have a backup or are using version control!\n\n" +
				    "Continue?",
				    "Yes, Migrate", "Cancel"))
			{
				return;
			}

			var results = PerformMigration(targets);

			EditorPrefs.SetBool(MIGRATION_KEY, true);
			EditorUtility.DisplayDialog("Migration Complete",
				$"Successfully migrated: {results.successCount}\n" +
				$"Failed: {results.failCount}\n\n" +
				"Check Console for details.",
				"OK");
		}

		[MenuItem("Ecs/Migration/Authoring Component Database/3. Reset Migration Flag")]
		public static void ResetMigrationFlag()
		{
			EditorPrefs.DeleteKey(MIGRATION_KEY);
			Debug.Log("Migration flag reset.");
		}

		public static bool MigratePrefab(string path)
		{
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			if (!prefab)
			{
				return false;
			}

			var entity = prefab.GetComponent<AuthoringEntity>();
			if (!entity)
			{
				return false;
			}

			var databases = prefab.GetComponentsInChildren<AuthoringComponentDatabase>();
			if (databases.Length == 0)
			{
				return false;
			}

			var recipe = prefab.GetComponent<AuthoringEntityRecipe>() ?? prefab.AddComponent<AuthoringEntityRecipe>();
			if (!recipe.Recipe)
			{
				var recipePath = Path.Combine(Path.GetDirectoryName(path)!,
					Path.GetFileNameWithoutExtension(path) + FILE_POSFIX);
				var newRecipe = ScriptableObject.CreateInstance<EntityRecipe>();
				AssetDatabase.CreateAsset(newRecipe, recipePath);
				recipe.Recipe = newRecipe;
			}

			foreach (var database in databases)
			{
				foreach (var set in database.Enumerate())
				{
					recipe.Recipe.Add(set);
				}

				Object.DestroyImmediate(database, true);
			}

			EditorUtility.SetDirty(prefab);
			EditorUtility.SetDirty(recipe.Recipe);
			return true;
		}

		private static IEnumerable<string> FindMigrationTargets() =>
			from guid in AssetDatabase.FindAssets("t:Prefab")
			select AssetDatabase.GUIDToAssetPath(guid)
			into assetPath
			let prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)
			where prefab != null && prefab.GetComponent<AuthoringEntity>() != null &&
			      prefab.GetComponentInChildren<AuthoringComponentDatabase>() != null
			select assetPath;

		private static (int successCount, int failCount) PerformMigration(string[] paths)
		{
			var successCount = 0;
			var failCount = 0;

			for (var index = 0; index < paths.Length; index++)
			{
				var path = paths[index];
				EditorUtility.DisplayProgressBar("Migrating Authoring Component Databases",
					$"Processing {index + 1}/{paths.Length}: {path}",
					(float)index / paths.Length);

				try
				{
					if (MigratePrefab(path))
					{
						successCount++;
						Debug.Log($"✓ Migrated: {path}");
					}
					else
					{
						failCount++;
						Debug.LogWarning($"✗ Failed: {path}");
					}
				}
				catch (Exception e)
				{
					failCount++;
					Debug.LogError($"✗ Error migrating {path}: {e.Message}");
				}
			}

			EditorUtility.ClearProgressBar();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return (successCount, failCount);
		}
	}
}