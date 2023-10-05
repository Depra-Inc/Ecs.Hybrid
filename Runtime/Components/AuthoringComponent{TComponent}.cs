// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Ecs.Entities;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	public abstract class AuthoringComponent<TComponent> : AuthoringComponent where TComponent : struct
	{
		[SerializeField] private TComponent _value;

		private static readonly Type COMPONENT_TYPE = typeof(TComponent);

		internal override object Data => _value;

		internal override Type ComponentType => COMPONENT_TYPE;

		public override IComponentBaker CreateBaker(PackedEntityWithWorld entity) => new ComponentBaker(entity);
	}
}