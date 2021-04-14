using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class NullInboundProxyDestinationPerfCounters : IInboundProxyDestinationPerfCounters
	{
		public IExPerformanceCounter ConnectionsCurrent
		{
			get
			{
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter ConnectionsTotal
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
				return TransportNoopExPerformanceCounter.Instance;
			}
		}

		public IExPerformanceCounter MessageBytesReceivedTotal
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
				return TransportNoopExPerformanceCounter.Instance;
			}
		}
	}
}
