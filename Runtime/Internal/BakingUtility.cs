// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Ecs.Baking.Runtime.Components;
using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Entities;
using Depra.Ecs.Worlds;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Depra.Ecs.Baking.Runtime.Internal
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	internal static class BakingUtility
	{
		public static void Bake(AuthoringEntity authoringEntity, World world)
		{
			var entity = world.CreateEntity();
			var packedEntity = world.PackEntityWithWorld(entity);

			foreach (var authoring in authoringEntity.GetComponents<IAuthoring>())
			{
				authoring.CreateBaker(packedEntity).Bake(authoring);
				Object.Destroy((Component) authoring);
			}

			authoringEntity.MarkAsProcessed();
			FinalizeConversion(authoringEntity, packedEntity);
		}

		private static void FinalizeConversion(AuthoringEntity authoringEntity, PackedEntityWithWorld entity)
		{
			switch (authoringEntity._mode)
			{
				case ConversionMode.CONVERT_AND_DESTROY:
					Object.Destroy(authoringEntity.gameObject);
					break;
				case ConversionMode.CONVERT_AND_INJECT:
					Object.Destroy(authoringEntity);
					break;
				case ConversionMode.CONVERT_AND_SAVE:
					authoringEntity.Initialize(entity);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}