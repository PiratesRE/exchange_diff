using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class AmTrace
	{
		internal static void Debug(string format, params object[] args)
		{
			ExTraceGlobals.ActiveManagerTracer.TraceDebug(0L, format, args);
		}

		internal static void Info(string format, params object[] args)
		{
			ExTraceGlobals.ActiveManagerTracer.TraceInformation(0, 0L, format, args);
		}

		internal static void Warning(string format, params object[] args)
		{
			ExTraceGlobals.ActiveManagerTracer.TraceWarning(0L, format, args);
		}

		internal static void Error(string format, params object[] args)
		{
			ExTraceGlobals.ActiveManagerTracer.TraceError(0L, format, args);
		}

		internal static void Entering(string format, params object[] args)
		{
			string formatString = "Entering " + format;
			ExTraceGlobals.ActiveManagerTracer.TraceFunction(0L, formatString, args);
		}

		internal static void Leaving(string format, params object[] args)
		{
			string formatString = "Leaving " + format;
			ExTraceGlobals.ActiveManagerTracer.TraceFunction(0L, formatString, args);
		}

		internal static void Diagnostic(string format, params object[] args)
		{
			string text = string.Format(format, args);
			AmTrace.Debug(text, new object[0]);
			ReplayCrimsonEvents.GenericMessage.Log<string>(text);
		}
	}
}
