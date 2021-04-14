using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Management.Common
{
	internal static class IPAddressValidation
	{
		static IPAddressValidation()
		{
			string pattern = "^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
			IPAddressValidation.regExValidIPv4 = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
			string text = string.Format("^10\\.{0}\\.{1}\\.{2}$", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
			string text2 = string.Format("^172\\.(1[6-9]|2[0-9]|3[0-1])\\.{0}\\.{1}$", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
			string text3 = string.Format("^192\\.168\\.{0}\\.{1}$", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
			string text4 = string.Format("^127\\.0\\.0\\.{0}$", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
			string text5 = string.Format("^169\\.254\\.{0}\\.{1}$", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])", "(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])");
			pattern = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", new object[]
			{
				text,
				text2,
				text3,
				text4,
				text5,
				"^0\\.0\\.0\\.0$"
			});
			IPAddressValidation.regExReservedIPv4 = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
		}

		internal static bool IsValidIPv4Address(string ipAddr)
		{
			return IPAddressValidation.regExValidIPv4.IsMatch(ipAddr);
		}

		internal static bool IsReservedIPv4Address(string ipAddr)
		{
			return IPAddressValidation.regExReservedIPv4.IsMatch(ipAddr);
		}

		private static Regex regExValidIPv4;

		private static Regex regExReservedIPv4;
	}
}
