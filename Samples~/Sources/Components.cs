using System;

namespace Depra.Ecs.Hybrid.Samples
{
	[Serializable]
	internal readonly struct DeadTag { }

	[Serializable]
	internal struct Health
	{
		public float Value;
	}

	[Serializable]
	internal struct Damage
	{
		public float Amount;
		public int Source;
	}
}