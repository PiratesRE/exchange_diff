using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics
{
	public static class ExAssert
	{
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				ExAssert.AssertInternal(formatString, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string formatString)
		{
			if (!condition)
			{
				ExAssert.AssertInternal(formatString, null);
			}
		}

		public static void RetailAssert(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				ExAssert.AssertInternal(formatString, parameters);
			}
		}

		public static void RetailAssert(bool condition, string formatString)
		{
			if (!condition)
			{
				ExAssert.AssertInternal(formatString, null);
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
			string text = stringBuilder.ToString();
			ExTraceGlobals.CommonTracer.TraceDebug(23657, 0L, text);
			throw new ExAssertException(text);
		}
	}
}
