// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Systems
{
	public static class WorldSystemsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IWorldSystems ConvertScene<TSystems>(this TSystems self) where TSystems : IWorldSystems => self
			.Add(new BakingServiceSystem())
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}