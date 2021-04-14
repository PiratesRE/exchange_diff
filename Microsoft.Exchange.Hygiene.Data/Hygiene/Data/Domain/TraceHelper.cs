using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HygieneData;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal static class TraceHelper
	{
		public static void TraceInformation(this DomainSession session, string formatString, params object[] args)
		{
			string formatString2 = session.ContextPrefix + formatString;
			ExTraceGlobals.DomainSessionTracer.Information((long)session.GetHashCode(), formatString2, args);
		}

		public static void TraceError(this DomainSession session, string formatString, params object[] args)
		{
			string formatString2 = session.ContextPrefix + formatString;
			ExTraceGlobals.DomainSessionTracer.TraceError((long)session.GetHashCode(), formatString2, args);
		}

		public static void TraceWarning(this DomainSession session, string formatString, params object[] args)
		{
			string formatString2 = session.ContextPrefix + formatString;
			ExTraceGlobals.DomainSessionTracer.TraceWarning((long)session.GetHashCode(), formatString2, args);
		}

		public static void TraceDebug(this DomainSession session, string formatString, params object[] args)
		{
			string formatString2 = session.ContextPrefix + formatString;
			ExTraceGlobals.DomainSessionTracer.TraceDebug((long)session.GetHashCode(), formatString2, args);
		}

		public static void TraceDebug(this DomainSession session, string message)
		{
			string message2 = session.ContextPrefix + message;
			ExTraceGlobals.DomainSessionTracer.TraceDebug((long)session.GetHashCode(), message2);
		}

		public static bool IsDebugTraceEnabled()
		{
			return ExTraceGlobals.DomainSessionTracer.IsTraceEnabled(TraceType.DebugTrace);
		}
	}
}
