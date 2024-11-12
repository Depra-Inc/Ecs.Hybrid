// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Linq;
using Depra.SerializeReference.Extensions;

namespace Depra.Ecs.Hybrid
{
	public sealed class ComponentSerializeReference : SerializeReferenceAttribute
	{
		private readonly string _nameSubstring;

		public ComponentSerializeReference(string nameSubstring) => _nameSubstring = nameSubstring;

		public override IEnumerable<Type> GetTypes(Type referenceType) =>
			from assembly in AppDomain.CurrentDomain.GetAssemblies()
			from extractedType in assembly.GetTypes()
			where extractedType.IsPublic &&
			      extractedType.IsValueType &&
			      !extractedType.IsGenericType &&
			      extractedType.FullName!.Contains(_nameSubstring)
			select extractedType;
	}
}