// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Runtime.Systems
{
	public static class WorldSystemsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IWorldSystems ConvertScene<TSystems>(this TSystems self) where TSystems : IWorldSystems => self
			.Add(new BakingInitializationSystem())
			.Add(new PreBakingSystem())
			.Add(new ContinuousBakingSystem())
			.Add(new BakingFinalizationSystem());
	}
}