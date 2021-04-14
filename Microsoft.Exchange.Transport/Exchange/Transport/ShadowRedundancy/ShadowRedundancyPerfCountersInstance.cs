using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyPerfCountersInstance : PerformanceCounterInstance
	{
		internal ShadowRedundancyPerfCountersInstance(string instanceName, ShadowRedundancyPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Shadow Redundancy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RedundantMessageDiscardEvents = new ExPerformanceCounter(base.CategoryName, "Redundant Message Discard Events", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RedundantMessageDiscardEvents);
				this.RedundantMessageDiscardEventsExpired = new ExPerformanceCounter(base.CategoryName, "Redundant Message Discard Events Expired", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RedundantMessageDiscardEventsExpired);
				this.CurrentMessagesAckBeforeRelayCompleted = new ExPerformanceCounter(base.CategoryName, "Current Messages Acknowledged before Relay Completed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentMessagesAckBeforeRelayCompleted);
				this.ShadowHostSelectionAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Selection Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSelectionAverageTime);
				this.ShadowHostSelectionAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Selection Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSelectionAverageTimeBase);
				this.ShadowHostNegotiationAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Negotiation Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostNegotiationAverageTime);
				this.ShadowHostNegotiationAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Negotiation Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostNegotiationAverageTimeBase);
				this.ShadowHostSuccessfulNegotiationAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Successful Negotiation Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSuccessfulNegotiationAverageTime);
				this.ShadowHostSuccessfulNegotiationAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Successful Negotiation Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSuccessfulNegotiationAverageTimeBase);
				this.LocalSiteShadowPercentage = new ExPerformanceCounter(base.CategoryName, "Percentage of Messages Shadowed to Local Site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LocalSiteShadowPercentage);
				this.MessagesFailedToBeMadeRedundant = new ExPerformanceCounter(base.CategoryName, "Messages Failed to be made Redundant", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFailedToBeMadeRedundant);
				this.MessagesFailedToBeMadeRedundantPercentage = new ExPerformanceCounter(base.CategoryName, "Percentage of Messages Failed to be made Redundant", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFailedToBeMadeRedundantPercentage);
				this.TotalSmtpTimeouts = new ExPerformanceCounter(base.CategoryName, "Total SMTP Timeouts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSmtpTimeouts);
				this.ClientAckFailureCount = new ExPerformanceCounter(base.CategoryName, "Client ACK Failure Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ClientAckFailureCount);
				long num = this.RedundantMessageDiscardEvents.RawValue;
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

		internal ShadowRedundancyPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Shadow Redundancy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RedundantMessageDiscardEvents = new ExPerformanceCounter(base.CategoryName, "Redundant Message Discard Events", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RedundantMessageDiscardEvents);
				this.RedundantMessageDiscardEventsExpired = new ExPerformanceCounter(base.CategoryName, "Redundant Message Discard Events Expired", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RedundantMessageDiscardEventsExpired);
				this.CurrentMessagesAckBeforeRelayCompleted = new ExPerformanceCounter(base.CategoryName, "Current Messages Acknowledged before Relay Completed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentMessagesAckBeforeRelayCompleted);
				this.ShadowHostSelectionAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Selection Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSelectionAverageTime);
				this.ShadowHostSelectionAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Selection Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSelectionAverageTimeBase);
				this.ShadowHostNegotiationAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Negotiation Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostNegotiationAverageTime);
				this.ShadowHostNegotiationAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Negotiation Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostNegotiationAverageTimeBase);
				this.ShadowHostSuccessfulNegotiationAverageTime = new ExPerformanceCounter(base.CategoryName, "Shadow Host Successful Negotiation Average Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSuccessfulNegotiationAverageTime);
				this.ShadowHostSuccessfulNegotiationAverageTimeBase = new ExPerformanceCounter(base.CategoryName, "Shadow Host Successful Negotiation Average Time Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowHostSuccessfulNegotiationAverageTimeBase);
				this.LocalSiteShadowPercentage = new ExPerformanceCounter(base.CategoryName, "Percentage of Messages Shadowed to Local Site", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LocalSiteShadowPercentage);
				this.MessagesFailedToBeMadeRedundant = new ExPerformanceCounter(base.CategoryName, "Messages Failed to be made Redundant", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFailedToBeMadeRedundant);
				this.MessagesFailedToBeMadeRedundantPercentage = new ExPerformanceCounter(base.CategoryName, "Percentage of Messages Failed to be made Redundant", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFailedToBeMadeRedundantPercentage);
				this.TotalSmtpTimeouts = new ExPerformanceCounter(base.CategoryName, "Total SMTP Timeouts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSmtpTimeouts);
				this.ClientAckFailureCount = new ExPerformanceCounter(base.CategoryName, "Client ACK Failure Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ClientAckFailureCount);
				long num = this.RedundantMessageDiscardEvents.RawValue;
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

		public readonly ExPerformanceCounter RedundantMessageDiscardEvents;

		public readonly ExPerformanceCounter RedundantMessageDiscardEventsExpired;

		public readonly ExPerformanceCounter CurrentMessagesAckBeforeRelayCompleted;

		public readonly ExPerformanceCounter ShadowHostSelectionAverageTime;

		public readonly ExPerformanceCounter ShadowHostSelectionAverageTimeBase;

		public readonly ExPerformanceCounter ShadowHostNegotiationAverageTime;

		public readonly ExPerformanceCounter ShadowHostNegotiationAverageTimeBase;

		public readonly ExPerformanceCounter ShadowHostSuccessfulNegotiationAverageTime;

		public readonly ExPerformanceCounter ShadowHostSuccessfulNegotiationAverageTimeBase;

		public readonly ExPerformanceCounter LocalSiteShadowPercentage;

		public readonly ExPerformanceCounter MessagesFailedToBeMadeRedundant;

		public readonly ExPerformanceCounter MessagesFailedToBeMadeRedundantPercentage;

		public readonly ExPerformanceCounter TotalSmtpTimeouts;

		public readonly ExPerformanceCounter ClientAckFailureCount;
	}
}
