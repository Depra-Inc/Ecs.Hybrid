// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid;
using Depra.Ecs.QoL;
using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Unity
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	[DefaultExecutionOrder(100)]
	public abstract class UnityEventSource : AuthoringBehaviour
	{
		[SerializeField] private string _senderName;

		public UnityEventSender Sender => new(_senderName, gameObject);
		protected World World { get; private set; }

		protected override void Bake(World world) => World = world;

		protected virtual bool Validate() => World != null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected virtual PackedEntityWithWorld CreateEntity() => World.PackEntityWithWorld(World.CreateEntity());
	}
}