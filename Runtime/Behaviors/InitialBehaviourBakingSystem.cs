// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Hybrid.Internal;
using Unity.IL2CPP.CompilerServices;

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class InitialBehaviourBakingSystem : IPreInitializationSystem
	{
		void IPreInitializationSystem.PreInitialize(IWorldGroup worlds)
		{
			foreach (IAuthoring authoringBehaviour in SceneUtility.FindOnActiveScene<AuthoringBehaviour>())
			{
				authoringBehaviour.CreateBaker().Bake(authoringBehaviour, worlds.Default);
			}
		}
	}
}