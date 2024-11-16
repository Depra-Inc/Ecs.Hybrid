// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Ecs.Hybrid
{
	public interface IAuthoring
	{
		IBaker CreateBaker();
	}
}