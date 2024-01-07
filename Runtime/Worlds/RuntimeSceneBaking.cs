// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Ecs.Components;
using Depra.Ecs.Hybrid.Systems;
using Depra.Ecs.Modular;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Hybrid.Worlds
{
	public readonly struct RuntimeSceneBaking : IModule
	{
		IEnumerable<IModule> IModule.Modules => Array.Empty<IModule>();

		IEnumerable<IComponentAspect> IModule.Aspects => new IComponentAspect[]
		{
			new SceneBakingAspect()
		};

		void IModule.Initialize(ISystemGroup systems) => systems
			.Add(new BakingServiceSystem())
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}