// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Ecs.Hybrid.Entities
{
	internal enum DestructionMode
	{
		[InspectorName("None")]
		NONE,

		[InspectorName("Destroy Object")]
		DESTROY_OBJECT,

		[InspectorName("Destroy Component")]
		DESTROY_COMPONENT,
	}
}