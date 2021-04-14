using System;
using System.Collections.Generic;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingPerformanceCounters
	{
		public RoutingPerformanceCounters(ProcessTransportRole transportRole)
		{
			this.transportRole = transportRole;
			this.Initialize();
		}

		public virtual void IncrementRoutingNdrs()
		{
			if (this.instance != null)
			{
				this.instance.RoutingNdrsTotal.Increment();
			}
		}

		public virtual void IncrementRoutingTablesCalculated()
		{
			if (this.instance != null)
			{
				this.instance.RoutingTablesCalculatedTotal.Increment();
			}
		}

		public virtual void IncrementRoutingTablesChanged()
		{
			if (this.instance != null)
			{
				this.instance.RoutingTablesChangedTotal.Increment();
			}
		}

		protected virtual void Initialize()
		{
			try
			{
				RoutingPerfCounters.SetCategoryName(RoutingPerformanceCounters.perfCounterCategoryMap[this.transportRole]);
				this.instance = RoutingPerfCounters.GetInstance("_total");
			}
			catch (InvalidOperationException ex)
			{
				RoutingDiag.Tracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "Failed to initialize performance counters: {0}", ex);
				RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingPerfCountersLoadFailure, null, new object[]
				{
					ex.ToString()
				});
			}
		}

		private const string InstanceName = "_total";

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport Routing"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport Routing"
			},
			{
				ProcessTransportRole.FrontEnd,
				"MSExchangeFrontEndTransport Routing"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery Routing"
			},
			{
				ProcessTransportRole.MailboxSubmission,
				"MSExchange Submission Routing"
			}
		};

		private RoutingPerfCountersInstance instance;

		private ProcessTransportRole transportRole;
	}
}
