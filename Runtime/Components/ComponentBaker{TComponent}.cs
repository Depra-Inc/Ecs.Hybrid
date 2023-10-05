// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Ecs.Baking.Runtime.Components
{
	public abstract class ComponentBaker<TAuthoring> : IComponentBaker where TAuthoring : Component
	{
		public abstract void Bake(TAuthoring authoring);

		public void Bake(AuthoringComponent authoring) => Bake(authoring as TAuthoring);
	}
}