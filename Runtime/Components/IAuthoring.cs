// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.QoL.Entities;

namespace Depra.Ecs.Hybrid.Components
{
	public interface IAuthoring
	{
		IBaker CreateBaker(PackedEntityWithWorld entity);
	}
}