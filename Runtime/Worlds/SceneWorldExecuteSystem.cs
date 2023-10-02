using Depra.Ecs.Filters;
using Depra.Ecs.Pools;
using Depra.Ecs.Systems;

namespace Depra.Ecs.Baking.Runtime.Worlds
{
#if ENABLE_IL2CPP
	using Unity.IL2CPP.CompilerServices;

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class SceneWorldExecuteSystem : IPreInitializeSystem, IExecuteSystem
	{
		private EntityFilter _entities;
		private ComponentPool<ConvertibleGameObject> _convertibles;

		void IPreInitializeSystem.PreInitialize(IWorldSystems systems)
		{
			var world = systems.World;
			_entities = world.Filter<ConvertibleGameObject>().End();
			_convertibles = world.Pool<ConvertibleGameObject>();
		}

		void IExecuteSystem.Execute(float frameTime)
		{
			foreach (int entity in _entities)
			{
				ref var convertible = ref _convertibles[entity];

				if (convertible.Value)
				{
					SceneEntity.TryConvert(convertible.Value, _convertibles.World);
				}

				_convertibles.Delete(entity);
			}
		}
	}
}