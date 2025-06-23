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
	public sealed class AuthoringEntityWrapper : IAuthoringEntity
	{
		private readonly PackedEntity _entity;
		private readonly GameObject _gameObject;
		private readonly DestructionMode _destructionMode;

		private World _world;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AuthoringEntityWrapper(PackedEntity entity, GameObject gameObject,
			DestructionMode destructionMode = DestructionMode.NONE)
		{
			_world = null;
			_entity = entity;
			_gameObject = gameObject;
			_destructionMode = destructionMode;
		}

		IBaker IAuthoring.CreateBaker() => new Baker(this);

		bool IAuthoringEntity.Unpack(out World world, out Entity entity)
		{
			world = _world;
			return _entity.Unpack(_world, out entity);
		}

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
				if (_wrapper._entity.IsNull())
				{
					return;
				}

				_wrapper._world = world;
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