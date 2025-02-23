// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Unity
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public struct UnityEventSender
	{
		public string Name;
		public GameObject Object;

		public UnityEventSender(string name, GameObject @object)
		{
			Name = name;
			Object = @object;
		}

		public override string ToString() => Name;
	}
}