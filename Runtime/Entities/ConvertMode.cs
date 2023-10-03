// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Entities
{
	internal enum ConvertMode
	{
		[InspectorName("Convert and inject into world")]
		CONVERT_AND_INJECT,

		[InspectorName("Convert and destroy")]
		CONVERT_AND_DESTROY,

		[InspectorName("Convert and save")]
		CONVERT_AND_SAVE
	}
}