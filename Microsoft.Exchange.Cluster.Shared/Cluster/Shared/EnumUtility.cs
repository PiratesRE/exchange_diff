using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class EnumUtility
	{
		public static bool TryParse<T>(string value, out T result, T defaultValue) where T : struct
		{
			return EnumUtility.TryParse<T>(value, out result, defaultValue, true);
		}

		public static bool TryParse<T>(string value, out T result, T defaultValue, bool ignoreCase) where T : struct
		{
			result = defaultValue;
			if (value != null)
			{
				try
				{
					result = (T)((object)Enum.Parse(typeof(T), value, ignoreCase));
					if (Enum.IsDefined(typeof(T), result))
					{
						return true;
					}
					result = defaultValue;
					return false;
				}
				catch (ArgumentException)
				{
				}
				return false;
			}
			return false;
		}
	}
}
