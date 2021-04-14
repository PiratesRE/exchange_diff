using System;
using System.Reflection;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class TypeExtension
	{
		public static PropertyInfo GetPropertyEx(this Type type, string name)
		{
			PropertyInfo propertyInfo = null;
			try
			{
				propertyInfo = type.GetProperty(name);
			}
			catch (AmbiguousMatchException)
			{
				do
				{
					propertyInfo = type.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
				}
				while (propertyInfo == null && (type = type.BaseType) != null);
			}
			return propertyInfo;
		}
	}
}
