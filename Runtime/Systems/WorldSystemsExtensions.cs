// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Systems
{
	public static class WorldSystemsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WorldSystems ConvertScene(this WorldSystems self) => self
			.Add(new BakingServiceSystem())
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}