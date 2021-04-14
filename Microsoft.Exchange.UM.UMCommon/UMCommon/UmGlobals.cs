using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class UmGlobals
	{
		public static ExEventLog ExEvent
		{
			get
			{
				return UmGlobals.eventLog;
			}
		}

		internal const string EventSourceName = "MSExchange Unified Messaging";

		private static ExEventLog eventLog = new ExEventLog(ExTraceGlobals.ServiceTracer.Category, "MSExchange Unified Messaging");
	}
}
