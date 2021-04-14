using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpReceivePerfCountersFrontendWrapper : ISmtpReceivePerfCounters
	{
		public SmtpReceivePerfCountersFrontendWrapper(string instanceName)
		{
			this.perfCountersInstance = SmtpReceiveFrontendPerfCounters.GetInstance(instanceName);
		}

		public IExPerformanceCounter ConnectionsCurrent
		{
			get
			{
				return this.perfCountersInstance.ConnectionsCurrent;
			}
		}

		public IExPerformanceCounter ConnectionsTotal
		{
			get
			{
				return this.perfCountersInstance.ConnectionsTotal;
			}
		}

		public IExPerformanceCounter TlsConnectionsCurrent
		{
			get
			{
				return this.perfCountersInstance.TlsConnectionsCurrent;
			}
		}

		public IExPerformanceCounter TlsNegotiationsFailed
		{
			get
			{
				return this.perfCountersInstance.TlsNegotiationsFailed;
			}
		}

		public IExPerformanceCounter TlsConnectionsRejectedDueToRateExceeded
		{
			get
			{
				return this.perfCountersInstance.TlsConnectionsRejectedDueToRateExceeded;
			}
		}

		public IExPerformanceCounter ConnectionsDroppedByAgentsTotal
		{
			get
			{
				return this.perfCountersInstance.ConnectionsDroppedByAgentsTotal;
			}
		}

		public IExPerformanceCounter InboundMessageConnectionsCurrent
		{
			get
			{
				return this.perfCountersInstance.InboundMessageConnectionsCurrent;
			}
		}

		public IExPerformanceCounter InboundMessageConnectionsTotal
		{
			get
			{
				return this.perfCountersInstance.InboundMessageConnectionsTotal;
			}
		}

		public IExPerformanceCounter MessagesReceivedTotal
		{
			get
			{
				return this.perfCountersInstance.InboundMessagesReceivedTotal;
			}
		}

		public IExPerformanceCounter MessageBytesReceivedTotal
		{
			get
			{
				return this.perfCountersInstance.InboundMessageBytesReceivedTotal;
			}
		}

		public IExPerformanceCounter MessagesRefusedForSize
		{
			get
			{
				return this.perfCountersInstance.InboundMessagesRefusedForSize;
			}
		}

		public IExPerformanceCounter MessagesReceivedWithBareLinefeeds
		{
			get
			{
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter MessagesReceivedForNonProvisionedUsers
		{
			get
			{
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter MessagesRefusedForBareLinefeeds
		{
			get
			{
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter RecipientsAccepted
		{
			get
			{
				return this.perfCountersInstance.InboundRecipientsAccepted;
			}
		}

		public IExPerformanceCounter TotalBytesReceived
		{
			get
			{
				return this.perfCountersInstance.TotalBytesReceived;
			}
		}

		public IExPerformanceCounter TarpittingDelaysAuthenticated
		{
			get
			{
				return this.perfCountersInstance.TarpittingDelaysAuthenticated;
			}
		}

		public IExPerformanceCounter TarpittingDelaysAnonymous
		{
			get
			{
				return this.perfCountersInstance.TarpittingDelaysAnonymous;
			}
		}

		public IExPerformanceCounter TarpittingDelaysBackpressure
		{
			get
			{
				return this.perfCountersInstance.TarpittingDelaysBackpressure;
			}
		}

		private readonly SmtpReceiveFrontendPerfCountersInstance perfCountersInstance;
	}
}
