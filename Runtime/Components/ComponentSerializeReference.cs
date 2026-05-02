// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Depra <n.melnikov@depra.org>

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

		public override IEnumerable<Type> GetDerivedTypes(Type referenceType)
		{
#if UNITY_EDITOR
			return from extractedType in UnityEditor.TypeCache.GetTypesDerivedFrom(referenceType)
				where extractedType.IsPublic &&
				      extractedType.IsValueType &&
				      !extractedType.IsGenericType &&
				      extractedType.FullName!.Contains(_nameSubstring) &&
				      !IsDefined(extractedType, HideSerializeReferenceAttribute.TYPE)
				// TODO: Uncomment this line after obsolete serialization is removed.
				// && extractedType.GetCustomAttribute<SerializableAttribute>() != null
				select extractedType;
#else
			return Array.Empty<Type>();
#endif
		}

		public override IEnumerable<Type> GetGenericTypes(Type referenceType) => Array.Empty<Type>();
	}
}