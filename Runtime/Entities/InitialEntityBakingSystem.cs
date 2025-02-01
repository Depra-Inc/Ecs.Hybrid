// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Internal;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class InitialEntityBakingSystem : IPreInitializationSystem
	{
		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds)
		{
			foreach (var authoringEntity in SceneUtility.FindOnActiveScene<IAuthoringEntity>())
			{
				authoringEntity.CreateBaker().Bake(authoringEntity, worlds.Default);
			}
		}
	}
}