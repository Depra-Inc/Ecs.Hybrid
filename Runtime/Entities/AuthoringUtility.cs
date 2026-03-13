// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.QoL;
using UnityEngine;
using Object = UnityEngine.Object;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public static class AuthoringUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BakeEntity(World world, Entity entity,
			GameObject gameObject, DestructionMode destructionMode = DestructionMode.NONE)
		{
			BakeEntity(new ManualBaker(world.PackEntityWithWorld(entity), gameObject, destructionMode), world);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void BakeEntity(PackedEntityWithWorld entity,
			GameObject gameObject, DestructionMode destructionMode = DestructionMode.NONE)
		{
			if (entity.Unpack(out var world, out _))
			{
				BakeEntity(new ManualBaker(entity, gameObject, destructionMode), world);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity BakeEntity(IAuthoringEntity authoringEntity, World world)
		{
			authoringEntity.CreateBaker().Bake(authoringEntity, world);
			return authoringEntity.Unpack(out _, out var entity) ? entity : Entity.NULL;
		}

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		internal readonly struct ManualBaker : IAuthoringEntity, IBaker
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
				if (!_entity.Unpack(out world, out var entity))
				{
					Debug.LogError($"Failed to unpack entity '{entity}'!", _gameObject);
					return;
				}

				var authoringComponents = _gameObject.GetComponents<IAuthoring>();
				if (authoringComponents.Length == 0)
				{
					Debug.LogWarning($"No authoring components found on '{_gameObject.name}'", _gameObject);
					return;
				}

				foreach (var authoringComponent in authoringComponents)
				{
					authoringComponent.CreateBaker().Bake(this, world);
					if (_destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Object.Destroy((Component)authoringComponent);
					}
				}
			}

			bool IAuthoringEntity.Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);
		}
	}

	[Obsolete("Use AuthoringUtility instead.")]
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
				IAuthoringEntity authoring = new AuthoringUtility.ManualBaker(entity, gameObject, destructionMode);
				authoring.CreateBaker().Bake(authoring, world);
			}
		}
	}
}