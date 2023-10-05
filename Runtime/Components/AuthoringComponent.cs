// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Entities;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	[RequireComponent(typeof(AuthoringEntity))]
	public abstract class AuthoringComponent<TComponent> : MonoBehaviour, IAuthoring where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		protected TComponent Value => _value;

		public abstract IBaker CreateBaker(PackedEntityWithWorld entity);
	}
}