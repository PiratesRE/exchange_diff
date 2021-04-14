using System;
using System.Threading;
using Cobalt;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class CobaltHostServices : HostServices
	{
		static CobaltHostServices()
		{
			HostServices.Singleton = new CobaltHostServices();
		}

		public static void Initialize()
		{
		}

		public override bool IsLoggingLevelEnabled(Log.Level level)
		{
			return true;
		}

		public override void WriteToLog(Log.Level level, string message)
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			int lid = 0;
			switch (level)
			{
			case 0:
			case 3:
			case 7:
				ExTraceGlobals.CobaltTracer.TraceWarning(lid, (long)managedThreadId, message);
				return;
			case 1:
			case 2:
				ExTraceGlobals.CobaltTracer.TraceError(lid, (long)managedThreadId, message);
				return;
			case 4:
			case 5:
			case 6:
				ExTraceGlobals.CobaltTracer.TraceInformation(lid, (long)managedThreadId, message);
				return;
			default:
				ExTraceGlobals.CobaltTracer.TraceInformation(lid, (long)managedThreadId, message);
				return;
			}
		}
	}
}
