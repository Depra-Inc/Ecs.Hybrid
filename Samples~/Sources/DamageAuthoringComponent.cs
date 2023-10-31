using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Samples
{
	internal sealed class DamageAuthoringComponent : MonoBehaviour, IAuthoring
	{
		[SerializeField] private System.Single _amount;
		[SerializeField] private System.Int32 _source;

		IBaker IAuthoring.CreateBaker(PackedEntityWithWorld entity) => new Baker(entity);

		private readonly struct Baker : IBaker
		{
			private readonly PackedEntityWithWorld _entity;

			public Baker(PackedEntityWithWorld entity) => _entity = entity;

			void IBaker.Bake(IAuthoring authoring)
			{
				if (_entity.Unpack(out var world, out var entity))
				{
					ref var component = ref world.Pool<Damage>().Allocate(entity);
					component.Amount = ((DamageAuthoringComponent) authoring)._amount;
					component.Source = ((DamageAuthoringComponent) authoring)._source;
				}
			}
		}
	}
}