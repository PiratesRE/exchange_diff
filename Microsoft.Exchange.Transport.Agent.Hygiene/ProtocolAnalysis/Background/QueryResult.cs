using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal struct QueryResult
	{
		public TargetHost[] TargetHosts;

		public DnsStatus Status;
	}
}
