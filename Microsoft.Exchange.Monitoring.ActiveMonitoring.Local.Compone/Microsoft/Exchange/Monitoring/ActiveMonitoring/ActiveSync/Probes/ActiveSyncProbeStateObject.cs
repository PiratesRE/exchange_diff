using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync.Probes
{
	public class ActiveSyncProbeStateObject
	{
		internal HttpWebRequest WebRequest { get; set; }

		internal ProbeState State { get; set; }

		internal ProbeResult Result { get; private set; }

		internal List<ActiveSyncWebResponse> WebResponses { get; set; }

		internal DateTime TimeoutLimit { get; set; }

		internal ManualResetEvent ResetEvent { get; set; }

		internal int LastResponseIndex
		{
			get
			{
				if (this.WebResponses.Count <= 0)
				{
					return -1;
				}
				return this.WebResponses.Count - 1;
			}
		}

		internal string HostOverrideValue { get; set; }

		internal string ProbeErrorResponse { get; set; }

		internal ActiveSyncProbeStateObject(HttpWebRequest request, ProbeResult result, ProbeState startingState)
		{
			this.WebRequest = request;
			this.Result = result;
			this.State = startingState;
			this.WebResponses = new List<ActiveSyncWebResponse>();
		}
	}
}
