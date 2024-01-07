// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Worlds
{
	public sealed class SceneBakingAspect : IComponentAspect
	{
		void IComponentAspect.Initialize(World world)
		{
			world.AddAspect(this);
			world.AddPool(new ComponentPool<BakingEntityRef>());
		}

		void IComponentAspect.PostInitialize() { }
	}
}