using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PropertyDefinitionCollection
	{
		internal static T[] Merge<T>(params IEnumerable<T>[] propertyDefinitionCollections) where T : PropertyDefinition
		{
			ArgumentValidator.ThrowIfNull("propertyDefinitionCollections", propertyDefinitionCollections);
			ArgumentValidator.ThrowIfInvalidValue<int>("propertyDefinitionCollections.Length", propertyDefinitionCollections.Length, (int length) => length > 1);
			HashSet<T> hashSet = new HashSet<T>();
			foreach (IList<T> list in propertyDefinitionCollections)
			{
				if (list != null)
				{
					hashSet.UnionWith(list);
				}
			}
			return PropertyDefinitionCollection.GetArray<T>(hashSet);
		}

		internal static T[] Merge<T>(IEnumerable<T> propertyDefinitionCollection, params T[] properties) where T : PropertyDefinition
		{
			ArgumentValidator.ThrowIfNull("propertyDefinitionCollection", propertyDefinitionCollection);
			ArgumentValidator.ThrowIfNull("properties", properties);
			ArgumentValidator.ThrowIfInvalidValue<int>("properties.Length", properties.Length, (int length) => length > 0);
			HashSet<T> hashSet = new HashSet<T>(propertyDefinitionCollection);
			hashSet.UnionWith(properties);
			return PropertyDefinitionCollection.GetArray<T>(hashSet);
		}

		private static T[] GetArray<T>(HashSet<T> properties)
		{
			T[] array = new T[properties.Count];
			properties.CopyTo(array);
			return array;
		}
	}
}
