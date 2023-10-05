// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Ecs.Baking.Runtime.Components
{
	public interface IBaker
	{
		void Bake(IAuthoring authoring);
	}
}