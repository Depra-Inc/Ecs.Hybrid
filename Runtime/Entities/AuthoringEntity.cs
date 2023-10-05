// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Internal;
using Depra.Ecs.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Entities
{
	[DisallowMultipleComponent]
	public sealed class AuthoringEntity : MonoBehaviour
	{
		[SerializeField] internal ConversionMode _mode;

		private bool _processed;
		private World _spawnWorld;
		private PackedEntityWithWorld _entity;

		private void OnEnable()
		{
			var world = BakingWorld.World;
			if (world == null || _processed)
			{
				return;
			}

			var entity = world.CreateEntity();
			ref var convertibleEntity = ref world.Pool<ConvertibleEntityRef>().Allocate(entity);
			convertibleEntity.Value = gameObject;
		}

		public int? TryGetEntity() => _entity.Unpack(out _, out var entity) ? entity : null;

		internal void Initialize(PackedEntityWithWorld entity) => _entity = entity;

		internal void MarkAsProcessed() => _processed = true;
	}
}