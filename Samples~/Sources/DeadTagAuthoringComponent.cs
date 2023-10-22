using Depra.Ecs.Baking.Components;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using UnityEngine;

namespace Depra.Ecs.Baking.Samples
{
	internal sealed class DeadTagAuthoringComponent : MonoBehaviour, IAuthoring
	{
		IBaker IAuthoring.CreateBaker(PackedEntityWithWorld entity) => new Baker(entity);

		private readonly struct Baker : IBaker
		{
			private readonly PackedEntityWithWorld _entity;

			public Baker(PackedEntityWithWorld entity) => _entity = entity;

			void IBaker.Bake(IAuthoring authoring)
			{
				if (_entity.Unpack(out var world, out var entity))
				{
					world.Pool<DeadTag>().Allocate(entity);
				}
			}
		}
	}
}