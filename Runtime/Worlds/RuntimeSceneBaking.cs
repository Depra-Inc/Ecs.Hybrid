// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Ecs.Hybrid.Systems;
using Depra.Ecs.Modular;
using Depra.Ecs.Systems;
using Depra.Ecs.Worlds;

namespace Depra.Ecs.Hybrid.Worlds
{
	public readonly struct RuntimeSceneBaking : IEcsModule
	{
		IEcsModule[] IEcsModule.Modules => Array.Empty<IEcsModule>();

		IWorldRegistry[] IEcsModule.Registries => new IWorldRegistry[] { new BackingWorldRegistry() };

		void IEcsModule.Initialize(IWorldSystems systems) => systems
			.Add(new BakingServiceSystem())
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}