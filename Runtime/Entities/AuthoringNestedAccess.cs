// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Depra.Ecs.Hybrid
{
	public readonly struct AuthoringNestedAccess : IAuthoringAccess
	{
		private readonly GameObject _gameObject;
		private readonly IAuthoringEntity _parent;
		private readonly List<IAuthoring> _nested;

		public AuthoringNestedAccess(AuthoringEntity parent)
		{
			_parent = parent;
			_gameObject = parent.gameObject;
			_nested = ListPool<IAuthoring>.Get();
		}
		
		public AuthoringNestedAccess(IAuthoringEntity parent, GameObject gameObject)
		{
			_parent = parent;
			_gameObject = gameObject;
			_nested = ListPool<IAuthoring>.Get();
		}

		public void Dispose() => ListPool<IAuthoring>.Release(_nested);

		List<IAuthoring> IAuthoringAccess.Enumerate()
		{
			_gameObject.GetComponents(_nested);

			var parentIndex = _nested.IndexOf(_parent);
			if (parentIndex >= 0)
			{
				_nested.RemoveAt(parentIndex);
			}

			return _nested;
		}
	}
}