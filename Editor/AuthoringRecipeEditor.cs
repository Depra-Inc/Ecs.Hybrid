// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Editor.Migration;
using UnityEditor;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Editor
{
	[CustomEditor(typeof(AuthoringRecipe))]
	internal sealed class AuthoringRecipeEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if (GUILayout.Button("Migrate"))
			{
				var prefabPath = AssetDatabase.GetAssetPath(target);
				if (AuthoringRecipeMigration.MigratePrefab(prefabPath))
				{
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					Debug.Log($"✓ Migrated: {prefabPath}");
				}
			}
		}
	}
}