using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal struct SenderGuidTraceFilter : IDisposable
	{
		public SenderGuidTraceFilter(Guid mdbGuid, Guid mailboxGuid)
		{
			this.traceConfig = TraceConfigurationSingleton<MailboxGuidTraceConfiguration>.Instance;
			this.traceEnabled = false;
			if (!this.traceConfig.FilteredTracingEnabled)
			{
				return;
			}
			this.traceEnabled = (this.IsMDBFiltered(mdbGuid) || this.IsMailboxFiltered(mailboxGuid));
			if (this.traceEnabled)
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
		}

		public void Dispose()
		{
			if (this.traceEnabled)
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}

		private bool IsMDBFiltered(Guid mdbGuid)
		{
			return this.traceConfig.FilteredMDBs.Contains(mdbGuid);
		}

		private bool IsMailboxFiltered(Guid mailboxGuid)
		{
			return this.traceConfig.FilteredMailboxs.Contains(mailboxGuid);
		}

		private bool traceEnabled;

		private MailboxGuidTraceConfiguration traceConfig;
	}
}
