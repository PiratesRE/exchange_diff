using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public class PopImapProbeStateObject
	{
		internal ProbeState State { get; set; }

		internal ProbeResult Result { get; private set; }

		internal List<TcpResponse> TcpResponses { get; set; }

		internal DateTime TimeoutLimit { get; set; }

		internal int LastResponseIndex
		{
			get
			{
				if (this.TcpResponses.Count <= 0)
				{
					return -1;
				}
				return this.TcpResponses.Count - 1;
			}
		}

		internal string ProbeErrorResponse { get; set; }

		internal TcpConnection Connection { get; private set; }

		internal string Command { get; set; }

		internal string ExpectedTag { get; set; }

		internal bool MultiLine { get; set; }

		internal string UserAccount { get; set; }

		internal string UserPassword { get; set; }

		internal string FailingReason { get; set; }

		internal Exception ConnectionException { get; set; }

		internal PopImapProbeStateObject(TcpConnection protocolConnection, ProbeResult result, ProbeState startingState)
		{
			this.Connection = protocolConnection;
			this.Result = result;
			this.State = startingState;
			this.TcpResponses = new List<TcpResponse>();
		}
	}
}
