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

			foreach (var authoringComponent in authoringEntity.GetComponents<AuthoringComponent>())
			{
				authoringComponent.CreateBaker(packedEntity).Bake(authoringComponent);
				Object.Destroy(authoringComponent);
			}

			authoringEntity.MarkAsProcessed();
			FinalizeConversion(authoringEntity.gameObject, authoringEntity, packedEntity);
		}

		private static void FinalizeConversion(Object root, AuthoringEntity authoringEntity,
			PackedEntityWithWorld entity)
		{
			switch (authoringEntity._mode)
			{
				case ConversionMode.CONVERT_AND_DESTROY:
					Object.Destroy(root);
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