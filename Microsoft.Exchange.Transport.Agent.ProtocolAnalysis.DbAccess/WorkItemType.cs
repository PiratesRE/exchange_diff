using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal enum WorkItemType
	{
		InvalidType,
		OpenProxyDetection,
		ReverseDnsQuery,
		BlockSender
	}
}
