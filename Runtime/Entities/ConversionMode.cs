// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Ecs.Hybrid.Entities
{
	internal enum ConversionMode
	{
		[InspectorName("Convert and Inject")]
		CONVERT_AND_INJECT,

		[InspectorName("Convert and Destroy")]
		CONVERT_AND_DESTROY,

		[InspectorName("Convert and Save")]
		CONVERT_AND_SAVE
	}
}