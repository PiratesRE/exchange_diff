using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxyHubSelectorPerformanceCounters
	{
		public ProxyHubSelectorPerformanceCounters(ProcessTransportRole role)
		{
			this.Initialize(role);
		}

		public virtual void IncrementHubSelectionRequestsTotal()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionRequestsTotal.Increment();
			}
		}

		public virtual void IncrementResolverFailures()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionResolverFailures.Increment();
			}
		}

		public virtual void IncrementOrganizationMailboxFailures()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionOrganizationMailboxFailures.Increment();
			}
		}

		public virtual void IncrementMessagesWithoutMailboxRecipients()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionMessagesWithoutMailboxRecipients.Increment();
			}
		}

		public virtual void IncrementMessagesWithoutOrganizationMailboxes()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionMessagesWithoutOrganizationMailboxes.Increment();
			}
		}

		public virtual void IncrementMessagesRoutedUsingDagSelector()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionMessagesRoutedUsingDagSelector.Increment();
			}
		}

		public virtual void IncrementFallbackRoutingRequests()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionFallbackRoutingRequests.Increment();
			}
		}

		public virtual void IncrementRoutingFailures()
		{
			if (this.instance != null)
			{
				this.instance.HubSelectionRoutingFailures.Increment();
			}
		}

		protected virtual void Initialize(ProcessTransportRole role)
		{
			try
			{
				ProxyHubSelectorPerfCounters.SetCategoryName(ProxyHubSelectorPerformanceCounters.GetCategoryName(role));
				this.instance = ProxyHubSelectorPerfCounters.GetInstance("_total");
			}
			catch (InvalidOperationException arg)
			{
				ProxyHubSelectorComponent.Tracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "Failed to initialize performance counters: {0}", arg);
			}
		}

		private static string GetCategoryName(ProcessTransportRole role)
		{
			switch (role)
			{
			case ProcessTransportRole.FrontEnd:
				return "MSExchangeFrontEndTransport ProxyHubSelector";
			case ProcessTransportRole.MailboxSubmission:
				return "MSExchange Submission ProxyHubSelector";
			case ProcessTransportRole.MailboxDelivery:
				return "MSExchange Delivery ProxyHubSelector";
			default:
				throw new NotSupportedException("Hub Selector perf counters are not supported on role " + role);
			}
		}

		private const string InstanceName = "_total";

		private ProxyHubSelectorPerfCountersInstance instance;
	}
}
