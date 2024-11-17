// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Modules;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class RuntimeSceneBakeModule : IModule
	{
		IComponentAspect[] IModule.Aspects => new IComponentAspect[]
		{
			new SceneBakingAspect()
		};

		void IModule.Initialize(ISystemGroup systems) => systems
			.Add(new InitialBakingSystem())
			.Add(new ContinuousBakingSystem());
	}
}