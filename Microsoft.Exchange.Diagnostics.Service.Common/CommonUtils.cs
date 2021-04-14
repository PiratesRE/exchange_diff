using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public static class CommonUtils
	{
		public static bool TryGetRegistryValue(string keyName, string valueName, object defaultValue, out object value)
		{
			bool result = false;
			value = defaultValue;
			try
			{
				object value2 = Registry.GetValue(keyName, valueName, null);
				if (value2 != null)
				{
					result = true;
					value = value2;
				}
			}
			catch (Exception)
			{
			}
			return result;
		}

		public static string FoldIntoSingleLine(string input)
		{
			return input.Replace(Environment.NewLine, "\t");
		}
	}
}
