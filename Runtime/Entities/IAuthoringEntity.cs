using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;

namespace Depra.Ecs.Hybrid.Entities
{
	public interface IAuthoringEntity : IAuthoring
	{
		bool Unpack(out Entity entity);
	}
}