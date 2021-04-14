using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal enum WorkItemPriority
	{
		InvalidPriority,
		OpenProxyPriority,
		ReverseDnsQueryPriority,
		BlockSenderPriority
	}
}
