using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal static class Utilities
	{
		public static bool TryExtractForestFqdnFromServerFqdn(string serverFqdn, out string forestFqdn)
		{
			forestFqdn = string.Empty;
			Fqdn fqdn;
			if (!Fqdn.TryParse(serverFqdn, out fqdn))
			{
				return false;
			}
			int num = serverFqdn.IndexOf('.');
			if (num < 0)
			{
				return false;
			}
			forestFqdn = serverFqdn.Substring(num + 1);
			return true;
		}

		public static void GetTwoSubstrings(string input, char separator, out string str1, out string str2)
		{
			str1 = null;
			str2 = null;
			if (string.IsNullOrEmpty(input))
			{
				return;
			}
			int num = input.IndexOf(separator);
			if (num == -1)
			{
				str1 = input;
				return;
			}
			str1 = input.Substring(0, num);
			int num2 = input.IndexOf(separator, num + 1);
			if (num2 == -1)
			{
				str2 = input.Substring(num + 1);
				return;
			}
			str2 = input.Substring(num + 1, num2 - num - 1);
		}
	}
}
