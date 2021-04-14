using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class ProxyHubSelectorPerfCountersInstance : PerformanceCounterInstance
	{
		internal ProxyHubSelectorPerfCountersInstance(string instanceName, ProxyHubSelectorPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, ProxyHubSelectorPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.HubSelectionRequestsTotal = new ExPerformanceCounter(base.CategoryName, "Hub Selection Requests Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionRequestsTotal);
				this.HubSelectionResolverFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Resolver Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionResolverFailures);
				this.HubSelectionOrganizationMailboxFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Organization Mailbox Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionOrganizationMailboxFailures);
				this.HubSelectionMessagesWithoutMailboxRecipients = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Without Mailbox Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesWithoutMailboxRecipients);
				this.HubSelectionMessagesWithoutOrganizationMailboxes = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Without Organization Mailboxes", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesWithoutOrganizationMailboxes);
				this.HubSelectionMessagesRoutedUsingDagSelector = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Routed Using Dag Selector", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesRoutedUsingDagSelector);
				this.HubSelectionFallbackRoutingRequests = new ExPerformanceCounter(base.CategoryName, "Hub Selection Fallback Routing Requests", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionFallbackRoutingRequests);
				this.HubSelectionRoutingFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Routing Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionRoutingFailures);
				long num = this.HubSelectionRequestsTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal ProxyHubSelectorPerfCountersInstance(string instanceName) : base(instanceName, ProxyHubSelectorPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.HubSelectionRequestsTotal = new ExPerformanceCounter(base.CategoryName, "Hub Selection Requests Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionRequestsTotal);
				this.HubSelectionResolverFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Resolver Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionResolverFailures);
				this.HubSelectionOrganizationMailboxFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Organization Mailbox Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionOrganizationMailboxFailures);
				this.HubSelectionMessagesWithoutMailboxRecipients = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Without Mailbox Recipients", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesWithoutMailboxRecipients);
				this.HubSelectionMessagesWithoutOrganizationMailboxes = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Without Organization Mailboxes", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesWithoutOrganizationMailboxes);
				this.HubSelectionMessagesRoutedUsingDagSelector = new ExPerformanceCounter(base.CategoryName, "Hub Selection Messages Routed Using Dag Selector", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionMessagesRoutedUsingDagSelector);
				this.HubSelectionFallbackRoutingRequests = new ExPerformanceCounter(base.CategoryName, "Hub Selection Fallback Routing Requests", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionFallbackRoutingRequests);
				this.HubSelectionRoutingFailures = new ExPerformanceCounter(base.CategoryName, "Hub Selection Routing Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.HubSelectionRoutingFailures);
				long num = this.HubSelectionRequestsTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter HubSelectionRequestsTotal;

		public readonly ExPerformanceCounter HubSelectionResolverFailures;

		public readonly ExPerformanceCounter HubSelectionOrganizationMailboxFailures;

		public readonly ExPerformanceCounter HubSelectionMessagesWithoutMailboxRecipients;

		public readonly ExPerformanceCounter HubSelectionMessagesWithoutOrganizationMailboxes;

		public readonly ExPerformanceCounter HubSelectionMessagesRoutedUsingDagSelector;

		public readonly ExPerformanceCounter HubSelectionFallbackRoutingRequests;

		public readonly ExPerformanceCounter HubSelectionRoutingFailures;
	}
}
