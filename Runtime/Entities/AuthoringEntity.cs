﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Unity;
using Depra.Ecs.Worlds;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;

namespace Depra.Ecs.Hybrid.Entities
{
	[DisallowMultipleComponent]
	[AddComponentMenu(MENU_PATH + nameof(AuthoringEntity), DEFAULT_ORDER)]
	public sealed class AuthoringEntity : MonoBehaviour, IAuthoringEntity
	{
		[SerializeField] internal DestructionMode _destructionMode;

		private bool _processed;
		private PackedEntityWithWorld _entity;

		private void OnEnable()
		{
			if (UnityWorlds.Connected == false || _processed)
			{
				return;
			}

			var world = UnityWorlds.Default;
			var entity = world.CreateEntity();
			world.Pool<BakingEntityRef>().Allocate(entity).Value = gameObject;
		}

		public IEnumerable<IAuthoring> Nested => GetComponents<IAuthoring>()
			.Where(x => ReferenceEquals(x, this) == false);

		public bool Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);

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

		IBaker IAuthoring.CreateBaker() => new Backer(_destructionMode);

		private readonly struct Backer : IBaker
		{
			private readonly DestructionMode _destructionMode;

			public Backer(DestructionMode destructionMode) => _destructionMode = destructionMode;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				var entity = world.CreateEntity();
				var authoringEntity = (AuthoringEntity) authoring;
				authoringEntity.Initialize(world.PackEntityWithWorld(entity));

				foreach (var element in authoringEntity.Nested)
				{
					element.CreateBaker().Bake(authoringEntity, world);
					if (_destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Destroy((Component) element);
					}
				}

				authoringEntity.FinalizeConversion();
			}
		}
	}
}