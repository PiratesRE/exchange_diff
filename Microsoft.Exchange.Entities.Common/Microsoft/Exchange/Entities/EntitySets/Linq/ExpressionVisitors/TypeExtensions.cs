using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	public static class TypeExtensions
	{
		public static Type FindGenericType(this Type typeToInspect, Type genericTypeToFind)
		{
			while (typeToInspect != null && typeToInspect != typeof(object))
			{
				if (typeToInspect.IsGenericType && typeToInspect.GetGenericTypeDefinition() == genericTypeToFind)
				{
					return typeToInspect;
				}
				if (genericTypeToFind.IsInterface)
				{
					foreach (Type typeToInspect2 in typeToInspect.GetInterfaces())
					{
						Type type = typeToInspect2.FindGenericType(genericTypeToFind);
						if (type != null)
						{
							return type;
						}
					}
				}
				typeToInspect = typeToInspect.BaseType;
			}
			return null;
		}

		public static Type GetEnumerableElementTypeOrSameType(this Type typeToInspect)
		{
			Type type = typeToInspect.FindGenericType(typeof(IEnumerable<>));
			if (!(type != null))
			{
				return typeToInspect;
			}
			return type.GetGenericArguments()[0];
		}
	}
}
