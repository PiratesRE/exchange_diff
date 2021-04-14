using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class InboundProxyAccountForestPerfCountersWrapper : IInboundProxyDestinationPerfCounters
	{
		public InboundProxyAccountForestPerfCountersWrapper(string instanceName)
		{
			this.perfCountersInstance = InboundProxyAccountForestPerfCounters.GetInstance(instanceName);
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

		public IExPerformanceCounter RecipientsAccepted
		{
			get
			{
				return this.perfCountersInstance.InboundRecipientsAccepted;
			}
		}

		private readonly InboundProxyAccountForestPerfCountersInstance perfCountersInstance;
	}
}
