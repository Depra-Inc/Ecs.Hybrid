//<auto-generated/>
using Depra.Ecs.Hybrid.Components;
using Depra.Ecs.Hybrid.Entities;
using Depra.Ecs.QoL.Entities;
using Depra.Ecs.QoL.Worlds;
using Depra.Ecs.Worlds;
using UnityEngine;

namespace Depra.Ecs.Hybrid.Samples
{
	internal sealed class DeadTagAuthoringComponent : MonoBehaviour, IAuthoring
	{
		IBaker IAuthoring.CreateBaker() => new Baker();

		private readonly struct Baker : IBaker
		{
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (((IAuthoringEntity) authoring).TryGetEntity(out var entity))
				{
					world.Pool<DeadTag>().Allocate(entity);
				}
			}
		}
	}
}