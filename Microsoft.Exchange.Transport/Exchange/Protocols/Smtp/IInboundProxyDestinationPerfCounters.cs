using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IInboundProxyDestinationPerfCounters
	{
		IExPerformanceCounter ConnectionsCurrent { get; }

		IExPerformanceCounter ConnectionsTotal { get; }

		IExPerformanceCounter MessagesReceivedTotal { get; }

		IExPerformanceCounter MessageBytesReceivedTotal { get; }

		IExPerformanceCounter RecipientsAccepted { get; }
	}
}
