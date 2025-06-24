// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.QoL;
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
	public static class AuthoringEntityUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Bake(World world, Entity entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			Bake(world.PackEntityWithWorld(entity), gameObject, destructionMode);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Bake(PackedEntityWithWorld entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			if (entity.Unpack(out var world, out _))
			{
				IAuthoringEntity authoring = new ManualBaker(entity, gameObject, destructionMode);
				authoring.CreateBaker().Bake(authoring, world);
			}
		}

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct ManualBaker : IAuthoringEntity, IBaker
		{
			private readonly GameObject _gameObject;
			private readonly PackedEntityWithWorld _entity;
			private readonly DestructionMode _destructionMode;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ManualBaker(PackedEntityWithWorld entity, GameObject gameObject,
				DestructionMode destructionMode = DestructionMode.NONE)
			{
				_entity = entity;
				_gameObject = gameObject;
				_destructionMode = destructionMode;
			}

			IBaker IAuthoring.CreateBaker() => this;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (_entity.Unpack(out world, out _))
				{
					return;
				}

				foreach (var nestedAuthoring in _gameObject.GetComponents<IAuthoring>())
				{
					nestedAuthoring.CreateBaker().Bake(this, world);
					if (_destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Object.Destroy((Component)nestedAuthoring);
					}
				}
			}

			bool IAuthoringEntity.Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);
		}
	}
}