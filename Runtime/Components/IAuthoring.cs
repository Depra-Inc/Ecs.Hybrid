﻿// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Components
{
	public interface IAuthoring
	{
		IBaker CreateBaker(World world);
	}
}