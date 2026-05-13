// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Ecs.Hybrid.Editor
{
	[CustomEditor(typeof(EntityRecipe))]
	internal sealed class EntityRecipeEditor : UnityEditor.Editor
	{
		private ReorderableList _componentBundles;
		private SerializedProperty _componentBundlesProperty;
		private ReorderableList _componentSources;
		private SerializedProperty _componentSourcesProperty;

		private void OnEnable()
		{
			_componentBundlesProperty = serializedObject.FindProperty("_componentBundles");
			_componentBundles = new ReorderableList(serializedObject, _componentBundlesProperty, true, true, true, true)
			{
				drawHeaderCallback = DrawBundlesHeader,
				drawElementCallback = DrawBundleElement,
				onAddCallback = OnBundleAdded,
				onRemoveCallback = OnBundleRemoved,
				elementHeight = EditorGUIUtility.singleLineHeight + 2
			};

			_componentSourcesProperty = serializedObject.FindProperty("_componentSources");
			_componentSources = new ReorderableList(serializedObject, _componentSourcesProperty, true, true, true, true)
			{
				drawHeaderCallback = DrawSourcesHeader,
				drawElementCallback = DrawSourceElement,
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawControlButtons((EntityRecipe)target);
			EditorGUILayout.Space(5);
			_componentBundles.DoLayoutList();
			_componentSources.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawControlButtons(EntityRecipe recipe)
		{
			var bundles = GetBundles();
			var externalCount = bundles.Count(set => !IsNestedInAsset(set, recipe));
			var nestedCount = bundles.Count - externalCount;

			EditorGUILayout.LabelField($"Nested: {nestedCount} | External: {externalCount}", EditorStyles.miniLabel);
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginDisabledGroup(externalCount == 0);
			if (GUILayout.Button("Make All Nested"))
			{
				MakeAllNested(recipe, bundles);
			}

			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(nestedCount == 0);
			if (GUILayout.Button("Extract All"))
			{
				ExtractAllNested(recipe, bundles);
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSourceElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = _componentSourcesProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(rect, element, new GUIContent($"Element {index + 1}"), true);
		}

		private void DrawBundleElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = _componentBundlesProperty.GetArrayElementAtIndex(index);
			var componentBundle = element.objectReferenceValue as ComponentDatabase;
			var recipe = (EntityRecipe)target;

			rect.y += 2;
			rect.height = EditorGUIUtility.singleLineHeight;

			var statusRect = new Rect(rect.x, rect.y, 12, rect.height);
			var isNested = componentBundle && IsNestedInAsset(componentBundle, recipe);
			var statusLabel = isNested ? "N" : "E";
			var statusColor = isNested ? new Color(0.3f, 0.7f, 0.3f) : new Color(0.5f, 0.5f, 0.5f);

			var oldColor = GUI.color;
			GUI.color = statusColor;
			var statusContent = new GUIContent(statusLabel, isNested ? "Nested" : "External");
			EditorGUI.LabelField(statusRect, statusContent, EditorStyles.miniLabel);
			GUI.color = oldColor;

			if (isNested && componentBundle)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup(true);
				var nestedDBRect = new Rect(rect.x + 15, rect.y, rect.width - 270, rect.height);
				EditorGUI.ObjectField(nestedDBRect, componentBundle, typeof(ComponentDatabase), false);
				EditorGUI.EndDisabledGroup();

				var labelRect = new Rect(rect.width - 210, rect.y, 200, rect.height);
				var newName = EditorGUI.TextField(labelRect, componentBundle.name);
				if (newName != componentBundle.name && !string.IsNullOrWhiteSpace(newName))
				{
					Undo.RecordObject(componentBundle, "Rename Nested Asset");
					componentBundle.name = newName;
					EditorUtility.SetDirty(componentBundle);
					AssetDatabase.SaveAssets();
				}

				EditorGUILayout.EndHorizontal();
			}
			else
			{
				var fieldRect = new Rect(rect.x + 15, rect.y, rect.width - 65, rect.height);
				var newValue = EditorGUI.ObjectField(fieldRect, componentBundle, typeof(ComponentDatabase), false) as ComponentDatabase;
				if (newValue != componentBundle)
				{
					element.objectReferenceValue = newValue;
				}
			}

			if (componentBundle)
			{
				var actionRect = new Rect(rect.x + rect.width - 45, rect.y, 45, rect.height);
				var pendingOriginalPath = isNested ? GetPendingDeletionPath(componentBundle) : null;
				var hasPendingOriginal = !string.IsNullOrEmpty(pendingOriginalPath);

				if (!isNested)
				{
					if (GUI.Button(actionRect, "Nest"))
					{
						MakeNested(recipe, index);
					}
				}
				else if (hasPendingOriginal)
				{
					var originalColor = GUI.backgroundColor;
					GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
					if (GUI.Button(actionRect, "Del"))
					{
						DeleteOriginal(componentBundle, pendingOriginalPath);
					}

					GUI.backgroundColor = originalColor;
				}
				else
				{
					if (GUI.Button(actionRect, "Out"))
					{
						ExtractNested(recipe, index);
					}
				}
			}
		}

		private void OnBundleAdded(ReorderableList list)
		{
			list.serializedProperty.arraySize++;
			var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
			newElement.objectReferenceValue = null;
		}

		private void OnBundleRemoved(ReorderableList list)
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			var componentBundle = element.objectReferenceValue as ComponentDatabase;
			var recipe = (EntityRecipe)target;

			if (componentBundle && IsNestedInAsset(componentBundle, recipe))
			{
				Undo.DestroyObjectImmediate(componentBundle);
			}

			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			serializedObject.ApplyModifiedProperties();
		}

		private List<ComponentDatabase> GetBundles()
		{
			var bundles = new List<ComponentDatabase>();
			for (var index = 0; index < _componentBundlesProperty.arraySize; index++)
			{
				var element = _componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue as ComponentDatabase;
				if (element)
				{
					bundles.Add(element);
				}
			}

			return bundles;
		}

		private static bool IsNestedInAsset(Object obj, Object parent)
		{
			if (obj == null || parent == null)
			{
				return false;
			}

			var objPath = AssetDatabase.GetAssetPath(obj);
			var parentPath = AssetDatabase.GetAssetPath(parent);
			return objPath == parentPath && AssetDatabase.IsSubAsset(obj);
		}

		private void MakeAllNested(EntityRecipe recipe, List<ComponentDatabase> bundles)
		{
			Undo.RecordObject(recipe, "Make All Nested");
			var convertedCount = 0;

			for (var index = 0; index < bundles.Count; index++)
			{
				var componentBundle = bundles[index];
				if (!componentBundle || IsNestedInAsset(componentBundle, recipe))
				{
					continue;
				}

				var newNested = CreateNestedCopy(recipe, componentBundle);
				_componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue = newNested;
				SavePendingDeletion(newNested, componentBundle);
				convertedCount++;
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
			Debug.Log($"Created {convertedCount} nested copies in {recipe.name}");
		}

		private void MakeNested(EntityRecipe recipe, int index)
		{
			var original = _componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue as ComponentDatabase;
			if (!original || IsNestedInAsset(original, recipe))
			{
				return;
			}

			Undo.RecordObject(recipe, $"Make Nested: {original.name}");
			var newNested = CreateNestedCopy(recipe, original);
			_componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue = newNested;
			SavePendingDeletion(newNested, original);
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
		}

		private static void DeleteOriginal(ComponentDatabase nested, string originalPath)
		{
			if (nested == null || string.IsNullOrEmpty(originalPath))
			{
				return;
			}

			if (!File.Exists(originalPath))
			{
				ClearPendingDeletion(nested);
				Debug.LogWarning($"Original file no longer exists: {originalPath}");
				return;
			}

			if (EditorUtility.DisplayDialog(
				    "Delete Original File",
				    $"Delete original file?\n\n{originalPath}\n\nThis cannot be undone!",
				    "Delete",
				    "Cancel"))
			{
				if (AssetDatabase.DeleteAsset(originalPath))
				{
					ClearPendingDeletion(nested);
					AssetDatabase.SaveAssets();
					Debug.Log($"Deleted original file: {originalPath}");
				}
				else
				{
					Debug.LogError($"Failed to delete: {originalPath}");
				}
			}
		}

		private static ComponentDatabase CreateNestedCopy(EntityRecipe recipe, ComponentDatabase original)
		{
			var copy = Instantiate(original);
			copy.name = original.name;
			AssetDatabase.AddObjectToAsset(copy, recipe);

			return copy;
		}

		private void ExtractAllNested(EntityRecipe recipe, List<ComponentDatabase> bundles)
		{
			var recipePath = AssetDatabase.GetAssetPath(recipe);
			var directory = Path.GetDirectoryName(recipePath);
			Undo.RecordObject(recipe, "Extract All Nested");
			var extractedCount = 0;

			for (var index = 0; index < bundles.Count; index++)
			{
				var componentBundle = bundles[index];
				if (!componentBundle || !IsNestedInAsset(componentBundle, recipe))
				{
					continue;
				}

				var extracted = ExtractToNewAsset(componentBundle, directory);
				_componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue = extracted;
				extractedCount++;
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
			Debug.Log($"Extracted {extractedCount} nested objects from {recipe.name}");
		}

		private void ExtractNested(EntityRecipe recipe, int index)
		{
			var nested = _componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue as ComponentDatabase;
			if (!nested || !IsNestedInAsset(nested, recipe))
			{
				return;
			}

			var recipePath = AssetDatabase.GetAssetPath(recipe);
			var directory = Path.GetDirectoryName(recipePath);
			Undo.RecordObject(recipe, $"Extract Nested: {nested.name}");

			var extracted = ExtractToNewAsset(nested, directory);
			_componentBundlesProperty.GetArrayElementAtIndex(index).objectReferenceValue = extracted;

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
		}

		private static ComponentDatabase ExtractToNewAsset(ComponentDatabase nested, string targetDirectory)
		{
			var copy = Instantiate(nested);
			copy.name = nested.name;
			var assetPath = AssetDatabase.GenerateUniqueAssetPath($"{targetDirectory}/{nested.name}.asset");
			AssetDatabase.CreateAsset(copy, assetPath);
			Undo.DestroyObjectImmediate(nested);

			return copy;
		}

		private static void DrawBundlesHeader(Rect rect) => EditorGUI.LabelField(rect, "Component Bundles");
		private static void DrawSourcesHeader(Rect rect) => EditorGUI.LabelField(rect, "Component Sources");

		private static string GetSessionStateKey(string nestedGuid) => $"EntityRecipe_PendingDeletion_{nestedGuid}";

		private static void SavePendingDeletion(ComponentDatabase nested, ComponentDatabase original)
		{
			if (!nested || !original)
			{
				return;
			}

			var nestedPath = AssetDatabase.GetAssetPath(nested);
			var nestedGuid = AssetDatabase.AssetPathToGUID(nestedPath);
			var originalPath = AssetDatabase.GetAssetPath(original);
			SessionState.SetString(GetSessionStateKey(nestedGuid), originalPath);
		}

		private static string GetPendingDeletionPath(ComponentDatabase nested)
		{
			if (nested == null)
			{
				return null;
			}

			var nestedPath = AssetDatabase.GetAssetPath(nested);
			var nestedGuid = AssetDatabase.AssetPathToGUID(nestedPath);
			return SessionState.GetString(GetSessionStateKey(nestedGuid), null);
		}

		private static void ClearPendingDeletion(ComponentDatabase nested)
		{
			if (nested == null)
			{
				return;
			}

			var nestedPath = AssetDatabase.GetAssetPath(nested);
			var nestedGuid = AssetDatabase.AssetPathToGUID(nestedPath);
			SessionState.EraseString(GetSessionStateKey(nestedGuid));
		}
	}
}