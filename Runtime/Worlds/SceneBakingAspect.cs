// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Components;
using Depra.Ecs.Hybrid.Entities;

namespace Depra.Ecs.Hybrid.Worlds
{
	public sealed class SceneBakingAspect : IComponentAspect
	{
		void IComponentAspect.Initialize(AspectGroup aspects, ComponentPoolGroup pools)
		{
			aspects.Add(this);
			pools.Add(new ComponentPool<BakingEntityRef>());
		}
	}
}