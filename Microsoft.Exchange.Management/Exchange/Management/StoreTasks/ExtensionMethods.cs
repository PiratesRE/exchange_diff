using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal static class ExtensionMethods
	{
		public static T GetProperty<T>(this Item item, PropertyDefinition prop)
		{
			T result;
			try
			{
				result = item.GetValueOrDefault<T>(prop, default(T));
			}
			catch (InvalidOperationException)
			{
				result = default(T);
			}
			return result;
		}

		public static T GetPropertyValue<T>(this CalendarLogAnalysis log, PropertyDefinition prop)
		{
			object obj = null;
			if (log.InternalProperties.TryGetValue(prop, out obj))
			{
				try
				{
					return (T)((object)obj);
				}
				catch (InvalidCastException)
				{
					return default(T);
				}
			}
			return default(T);
		}

		public static string To64BitString(this byte[] bytes)
		{
			return GlobalObjectId.ByteArrayToHexString(bytes);
		}
	}
}
