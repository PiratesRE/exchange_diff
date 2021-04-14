using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class CommonLoggingHelper
	{
		public static SyncLogSession SyncLogSession
		{
			get
			{
				return CommonLoggingHelper.syncLogSession;
			}
			set
			{
				CommonLoggingHelper.syncLogSession = value;
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return CommonLoggingHelper.eventLogger;
			}
		}

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.EventLogTracer.Category, "MSExchangeTransportSyncCommon");

		private static SyncLogSession syncLogSession = SyncLogSession.InMemorySyncLogSession;
	}
}
