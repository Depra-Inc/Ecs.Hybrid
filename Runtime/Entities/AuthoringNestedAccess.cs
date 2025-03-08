// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using UnityEngine.Pool;

namespace Depra.Ecs.Hybrid
{
	public readonly struct AuthoringNestedAccess : IAuthoringAccess
	{
		private readonly AuthoringEntity _parent;
		private readonly List<IAuthoring> _nested;

		public AuthoringNestedAccess(AuthoringEntity parent)
		{
			_parent = parent;
			_nested = ListPool<IAuthoring>.Get();
		}

		public void Dispose() => ListPool<IAuthoring>.Release(_nested);

		List<IAuthoring> IAuthoringAccess.Enumerate()
		{
			_parent.GetComponents(_nested);

			var parentIndex = _nested.IndexOf(_parent);
			if (parentIndex >= 0)
			{
				_nested.RemoveAt(parentIndex);
			}

			return _nested;
		}
	}
}