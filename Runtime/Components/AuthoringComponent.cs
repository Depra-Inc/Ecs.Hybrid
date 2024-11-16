// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Ecs.QoL;
using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public abstract class AuthoringComponent<TComponent> : MonoBehaviour, IAuthoring where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		public virtual IBaker CreateBaker() => new Baker(this, typeof(TComponent));

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		private readonly struct Baker : IBaker
		{
			private readonly Type _componentType;
			private readonly AuthoringComponent<TComponent> _component;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Baker(AuthoringComponent<TComponent> component, Type componentType)
			{
				_component = component;
				_componentType = componentType;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (!((IAuthoringEntity)authoring).Unpack(out var entity))
				{
#if ECS_DEBUG
					Debug.LogWarning($"Failed to unpack entity from '{_component.name}'", _component);
#endif
					return;
				}
#if ECS_DEBUG
				if (!world.Pools.Contains(_componentType))
				{
					Debug.LogWarning("Component is not registered in the world", _component);
					return;
				}
#endif
				world.Pools[_componentType].Replace(entity, _component._value);
			}
		}
	}
}