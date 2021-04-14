using System;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ProbeWorkItemLogger : IHygieneLogger, IDisposable
	{
		public ProbeWorkItemLogger(ProbeWorkItem probeWorkItem, bool logVerbose, bool logMessage)
		{
			this.probeWorkItem = probeWorkItem;
			this.logVerbose = logVerbose;
			this.logMessage = logMessage;
		}

		public void LogMessage(string s)
		{
			if (this.logMessage)
			{
				ProbeResult result = this.probeWorkItem.Result;
				result.ExecutionContext += string.Format("PROBE -- {0}. ", s);
			}
		}

		public void LogVerbose(string s)
		{
			if (this.logVerbose)
			{
				ProbeResult result = this.probeWorkItem.Result;
				result.ExecutionContext += string.Format("PROBE -- {0}. ", s);
			}
		}

		public void LogError(string s)
		{
			ProbeResult result = this.probeWorkItem.Result;
			result.ExecutionContext += string.Format("PROBE -- {0}. ", s);
		}

		public void Dispose()
		{
		}

		private readonly bool logVerbose;

		private readonly bool logMessage;

		private ProbeWorkItem probeWorkItem;
	}
}
