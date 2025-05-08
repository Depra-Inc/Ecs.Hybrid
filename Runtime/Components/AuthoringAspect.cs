// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid.Internal;
using UnityEngine;
using static Depra.Ecs.Hybrid.RuntimeSceneBakeModule;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	[AddComponentMenu(MENU_PATH + nameof(AuthoringAspect), DEFAULT_ORDER)]
	public sealed class AuthoringAspect : MonoBehaviour, IAuthoring
	{
		[Tooltip("GameObjects with IAuthoring components to be baked.")]
		[SerializeField] private GameObject _scope;

		[UnityEngine.SerializeReference]
		[ComponentSerializeReference(nameof(Ecs))]
		private object[] _components;

		[Tooltip("What to do with the scope after baking.\n" +
		         "None - do nothing,\n" +
		         "Destroy Object - destroy this component and the scope,\n" +
		         "Destroy Component - destroy this components and all IAuthoring components on the scope.")]
		[SerializeField]
		private DestructionMode _destructionMode;

		public IEnumerable<IAuthoring> Scoped => _scope
			? _scope.GetComponents<IAuthoring>()
			: Array.Empty<IAuthoring>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FinalizeConversion()
		{
			switch (_destructionMode)
			{
				case DestructionMode.NONE:
					break;
				case DestructionMode.DESTROY_OBJECT:
					Destroy(this);
					Destroy(_scope);
					break;
				case DestructionMode.DESTROY_COMPONENT:
					Destroy(this);
					break;
				default:
					Debug.LogException(new ArgumentOutOfRangeException());
					break;
			}
		}

		IBaker IAuthoring.CreateBaker() => new Baker(this);

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly AuthoringAspect _aspect;

			public Baker(AuthoringAspect aspect) => _aspect = aspect;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				foreach (var component in _aspect.Scoped)
				{
#if ECS_DEBUG
					if (component == null)
					{
						Debug.LogWarning("Component is null.", _aspect);
						continue;
					}
#endif
					component.CreateBaker().Bake(authoring, world);
					if (_aspect._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Destroy((Component)component);
					}
				}

				if (!((IAuthoringEntity)authoring).Unpack(out var entity))
				{
#if ECS_DEBUG
					Debug.LogWarning($"Failed to unpack entity from '{_aspect.name}'", _aspect);
#endif
					return;
				}

				foreach (var component in _aspect._components)
				{
#if ECS_DEBUG
					if (component == null)
					{
						Debug.LogWarning("Component is null", _aspect);
						continue;
					}
#endif
					var componentType = component.GetType();
#if ECS_DEBUG
					if (!world.Pools.Contains(componentType))
					{
						Debug.LogWarning($"Component pool for {componentType} is not found", _aspect);
						continue;
					}
#endif
					world.Pools[componentType].Allocate(entity, component);
				}

				_aspect.FinalizeConversion();
			}
		}
	}
}