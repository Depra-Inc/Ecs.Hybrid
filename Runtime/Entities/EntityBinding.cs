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
	public readonly struct EntityBinding : IAuthoringEntity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Bind(World world, Entity entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			if (world.IsEntityNotAlive(entity))
			{
				return;
			}

			var packedEntity = world.PackEntityWithWorld(entity);
			IAuthoringEntity authoring = new EntityBinding(packedEntity, gameObject, destructionMode);
			authoring.CreateBaker().Bake(authoring, world);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Bind(PackedEntityWithWorld entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			if (entity.Unpack(out var world, out _))
			{
				IAuthoringEntity authoring = new EntityBinding(entity, gameObject, destructionMode);
				authoring.CreateBaker().Bake(authoring, world);
			}
		}

		private readonly GameObject _gameObject;
		private readonly PackedEntityWithWorld _entity;
		private readonly DestructionMode _destructionMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityBinding(PackedEntityWithWorld entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			_entity = entity;
			_gameObject = gameObject;
			_destructionMode = destructionMode;
		}

		IBaker IAuthoring.CreateBaker() => new Baker(this);

		bool IAuthoringEntity.Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly EntityBinding _binding;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Baker(EntityBinding binding) => _binding = binding;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (_binding._entity.Unpack(out world, out _))
				{
					return;
				}

				foreach (var nestedAuthoring in _binding._gameObject.GetComponents<IAuthoring>())
				{
					nestedAuthoring.CreateBaker().Bake(_binding, world);
					if (_binding._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Object.Destroy((Component)nestedAuthoring);
					}
				}
			}
		}
	}
}