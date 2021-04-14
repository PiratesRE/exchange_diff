using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal struct ASTraceFilter : IDisposable
	{
		public ASTraceFilter(string mailboxDN, string requestor)
		{
			this.traceEnabled = false;
			this.traceConfig = ASTraceConfiguration.Instance;
			if (!this.traceConfig.FilteredTracingEnabled)
			{
				return;
			}
			this.traceEnabled = (this.IsMailboxFiltered(mailboxDN) || this.IsRequesterFiltered(requestor));
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

		private bool IsRequesterFiltered(string requestor)
		{
			return !string.IsNullOrEmpty(requestor) && this.traceConfig.FilteredRequesters.Exists((string user) => StringComparer.OrdinalIgnoreCase.Equals(user, requestor));
		}

		private bool IsMailboxFiltered(string mailboxDN)
		{
			return mailboxDN != null && this.traceConfig.FilteredMailboxes.Exists((string legacyDN) => StringComparer.OrdinalIgnoreCase.Equals(mailboxDN, legacyDN));
		}

		private bool traceEnabled;

		private ASTraceConfiguration traceConfig;
	}
}
