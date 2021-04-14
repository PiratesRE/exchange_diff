using System;

namespace Microsoft.Exchange.Data
{
	internal static class MachineName
	{
		internal static StringComparer Comparer
		{
			get
			{
				return StringComparer.OrdinalIgnoreCase;
			}
		}

		internal static StringComparison Comparison
		{
			get
			{
				return StringComparison.OrdinalIgnoreCase;
			}
		}

		internal static string GetNodeNameFromFqdn(string fqdn)
		{
			int num = fqdn.IndexOf(".");
			if (num != -1)
			{
				fqdn = fqdn.Substring(0, num);
			}
			return fqdn;
		}

		public static readonly string Local = Environment.MachineName;
	}
}
