using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal static class SystemProbeUtilities
	{
		public static Guid GetProbeGuid(MailItem mailItem)
		{
			if (mailItem == null)
			{
				return Guid.Empty;
			}
			return mailItem.SystemProbeId;
		}

		public static void InitForThread(MailItem mailItem)
		{
			SystemProbe.ActivityId = SystemProbeUtilities.GetProbeGuid(mailItem);
		}

		public static void TracePass(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string message)
		{
			tracer.TracePass(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, message);
		}

		public static void TracePass<T0>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0)
		{
			tracer.TracePass<T0>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0);
		}

		public static void TracePass<T0, T1>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TracePass<T0, T1>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0, arg1);
		}

		public static void TracePass<T0, T1, T2>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TracePass<T0, T1, T2>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0, arg1, arg2);
		}

		public static void TracePass(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, params object[] args)
		{
			tracer.TracePass(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, args);
		}

		public static void TraceFail(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string message)
		{
			tracer.TraceFail(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, message);
		}

		public static void TraceFail<T0>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0)
		{
			tracer.TraceFail<T0>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0);
		}

		public static void TraceFail<T0, T1>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0, T1 arg1)
		{
			tracer.TraceFail<T0, T1>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0, arg1);
		}

		public static void TraceFail<T0, T1, T2>(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, T0 arg0, T1 arg1, T2 arg2)
		{
			tracer.TraceFail<T0, T1, T2>(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, arg0, arg1, arg2);
		}

		public static void TraceFail(this SystemProbeTrace tracer, MailItem mailItem, long etlTraceId, string formatString, params object[] args)
		{
			tracer.TraceFail(SystemProbeUtilities.GetProbeGuid(mailItem), etlTraceId, formatString, args);
		}
	}
}
