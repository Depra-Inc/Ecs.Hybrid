// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;

namespace Depra.Ecs.Hybrid
{
	public interface IAuthoringAccess : IDisposable
	{
		List<IAuthoring> Enumerate();
	}
}