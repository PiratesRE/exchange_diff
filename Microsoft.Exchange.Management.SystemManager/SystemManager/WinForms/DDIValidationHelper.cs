using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal static class DDIValidationHelper
	{
		public static Type[] GetAssemblyTypes(Assembly assembly)
		{
			Type[] result = null;
			try
			{
				result = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				result = (from type in ex.Types
				where type != null
				select type).ToArray<Type>();
			}
			return result;
		}
	}
}
