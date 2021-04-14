using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SmtpClientTransportSyncDebugOutput : ISmtpClientDebugOutput
	{
		internal SmtpClientTransportSyncDebugOutput(SyncLogSession syncLogSession)
		{
			this.syncLogSession = syncLogSession;
		}

		public void Output(Trace tracer, object context, string message, params object[] args)
		{
			int num = (context != null) ? context.GetHashCode() : 0;
			this.syncLogSession.LogVerbose((TSLID)9UL, tracer, (long)num, message, args);
		}

		private SyncLogSession syncLogSession;
	}
}
