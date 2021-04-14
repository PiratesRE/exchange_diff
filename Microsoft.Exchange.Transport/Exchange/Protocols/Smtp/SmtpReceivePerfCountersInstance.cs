using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpReceivePerfCountersInstance : PerformanceCounterInstance
	{
		internal SmtpReceivePerfCountersInstance(string instanceName, SmtpReceivePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, SmtpReceivePerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsCurrent, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "TLS Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TlsConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "TLS Connections Current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TlsConnectionsCurrent, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TlsConnectionsCurrent);
				this.TlsNegotiationsFailed = new ExPerformanceCounter(base.CategoryName, "TLS Negotiations Failed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TlsNegotiationsFailed, new ExPerformanceCounter[0]);
				list.Add(this.TlsNegotiationsFailed);
				this.TlsConnectionsRejectedDueToRateExceeded = new ExPerformanceCounter(base.CategoryName, "TLS Connections Rejected Due To Rate Exceeded", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TlsConnectionsRejectedDueToRateExceeded, new ExPerformanceCounter[0]);
				list.Add(this.TlsConnectionsRejectedDueToRateExceeded);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Disconnections by Agents/second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.ConnectionsDroppedByAgentsTotal = new ExPerformanceCounter(base.CategoryName, "Disconnections By Agents", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsDroppedByAgentsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.ConnectionsDroppedByAgentsTotal);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesReceivedForNonProvisionedUsers = new ExPerformanceCounter(base.CategoryName, "Messages Received For Non-Provisioned users", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesReceivedForNonProvisionedUsers, new ExPerformanceCounter[0]);
				list.Add(this.MessagesReceivedForNonProvisionedUsers);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesRefusedForSize = new ExPerformanceCounter(base.CategoryName, "Messages Refused for Size", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesRefusedForSize, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRefusedForSize);
				this.MessagesReceivedWithBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Received Containing Bare Linefeeds in the SMTP DATA Stream", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesReceivedWithBareLinefeeds, new ExPerformanceCounter[0]);
				list.Add(this.MessagesReceivedWithBareLinefeeds);
				this.MessagesRefusedForBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Rejected During SMTP DATA Due to Bare Linefeeds", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesRefusedForBareLinefeeds, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRefusedForBareLinefeeds);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average bytes/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.MessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Message Bytes Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageBytesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter5,
					exPerformanceCounter6
				});
				list.Add(this.MessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average bytes/message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.RecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "Recipients accepted Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RecipientsAccepted, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.RecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.MessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter4,
					exPerformanceCounter7,
					exPerformanceCounter9,
					exPerformanceCounter12
				});
				list.Add(this.MessagesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter11,
					exPerformanceCounter13
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.TotalBytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesReceived, new ExPerformanceCounter[]
				{
					exPerformanceCounter10,
					exPerformanceCounter14
				});
				list.Add(this.TotalBytesReceived);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.TarpittingDelaysAuthenticated = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysAuthenticated, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.TarpittingDelaysAuthenticated);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.TarpittingDelaysAnonymous = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysAnonymous, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.TarpittingDelaysAnonymous);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.TarpittingDelaysBackpressure = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysBackpressure, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.TarpittingDelaysBackpressure);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter18 in list)
					{
						exPerformanceCounter18.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal SmtpReceivePerfCountersInstance(string instanceName) : base(instanceName, SmtpReceivePerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "TLS Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TlsConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "TLS Connections Current", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TlsConnectionsCurrent);
				this.TlsNegotiationsFailed = new ExPerformanceCounter(base.CategoryName, "TLS Negotiations Failed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TlsNegotiationsFailed);
				this.TlsConnectionsRejectedDueToRateExceeded = new ExPerformanceCounter(base.CategoryName, "TLS Connections Rejected Due To Rate Exceeded", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TlsConnectionsRejectedDueToRateExceeded);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Disconnections by Agents/second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.ConnectionsDroppedByAgentsTotal = new ExPerformanceCounter(base.CategoryName, "Disconnections By Agents", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.ConnectionsDroppedByAgentsTotal);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesReceivedForNonProvisionedUsers = new ExPerformanceCounter(base.CategoryName, "Messages Received For Non-Provisioned users", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesReceivedForNonProvisionedUsers);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesRefusedForSize = new ExPerformanceCounter(base.CategoryName, "Messages Refused for Size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRefusedForSize);
				this.MessagesReceivedWithBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Received Containing Bare Linefeeds in the SMTP DATA Stream", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesReceivedWithBareLinefeeds);
				this.MessagesRefusedForBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Rejected During SMTP DATA Due to Bare Linefeeds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesRefusedForBareLinefeeds);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average bytes/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.MessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Message Bytes Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5,
					exPerformanceCounter6
				});
				list.Add(this.MessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average bytes/message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.RecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "Recipients accepted Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.RecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.MessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4,
					exPerformanceCounter7,
					exPerformanceCounter9,
					exPerformanceCounter12
				});
				list.Add(this.MessagesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter11,
					exPerformanceCounter13
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.TotalBytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter10,
					exPerformanceCounter14
				});
				list.Add(this.TotalBytesReceived);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.TarpittingDelaysAuthenticated = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.TarpittingDelaysAuthenticated);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.TarpittingDelaysAnonymous = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.TarpittingDelaysAnonymous);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.TarpittingDelaysBackpressure = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.TarpittingDelaysBackpressure);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter18 in list)
					{
						exPerformanceCounter18.Close();
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

		public readonly ExPerformanceCounter ConnectionsCurrent;

		public readonly ExPerformanceCounter ConnectionsTotal;

		public readonly ExPerformanceCounter TlsConnectionsCurrent;

		public readonly ExPerformanceCounter TlsNegotiationsFailed;

		public readonly ExPerformanceCounter TlsConnectionsRejectedDueToRateExceeded;

		public readonly ExPerformanceCounter ConnectionsDroppedByAgentsTotal;

		public readonly ExPerformanceCounter MessagesReceivedTotal;

		public readonly ExPerformanceCounter MessagesReceivedForNonProvisionedUsers;

		public readonly ExPerformanceCounter MessageBytesReceivedTotal;

		public readonly ExPerformanceCounter MessagesRefusedForSize;

		public readonly ExPerformanceCounter MessagesReceivedWithBareLinefeeds;

		public readonly ExPerformanceCounter MessagesRefusedForBareLinefeeds;

		public readonly ExPerformanceCounter RecipientsAccepted;

		public readonly ExPerformanceCounter TotalBytesReceived;

		public readonly ExPerformanceCounter TarpittingDelaysAuthenticated;

		public readonly ExPerformanceCounter TarpittingDelaysAnonymous;

		public readonly ExPerformanceCounter TarpittingDelaysBackpressure;
	}
}
