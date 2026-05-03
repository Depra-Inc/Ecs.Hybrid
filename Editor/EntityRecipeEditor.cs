// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Editor
{
	[CustomEditor(typeof(EntityRecipe))]
	internal sealed class EntityRecipeEditor : UnityEditor.Editor
	{
		private SerializedProperty _setsProperty;
		private ReorderableList _reorderableList;

		private void OnEnable()
		{
			_setsProperty = serializedObject.FindProperty("_sets");
			_reorderableList = new ReorderableList(serializedObject, _setsProperty, true, true, true, true)
			{
				drawHeaderCallback = DrawHeader,
				drawElementCallback = DrawElement,
				onAddCallback = OnAdd,
				onRemoveCallback = OnRemove,
				elementHeight = EditorGUIUtility.singleLineHeight + 2
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			var recipe = (EntityRecipe)target;
			DrawControlButtons(recipe);
			EditorGUILayout.Space(5);
			_reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawControlButtons(EntityRecipe recipe)
		{
			var sets = GetSets();
			var externalCount = sets.Count(set => !IsNestedInAsset(set, recipe));
			var nestedCount = sets.Count - externalCount;

			EditorGUILayout.LabelField($"Nested: {nestedCount} | External: {externalCount}", EditorStyles.miniLabel);
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginDisabledGroup(externalCount == 0);
			if (GUILayout.Button("Make All Nested"))
			{
				MakeAllNested(recipe, sets);
			}

			EditorGUI.EndDisabledGroup();

			EditorGUI.BeginDisabledGroup(nestedCount == 0);
			if (GUILayout.Button("Extract All"))
			{
				ExtractAllNested(recipe, sets);
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawHeader(Rect rect) => EditorGUI.LabelField(rect, "Sets");

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = _setsProperty.GetArrayElementAtIndex(index);
			var componentDB = element.objectReferenceValue as ComponentDatabase;
			var recipe = (EntityRecipe)target;

			rect.y += 2;
			rect.height = EditorGUIUtility.singleLineHeight;

			var statusRect = new Rect(rect.x, rect.y, 12, rect.height);
			var isNested = componentDB && IsNestedInAsset(componentDB, recipe);
			var statusLabel = isNested ? "N" : "E";
			var statusColor = isNested ? new Color(0.3f, 0.7f, 0.3f) : new Color(0.5f, 0.5f, 0.5f);

			var oldColor = GUI.color;
			GUI.color = statusColor;
			var statusContent = new GUIContent(statusLabel, isNested ? "Nested" : "External");
			EditorGUI.LabelField(statusRect, statusContent, EditorStyles.miniLabel);
			GUI.color = oldColor;

			if (isNested && componentDB)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup(true);
				var nestedDBRect = new Rect(rect.x + 15, rect.y, rect.width - 270, rect.height);
				EditorGUI.ObjectField(nestedDBRect, componentDB, typeof(ComponentDatabase), false);
				EditorGUI.EndDisabledGroup();

				var labelRect = new Rect(rect.width - 210, rect.y, 200, rect.height);
				var newName = EditorGUI.TextField(labelRect, componentDB.name);
				if (newName != componentDB.name && !string.IsNullOrWhiteSpace(newName))
				{
					Undo.RecordObject(componentDB, "Rename Nested Asset");
					componentDB.name = newName;
					EditorUtility.SetDirty(componentDB);
					AssetDatabase.SaveAssets();
				}

				EditorGUILayout.EndHorizontal();
			}
			else
			{
				var fieldRect = new Rect(rect.x + 15, rect.y, rect.width - 65, rect.height);
				var newValue = EditorGUI.ObjectField(fieldRect, componentDB, typeof(ComponentDatabase), false) as ComponentDatabase;
				if (newValue != componentDB)
				{
					element.objectReferenceValue = newValue;
				}
			}

			if (componentDB)
			{
				var actionRect = new Rect(rect.x + rect.width - 45, rect.y, 45, rect.height);
				if (!isNested)
				{
					if (GUI.Button(actionRect, "Nest"))
					{
						MakeNested(recipe, index);
					}
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

		private void OnAdd(ReorderableList list)
		{
			list.serializedProperty.arraySize++;
			var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
			newElement.objectReferenceValue = null;
		}

		private void OnRemove(ReorderableList list)
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
			var componentDB = element.objectReferenceValue as ComponentDatabase;
			var recipe = (EntityRecipe)target;

			Undo.RecordObject(recipe, "Remove Set");

			if (componentDB)
			{
				recipe.Remove(componentDB);
				if (IsNestedInAsset(componentDB, recipe))
				{
					Undo.DestroyObjectImmediate(componentDB);
				}
			}

			ReorderableList.defaultBehaviours.DoRemoveButton(list);
			serializedObject.ApplyModifiedProperties();
		}

		private List<ComponentDatabase> GetSets()
		{
			var sets = new List<ComponentDatabase>();
			for (var i = 0; i < _setsProperty.arraySize; i++)
			{
				var element = _setsProperty.GetArrayElementAtIndex(i).objectReferenceValue as ComponentDatabase;
				if (element)
				{
					sets.Add(element);
				}
			}

			return sets;
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

		private void MakeAllNested(EntityRecipe recipe, List<ComponentDatabase> sets)
		{
			Undo.RecordObject(recipe, "Make All Nested");
			var convertedCount = 0;

			for (var i = 0; i < sets.Count; i++)
			{
				if (!sets[i] || IsNestedInAsset(sets[i], recipe))
				{
					continue;
				}

				var newNested = CreateNestedCopy(recipe, sets[i]);
				_setsProperty.GetArrayElementAtIndex(i).objectReferenceValue = newNested;
				convertedCount++;
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
			Debug.Log($"Created {convertedCount} nested copies in {recipe.name}");
		}

		private void MakeNested(EntityRecipe recipe, int index)
		{
			var original = _setsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ComponentDatabase;
			if (!original || IsNestedInAsset(original, recipe))
			{
				return;
			}

			Undo.RecordObject(recipe, $"Make Nested: {original.name}");
			var newNested = CreateNestedCopy(recipe, original);
			_setsProperty.GetArrayElementAtIndex(index).objectReferenceValue = newNested;

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
		}

		private static ComponentDatabase CreateNestedCopy(EntityRecipe recipe, ComponentDatabase original)
		{
			var copy = Instantiate(original);
			copy.name = original.name;
			AssetDatabase.AddObjectToAsset(copy, recipe);

			return copy;
		}

		private void ExtractAllNested(EntityRecipe recipe, List<ComponentDatabase> sets)
		{
			var recipePath = AssetDatabase.GetAssetPath(recipe);
			var directory = Path.GetDirectoryName(recipePath);
			Undo.RecordObject(recipe, "Extract All Nested");
			var extractedCount = 0;

			for (var i = 0; i < sets.Count; i++)
			{
				if (!sets[i] || !IsNestedInAsset(sets[i], recipe)) continue;

				var extracted = ExtractToNewAsset(sets[i], directory);
				_setsProperty.GetArrayElementAtIndex(i).objectReferenceValue = extracted;
				extractedCount++;
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(recipe);
			AssetDatabase.SaveAssets();
			Debug.Log($"Extracted {extractedCount} nested objects from {recipe.name}");
		}

		private void ExtractNested(EntityRecipe recipe, int index)
		{
			var nested = _setsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ComponentDatabase;
			if (!nested || !IsNestedInAsset(nested, recipe)) return;

			var recipePath = AssetDatabase.GetAssetPath(recipe);
			var directory = Path.GetDirectoryName(recipePath);
			Undo.RecordObject(recipe, $"Extract Nested: {nested.name}");

			var extracted = ExtractToNewAsset(nested, directory);
			_setsProperty.GetArrayElementAtIndex(index).objectReferenceValue = extracted;

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
	}
}