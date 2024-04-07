// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Worlds;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Systems
{
	public readonly struct BakingServiceSystem : IPreInitializationSystem, ITerminationSystem
	{
		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds) => BakingWorld.Initialize(worlds.Default);

		void ITerminationSystem.Terminate(IWorldGroup worlds) => BakingWorld.Dispose();
	}
}