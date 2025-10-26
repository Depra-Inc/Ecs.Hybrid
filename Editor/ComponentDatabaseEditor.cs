// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Runtime.InteropServices;
using UnityEditor;

namespace Depra.Ecs.Hybrid.Editor
{
	[CustomEditor(typeof(ComponentDatabase))]
	internal sealed class ComponentDatabaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			DrawSizeLabel(serializedObject.FindProperty("_components"));
		}

		private void DrawSizeLabel(SerializedProperty property)
		{
			var size = 0;
			for (var index = 0; index < property.arraySize; index++)
			{
				var arrayElement = property.GetArrayElementAtIndex(index);
				var component = arrayElement.managedReferenceValue;
				if (component == null)
				{
					continue;
				}

				size += Marshal.SizeOf(component.GetType());
			}

			EditorGUILayout.HelpBox($"Total Size: {size} bytes", MessageType.Info);
		}
	}
}