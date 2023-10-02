using Depra.Ecs.Worlds;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
	internal static class SceneWorld
	{
		public static World World;

		public static void Initialize(World world) => World = world;

		public static void Destroy() => World = null;
	}
}