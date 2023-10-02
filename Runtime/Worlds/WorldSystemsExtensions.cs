using System.Runtime.CompilerServices;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
	public static class WorldSystemsExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IWorldSystems ConvertScene<TSystems>(this TSystems self) where TSystems : IWorldSystems => self
			.Add(new SceneWorldInitializeSystem())
			.Add(new SceneWorldExecuteSystem())
			.Add(new SceneWorldTearDownSystem());
	}
}