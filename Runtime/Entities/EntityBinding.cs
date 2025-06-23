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
			public Baker(EntityBinding binding) => _binding = binding;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Bake()
			{
				if (_binding._entity.Unpack(out var world, out _))
				{
					return;
				}

				foreach (var nested in _binding._gameObject.GetComponents<IAuthoring>())
				{
					nested.CreateBaker().Bake(_binding, world);
					if (_binding._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Object.Destroy((Component)nested);
					}
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world) => Bake();
		}
	}
}