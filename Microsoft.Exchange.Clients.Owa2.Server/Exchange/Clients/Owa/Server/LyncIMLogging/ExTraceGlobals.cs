using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging
{
	public class ExTraceGlobals
	{
		private ExTraceGlobals()
		{
			SyncLogConfiguration syncLogConfiguration = LogConfiguration.CreateSyncLogConfiguration();
			SyncLog syncLog = new SyncLog(syncLogConfiguration);
			ExTraceGlobals.syncLogSession = syncLog.OpenGlobalSession();
		}

		public static ExTraceGlobals InstantMessagingTracer
		{
			get
			{
				if (ExTraceGlobals.instantMessagingTracer == null)
				{
					lock (ExTraceGlobals.instanceInitializationLock)
					{
						if (ExTraceGlobals.instantMessagingTracer == null)
						{
							ExTraceGlobals.instantMessagingTracer = new ExTraceGlobals();
						}
					}
				}
				return ExTraceGlobals.instantMessagingTracer;
			}
		}

		public void TraceDebug(long id, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(id, message);
			ExTraceGlobals.syncLogSession.LogDebugging((TSLID)0UL, "DEBUG:" + message, new object[0]);
		}

		public void TraceDebug(long id, string formatString, params object[] args)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug(id, formatString, args);
			ExTraceGlobals.syncLogSession.LogDebugging((TSLID)0UL, "DEBUG:" + formatString, args);
		}

		public void TraceError(long id, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError(id, message);
			ExTraceGlobals.syncLogSession.LogError((TSLID)0UL, "ERROR:" + message, new object[0]);
		}

		public void TraceError(long id, string formatString, params object[] args)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceError(id, formatString, args);
			ExTraceGlobals.syncLogSession.LogError((TSLID)0UL, "ERROR:" + formatString, args);
		}

		private static object instanceInitializationLock = new object();

		private static ExTraceGlobals instantMessagingTracer = null;

		private static GlobalSyncLogSession syncLogSession;
	}
}
