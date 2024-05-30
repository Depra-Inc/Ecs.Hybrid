// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.Worlds;
using UnityEngine;
using static Depra.Ecs.Hybrid.Module;

namespace Depra.Ecs.Hybrid.Components
{
	[AddComponentMenu(MENU_PATH + nameof(AuthoringAspect), DEFAULT_ORDER)]
	public sealed class AuthoringAspect : MonoBehaviour, IAuthoring
	{
		[Tooltip("GameObjects with IAuthoring components to be baked.")]
		[SerializeField] private GameObject _scope;

		[Tooltip("What to do with the scope after baking.\n" +
		         "None - do nothing,\n" +
		         "Destroy Object - destroy this component and the scope,\n" +
		         "Destroy Component - destroy this components and all IAuthoring components on the scope.")]
		[SerializeField] private DestructionMode _destructionMode;

		public IEnumerable<IAuthoring> Scoped => _scope
			? _scope.GetComponents<IAuthoring>()
			: Array.Empty<IAuthoring>();

		IBaker IAuthoring.CreateBaker() => new Baker(this);

		private readonly struct Baker : IBaker
		{
			private readonly AuthoringAspect _aspect;

			public Baker(AuthoringAspect aspect) => _aspect = aspect;

			void IBaker.Bake(IAuthoring authoring, World world)
			{
				foreach (var component in _aspect.Scoped)
				{
					component.CreateBaker().Bake(authoring, world);
					if (_aspect._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Destroy((Component) component);
					}
				}

				FinalizeConversion();
			}

			private void FinalizeConversion()
			{
				switch (_aspect._destructionMode)
				{
					case DestructionMode.NONE:
						break;
					case DestructionMode.DESTROY_OBJECT:
						Destroy(_aspect);
						Destroy(_aspect._scope);
						break;
					case DestructionMode.DESTROY_COMPONENT:
						Destroy(_aspect);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}