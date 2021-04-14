using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal static class PolicySyncAssert
	{
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				PolicySyncAssert.AssertInternal(formatString, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString)
		{
			if (!condition)
			{
				PolicySyncAssert.AssertInternal(formatString, null);
			}
		}

		public static void RetailAssert(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				PolicySyncAssert.AssertInternal(formatString, parameters);
			}
		}

		public static void RetailAssert(bool condition, string formatString)
		{
			if (!condition)
			{
				PolicySyncAssert.AssertInternal(formatString, null);
			}
		}

		private static void AssertInternal(string formatString, params object[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder("ASSERT: ");
			if (formatString != null)
			{
				if (parameters != null)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, formatString, parameters);
				}
				else
				{
					stringBuilder.Append(formatString);
				}
			}
			stringBuilder.ToString();
		}
	}
}
