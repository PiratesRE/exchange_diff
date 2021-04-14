using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpReceivePerfCountersWrapper : ISmtpReceivePerfCounters
	{
		public SmtpReceivePerfCountersWrapper(string instanceName)
		{
			this.perfCountersInstance = SmtpReceivePerfCounters.GetInstance(instanceName);
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
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter InboundMessageConnectionsTotal
		{
			get
			{
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter MessagesReceivedTotal
		{
			get
			{
				return this.perfCountersInstance.MessagesReceivedTotal;
			}
		}

		public IExPerformanceCounter MessageBytesReceivedTotal
		{
			get
			{
				return this.perfCountersInstance.MessageBytesReceivedTotal;
			}
		}

		public IExPerformanceCounter MessagesRefusedForSize
		{
			get
			{
				return this.perfCountersInstance.MessagesRefusedForSize;
			}
		}

		public IExPerformanceCounter MessagesReceivedWithBareLinefeeds
		{
			get
			{
				return this.perfCountersInstance.MessagesReceivedWithBareLinefeeds;
			}
		}

		public IExPerformanceCounter MessagesReceivedForNonProvisionedUsers
		{
			get
			{
				return this.perfCountersInstance.MessagesReceivedForNonProvisionedUsers;
			}
		}

		public IExPerformanceCounter MessagesRefusedForBareLinefeeds
		{
			get
			{
				return this.perfCountersInstance.MessagesRefusedForBareLinefeeds;
			}
		}

		public IExPerformanceCounter RecipientsAccepted
		{
			get
			{
				return this.perfCountersInstance.RecipientsAccepted;
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

		private readonly SmtpReceivePerfCountersInstance perfCountersInstance;
	}
}
