using Depra.Ecs.Hybrid.Components;

namespace Depra.Ecs.Hybrid.Entities
{
	internal interface IAuthoringEntity
	{
		IAuthoring[] Components { get; }
	}
}