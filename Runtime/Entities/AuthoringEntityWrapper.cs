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
	public readonly struct AuthoringEntityWrapper : IAuthoringEntity
	{
		private readonly GameObject _gameObject;
		private readonly PackedEntityWithWorld _entity;
		private readonly DestructionMode _destructionMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AuthoringEntityWrapper(PackedEntityWithWorld entity, GameObject gameObject,
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
			private readonly AuthoringEntityWrapper _wrapper;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Baker(AuthoringEntityWrapper wrapper) => _wrapper = wrapper;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (_wrapper._entity.Unpack(out world, out _))
				{
					return;
				}

				foreach (var nested in _wrapper._gameObject.GetComponents<IAuthoring>())
				{
					nested.CreateBaker().Bake(_wrapper, world);
					if (_wrapper._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Object.Destroy((Component)authoring);
					}
				}
			}
		}
	}
}