using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class ValidateArgument
	{
		public static void NotNullOrEmpty(string value, string name)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Parameter cannot be null or empty", name);
			}
		}

		public static void NotNull(object value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name, "Parameter cannot be null");
			}
		}
	}
}
