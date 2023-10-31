// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Worlds
{
	internal static class BakingWorld
	{
		public static World World;

		public static void Initialize(World world) => World = world;

		public static void Dispose() => World = null;
	}
}