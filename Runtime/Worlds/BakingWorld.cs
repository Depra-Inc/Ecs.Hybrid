// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Worlds
{
	internal static class BakingWorld
	{
		public static World World
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			private set;
		}

		public static void Initialize(World world) => World = world;

		public static void Dispose() => World = null;
	}
}