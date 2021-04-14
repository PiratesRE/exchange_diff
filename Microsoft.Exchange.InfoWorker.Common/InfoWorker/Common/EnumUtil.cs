using System;

namespace Microsoft.Exchange.InfoWorker.Common
{
	public static class EnumUtil
	{
		public static T Parse<T>(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return default(T);
			}
			return EnumUtil.enumTypeMap.Parse<T>(value);
		}

		public static string ToString<T>(T value)
		{
			return EnumUtil.enumTypeMap.ToString<T>(value);
		}

		private static EnumTypeMap enumTypeMap = new EnumTypeMap();
	}
}
