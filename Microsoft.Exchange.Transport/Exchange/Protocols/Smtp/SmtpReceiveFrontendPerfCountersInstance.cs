using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpReceiveFrontendPerfCountersInstance : PerformanceCounterInstance
	{
		internal SmtpReceiveFrontendPerfCountersInstance(string instanceName, SmtpReceiveFrontendPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeFrontEndTransport SMTPReceive")
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
				this.InboundMessageConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Inbound Message Connections Current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessageConnectionsCurrent, new ExPerformanceCounter[0]);
				list.Add(this.InboundMessageConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec for inbound proxy messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Disconnections by Agents/second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.ConnectionsDroppedByAgentsTotal = new ExPerformanceCounter(base.CategoryName, "Disconnections By Agents", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsDroppedByAgentsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.ConnectionsDroppedByAgentsTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.InboundMessagesRefusedForSize = new ExPerformanceCounter(base.CategoryName, "InboundMessages Refused for Size", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessagesRefusedForSize, new ExPerformanceCounter[0]);
				list.Add(this.InboundMessagesRefusedForSize);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.InboundMessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessageBytesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter6,
					exPerformanceCounter7
				});
				list.Add(this.InboundMessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average recipients/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.InboundRecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "InboundRecipients accepted Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundRecipientsAccepted, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.InboundRecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average Recipients/Inbound Message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter12
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average inbound messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.InboundMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessagesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter5,
					exPerformanceCounter8,
					exPerformanceCounter10,
					exPerformanceCounter13
				});
				list.Add(this.InboundMessagesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Average inbound messages/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.InboundMessageConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Connections Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessageConnectionsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter14
				});
				list.Add(this.InboundMessageConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.TotalBytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesReceived, new ExPerformanceCounter[]
				{
					exPerformanceCounter11,
					exPerformanceCounter15
				});
				list.Add(this.TotalBytesReceived);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.TarpittingDelaysAuthenticated = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysAuthenticated, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.TarpittingDelaysAuthenticated);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.TarpittingDelaysAnonymous = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysAnonymous, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.TarpittingDelaysAnonymous);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.TarpittingDelaysBackpressure = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TarpittingDelaysBackpressure, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
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
					foreach (ExPerformanceCounter exPerformanceCounter19 in list)
					{
						exPerformanceCounter19.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal SmtpReceiveFrontendPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeFrontEndTransport SMTPReceive")
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
				this.InboundMessageConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Inbound Message Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InboundMessageConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec for inbound proxy messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Disconnections by Agents/second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.ConnectionsDroppedByAgentsTotal = new ExPerformanceCounter(base.CategoryName, "Disconnections By Agents", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.ConnectionsDroppedByAgentsTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.InboundMessagesRefusedForSize = new ExPerformanceCounter(base.CategoryName, "InboundMessages Refused for Size", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InboundMessagesRefusedForSize);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.InboundMessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6,
					exPerformanceCounter7
				});
				list.Add(this.InboundMessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average recipients/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.InboundRecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "InboundRecipients accepted Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.InboundRecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average Recipients/Inbound Message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter12
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "Average inbound messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.InboundMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5,
					exPerformanceCounter8,
					exPerformanceCounter10,
					exPerformanceCounter13
				});
				list.Add(this.InboundMessagesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "Average inbound messages/connection base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.InboundMessageConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter14
				});
				list.Add(this.InboundMessageConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.TotalBytesReceived = new ExPerformanceCounter(base.CategoryName, "Bytes Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter11,
					exPerformanceCounter15
				});
				list.Add(this.TotalBytesReceived);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.TarpittingDelaysAuthenticated = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Authenticated", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.TarpittingDelaysAuthenticated);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.TarpittingDelaysAnonymous = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Anonymous", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.TarpittingDelaysAnonymous);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.TarpittingDelaysBackpressure = new ExPerformanceCounter(base.CategoryName, "Tarpitting Delays Backpressure", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
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
					foreach (ExPerformanceCounter exPerformanceCounter19 in list)
					{
						exPerformanceCounter19.Close();
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

		public readonly ExPerformanceCounter InboundMessageConnectionsCurrent;

		public readonly ExPerformanceCounter InboundMessageConnectionsTotal;

		public readonly ExPerformanceCounter ConnectionsDroppedByAgentsTotal;

		public readonly ExPerformanceCounter InboundMessagesReceivedTotal;

		public readonly ExPerformanceCounter InboundMessageBytesReceivedTotal;

		public readonly ExPerformanceCounter InboundMessagesRefusedForSize;

		public readonly ExPerformanceCounter InboundRecipientsAccepted;

		public readonly ExPerformanceCounter TotalBytesReceived;

		public readonly ExPerformanceCounter TarpittingDelaysAuthenticated;

		public readonly ExPerformanceCounter TarpittingDelaysAnonymous;

		public readonly ExPerformanceCounter TarpittingDelaysBackpressure;
	}
}
