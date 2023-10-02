using System;
using Depra.Ecs.Baking.Runtime.Components;
using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	internal static class SceneEntity
	{
		public static void TryConvert(GameObject root, World world)
		{
			if (root.TryGetComponent(out ConvertibleEntity convertible))
			{
				Convert(convertible, world);
			}
		}

		private static void Convert(ConvertibleEntity convertible, World world)
		{
			var entity = world.CreateEntity();
			var packedEntity = new PackedEntityWithWorld(entity, world, world.GetEntityGeneration(entity));

			foreach (var backer in convertible.GetComponents<ComponentBaker>())
			{
				backer.Bake(entity, world);
				Object.Destroy(backer);
			}

			convertible.MarkAsProcessed();
			FinalizeConversion(convertible, convertible, packedEntity);
		}

		private static void FinalizeConversion(Object @object, ConvertibleEntity convertible, PackedEntityWithWorld entity)
		{
			switch (convertible._mode)
			{
				case ConvertMode.CONVERT_AND_DESTROY:
					Object.Destroy(@object);
					break;
				case ConvertMode.CONVERT_AND_INJECT:
					Object.Destroy(convertible);
					break;
				case ConvertMode.CONVERT_AND_SAVE:
					convertible.Initialize(entity);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}