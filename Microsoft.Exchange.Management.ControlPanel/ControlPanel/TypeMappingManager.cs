using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class TypeMappingManager<T> where T : class, ITypeMapping
	{
		public void RegisterMapping(T typeMapping)
		{
			int i;
			for (i = this.sortedTypeMappings.Count; i > 0; i--)
			{
				Type sourceType = typeMapping.SourceType;
				T t = this.sortedTypeMappings[i - 1];
				if (sourceType.IsAssignableFrom(t.SourceType))
				{
					break;
				}
			}
			this.sortedTypeMappings.Insert(i, typeMapping);
		}

		public T[] GetNearestMappings(Type type)
		{
			T firstNeareastMapping = this.sortedTypeMappings.FirstOrDefault((T mapping) => mapping.SourceType.IsAssignableFrom(type));
			if (firstNeareastMapping != null)
			{
				return (from mapping in this.sortedTypeMappings
				where mapping.SourceType == firstNeareastMapping.SourceType
				select mapping).ToArray<T>();
			}
			return new T[0];
		}

		private List<T> sortedTypeMappings = new List<T>();
	}
}
