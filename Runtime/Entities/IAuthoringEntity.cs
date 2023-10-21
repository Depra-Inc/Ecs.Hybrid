using Depra.Ecs.Baking.Components;

namespace Depra.Ecs.Baking.Entities
{
	internal interface IAuthoringEntity
	{
		IAuthoring[] Components { get; }
	}
}