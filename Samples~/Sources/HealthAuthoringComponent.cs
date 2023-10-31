using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.QoL.Components;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Samples
{
	public sealed class HealthAuthoringComponent : MonoBehaviour, IAuthoring
	{
		[SerializeField] private float _value;

		public IBaker CreateBaker(PackedEntityWithWorld entity) => new Baker(_value, entity);

		private readonly struct Baker : IBaker
		{
			private readonly float _value;
			private readonly PackedEntityWithWorld _entity;

			public Baker(float value, PackedEntityWithWorld entity)
			{
				_value = value;
				_entity = entity;
			}

			void IBaker.Bake(IAuthoring authoring)
			{
				if (_entity.Unpack(out var world, out var entity))
				{
					world.Pool<Health>().Replace(entity, new Health { Value = _value });
				}
			}
		}
	}
}