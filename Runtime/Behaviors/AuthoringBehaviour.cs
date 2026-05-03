// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public abstract class AuthoringBehaviour : MonoBehaviour, IAuthoring
	{
		[field: SerializeField] public string WorldName { get; private set; }

		IBaker IAuthoring.CreateBaker() => new Baker(this);

		protected abstract void Bake(World world);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly AuthoringBehaviour _behaviour;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Baker(AuthoringBehaviour behaviour) => _behaviour = behaviour;

			void IBaker.Bake(IAuthoring authoring, World world) => _behaviour.Bake(world);
		}
	}
}