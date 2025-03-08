// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.QoL;
using Depra.Ecs.Unity;
using UnityEngine;
using static Depra.Ecs.Hybrid.RuntimeSceneBakeModule;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
	[DisallowMultipleComponent]
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	[AddComponentMenu(MENU_PATH + nameof(AuthoringEntity), DEFAULT_ORDER)]
	public sealed class AuthoringEntity : MonoBehaviour, IAuthoringEntity
	{
		[SerializeField] internal DestructionMode _destructionMode;

		private bool _processed;
		private PackedEntityWithWorld _entity;

		private void OnEnable()
		{
			if (!UnityWorlds.Connected || _processed)
			{
				return;
			}

			var world = UnityWorlds.Default;
			var entity = world.CreateEntity();
			world.Pool<BakingEntityRef>().Allocate(entity).Value = gameObject;
		}

		public IAuthoringAccess GetNested() => new AuthoringNestedAccess(this);

		public bool Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);

		IBaker IAuthoring.CreateBaker() => new Backer(_destructionMode);

		private void Initialize(PackedEntityWithWorld entity)
		{
			_entity = entity;
			_processed = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FinalizeConversion()
		{
			switch (_destructionMode)
			{
				case DestructionMode.NONE:
					break;
				case DestructionMode.DESTROY_OBJECT:
					Destroy(gameObject);
					break;
				case DestructionMode.DESTROY_COMPONENT:
					Destroy(this);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Backer : IBaker
		{
			private readonly DestructionMode _destructionMode;

			public Backer(DestructionMode destructionMode) => _destructionMode = destructionMode;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				var authoringEntity = (AuthoringEntity)authoring;
				if (authoringEntity._processed)
				{
					return;
				}

				var entity = world.CreateEntity();
				authoringEntity.Initialize(world.PackEntityWithWorld(entity));

				using var nested = authoringEntity.GetNested();
				foreach (var element in nested.Enumerate())
				{
					element.CreateBaker().Bake(authoringEntity, world);
					if (_destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Destroy((Component)element);
					}
				}

				authoringEntity.FinalizeConversion();
			}
		}
	}
}