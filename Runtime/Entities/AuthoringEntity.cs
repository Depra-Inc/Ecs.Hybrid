// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Depra.Ecs.Hybrid.Internal;
using Depra.Ecs.QoL;
using Depra.Ecs.Unity;
using UnityEngine;
using static Depra.Ecs.Hybrid.RuntimeSceneBakeModule;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
	[DisallowMultipleComponent]
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	[AddComponentMenu(MENU_PATH + "Authoring Entity", DEFAULT_ORDER)]
	public sealed class AuthoringEntity : MonoBehaviour, IAuthoringEntity
	{
		[SerializeField] internal DestructionMode _destructionMode;

		private bool _processed;
		private PackedEntityWithWorld _entity = PackedEntityWithWorld.NULL;

		private void OnEnable()
		{
			if (!UnityWorlds.Connected || _processed)
			{
				return;
			}

			var world = UnityWorlds.Default;
			var entity = world.CreateEntity();
			world.Pool<BakingEntityRef>().Allocate(entity).Value = gameObject;
		}

		public NestedAccess GetNested() => new(this);

		public bool Unpack(out World world, out Entity entity) => _entity.Unpack(out world, out entity);

		IBaker IAuthoring.CreateBaker() => new Backer(this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FinalizeConversion()
		{
			_processed = true;
			switch (_destructionMode)
			{
				case DestructionMode.NONE:
					break;
				case DestructionMode.DESTROY_OBJECT:
					Destroy(gameObject);
					break;
				case DestructionMode.DESTROY_COMPONENT:
					Destroy(this);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		internal readonly struct Backer : IBaker
		{
			public static void Bake(AuthoringEntity component, World world, Entity entity)
			{
				if (component._processed)
				{
					return;
				}

				component._entity = world.PackEntityWithWorld(entity);
				using var access = component.GetNested();
				foreach (var element in access.Enumerate())
				{
					element.CreateBaker().Bake(component, world);
					if (component._destructionMode == DestructionMode.DESTROY_COMPONENT)
					{
						Destroy((Component)element);
					}
				}

				component.FinalizeConversion();
			}

			private readonly AuthoringEntity _component;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Backer(AuthoringEntity component) => _component = component;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IBaker.Bake(IAuthoring authoring, World world)
			{
				if (!_component._processed)
				{
					Bake(_component, world, world.CreateEntity());
				}
			}
		}

#if ENABLE_IL2CPP
		[Il2CppSetOption(Option.NullChecks, false)]
		[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
		public readonly struct NestedAccess : IDisposable
		{
			private readonly GameObject _gameObject;
			private readonly IAuthoringEntity _parent;
			private readonly List<IAuthoring> _nested;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public NestedAccess(AuthoringEntity parent)
			{
				_parent = parent;
				_gameObject = parent.gameObject;
				_nested = new List<IAuthoring>();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose() => _nested.Clear();

			public List<IAuthoring> Enumerate()
			{
				_gameObject.GetComponents(_nested);

				var parentIndex = _nested.IndexOf(_parent);
				if (parentIndex >= 0)
				{
					_nested.RemoveAt(parentIndex);
				}

				return _nested;
			}
		}
	}
}