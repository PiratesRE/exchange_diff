using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Inference.Common
{
	internal static class ConfigurationUtils
	{
		public static ByteQuantifiedSize ReadByteQuantifiedSizeValue(string configName, ByteQuantifiedSize defaultValue)
		{
			string expression = null;
			if (!AppConfigLoader.TryGetConfigRawValue(configName, out expression))
			{
				return defaultValue;
			}
			ByteQuantifiedSize result;
			if (!ByteQuantifiedSize.TryParse(expression, out result))
			{
				return defaultValue;
			}
			return result;
		}

		public static List<string> ReadCommaSeperatedStringValue(string configName, List<string> defaultValue)
		{
			string text = null;
			if (!AppConfigLoader.TryGetConfigRawValue(configName, out text))
			{
				return defaultValue;
			}
			string[] array = text.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return defaultValue;
			}
			return new List<string>(array);
		}
	}
}
