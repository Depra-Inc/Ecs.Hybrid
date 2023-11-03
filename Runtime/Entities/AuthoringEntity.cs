// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.Hybrid.Worlds;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Entities
{
	[DisallowMultipleComponent]
	public sealed class AuthoringEntity : MonoBehaviour, IAuthoringEntity
	{
		[SerializeField] internal ConversionMode _mode;

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

		public IAuthoring[] Components => GetComponents<IAuthoring>();

		public bool TryGetEntity(out Entity entity) => _entity.Unpack(out _, out entity);

		internal void Initialize(PackedEntityWithWorld entity) => _entity = entity;

		internal void MarkAsProcessed() => _processed = true;
	}
}