using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	internal class TraceException
	{
		internal static void Setup()
		{
			LocalizedException.TraceExceptionCallback = new LocalizedException.TraceExceptionDelegate(TraceException.TraceExceptionFn);
		}

		internal static void TraceExceptionFn(string formatString, params object[] formatObjects)
		{
			ExTraceGlobals.CommonTracer.TraceDebug(17510, 0L, formatString, formatObjects);
		}
	}
}
