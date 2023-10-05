// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Ecs.Baking.Runtime.Entities;
using Depra.Ecs.Entities;
using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	[RequireComponent(typeof(AuthoringEntity))]
	public abstract class AuthoringComponent : MonoBehaviour
	{
		internal abstract object Data { get; }

		internal abstract Type ComponentType { get; }

		public abstract IComponentBaker CreateBaker(PackedEntityWithWorld entity);
	}
}