// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.Hybrid.Worlds;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Entities
{
	[DisallowMultipleComponent]
	public sealed class AuthoringEntity : MonoBehaviour, IAuthoringEntity
	{
		[SerializeField] internal DestructionMode _destructionMode;

		private bool _processed;
		private PackedEntityWithWorld _entity;

		private void OnEnable()
		{
			var world = BakingWorld.World;
			if (world != null && _processed == false)
			{
				world.Pool<BakingEntityRef>().Allocate(world.CreateEntity()).Value = gameObject;
			}
		}

		private IEnumerable<IAuthoring> Nested => GetComponents<IAuthoring>()
			.Where(x => ReferenceEquals(x, this) == false);

		IEnumerable<IAuthoring> IAuthoringEntity.Nested => Nested;

		private void Initialize(PackedEntityWithWorld entity)
		{
			_entity = entity;
			_processed = true;
		}

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

		IBaker IAuthoring.CreateBaker(World world) => new Backer(world);

		bool IAuthoringEntity.TryGetEntity(out Entity entity) => _entity.Unpack(out _, out entity);

		private readonly struct Backer : IBaker
		{
			private readonly World _world;

			public Backer(World world) => _world = world;

			void IBaker.Bake(IAuthoring authoring)
			{
				var entity = _world.CreateEntity();
				var authoringEntity = (AuthoringEntity) authoring;
				authoringEntity.Initialize(_world.PackEntityWithWorld(entity));

				foreach (var element in authoringEntity.Nested)
				{
					element.CreateBaker(_world).Bake(authoringEntity);
					Destroy((Component) element);
				}

				authoringEntity.FinalizeConversion();
			}
		}
	}
}