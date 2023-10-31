// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Entities
{
	[Serializable]
	internal struct BakingEntityRef
	{
		public GameObject Value;
	}
}