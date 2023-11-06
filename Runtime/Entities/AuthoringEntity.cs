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
			if (BakingWorld.World != null && _processed == false)
			{
				BakingWorld.World.Pool<BakingEntityRef>().Allocate(BakingWorld.World.CreateEntity()).Value = gameObject;
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

		IBaker IAuthoring.CreateBaker() => new Backer();

		bool IAuthoringEntity.TryGetEntity(out Entity entity) => _entity.Unpack(out _, out entity);

		private readonly struct Backer : IBaker
		{
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				var entity = world.CreateEntity();
				var authoringEntity = (AuthoringEntity) authoring;
				authoringEntity.Initialize(world.PackEntityWithWorld(entity));

				foreach (var element in authoringEntity.Nested)
				{
					element.CreateBaker().Bake(authoringEntity, world);
					Destroy((Component) element);
				}

				authoringEntity.FinalizeConversion();
			}
		}
	}
}