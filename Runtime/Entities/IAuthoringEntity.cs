using System.Collections.Generic;
using Depra.Ecs.Entities;
using Depra.Ecs.Hybrid.Components;

namespace Depra.Ecs.Hybrid.Entities
{
	public interface IAuthoringEntity : IAuthoring
	{
		IEnumerable<IAuthoring> Nested { get; }

		bool TryGetEntity(out Entity entity);
	}
}