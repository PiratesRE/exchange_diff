using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[Flags]
	internal enum TransportAgentFlags
	{
		None = 0,
		InitializePerfCounters = 4
	}
}
