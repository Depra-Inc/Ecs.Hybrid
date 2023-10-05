// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Internal;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Runtime.Systems
{
	public readonly struct BakingInitializationSystem : IPreInitializeSystem
	{
		void IPreInitializeSystem.PreInitialize(IWorldSystems systems) => BakingWorld.Initialize(systems.World);
	}
}