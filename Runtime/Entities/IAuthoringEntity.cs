// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;

namespace Depra.Ecs.Hybrid.Entities
{
	public interface IAuthoringEntity : IAuthoring
	{
		bool Unpack(out Entity entity);
	}
}