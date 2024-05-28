// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Ecs.Entities;

namespace Depra.Ecs.Hybrid.Entities
{
	public static class AuthoringEntityExtensions
	{
		public static bool Unpack(this IAuthoringEntity self, out Entity entity) => self.Unpack(out _, out entity);
	}
}