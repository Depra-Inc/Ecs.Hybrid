// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Ecs.Hybrid
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public static class AuthoringEntityExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Unpack(this IAuthoringEntity self, out Entity entity) => self.Unpack(out _, out entity);
	}
}