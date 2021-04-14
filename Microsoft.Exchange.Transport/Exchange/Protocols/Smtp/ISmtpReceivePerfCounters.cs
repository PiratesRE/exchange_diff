using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpReceivePerfCounters
	{
		IExPerformanceCounter ConnectionsCurrent { get; }

		IExPerformanceCounter ConnectionsTotal { get; }

		IExPerformanceCounter TlsConnectionsCurrent { get; }

		IExPerformanceCounter TlsConnectionsRejectedDueToRateExceeded { get; }

		IExPerformanceCounter ConnectionsDroppedByAgentsTotal { get; }

		IExPerformanceCounter InboundMessageConnectionsCurrent { get; }

		IExPerformanceCounter InboundMessageConnectionsTotal { get; }

		IExPerformanceCounter MessagesReceivedTotal { get; }

		IExPerformanceCounter MessageBytesReceivedTotal { get; }

		IExPerformanceCounter MessagesRefusedForSize { get; }

		IExPerformanceCounter MessagesReceivedWithBareLinefeeds { get; }

		IExPerformanceCounter MessagesRefusedForBareLinefeeds { get; }

		IExPerformanceCounter MessagesReceivedForNonProvisionedUsers { get; }

		IExPerformanceCounter RecipientsAccepted { get; }

		IExPerformanceCounter TotalBytesReceived { get; }

		IExPerformanceCounter TarpittingDelaysAuthenticated { get; }

		IExPerformanceCounter TarpittingDelaysAnonymous { get; }

		IExPerformanceCounter TarpittingDelaysBackpressure { get; }

		IExPerformanceCounter TlsNegotiationsFailed { get; }
	}
}
