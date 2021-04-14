using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class InboundProxyAccountForestPerfCountersInstance : PerformanceCounterInstance
	{
		internal InboundProxyAccountForestPerfCountersInstance(string instanceName, InboundProxyAccountForestPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeFrontendTransport InboundProxyEXOAccountForests")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsCurrent, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.InboundMessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessageBytesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter4
				});
				list.Add(this.InboundMessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average recipients/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.InboundRecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "Inbound Recipients accepted Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundRecipientsAccepted, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.InboundRecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average Recipients/Inbound Message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.InboundMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InboundMessagesReceivedTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter5,
					exPerformanceCounter7
				});
				list.Add(this.InboundMessagesReceivedTotal);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter8 in list)
					{
						exPerformanceCounter8.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal InboundProxyAccountForestPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeFrontendTransport InboundProxyEXOAccountForests")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ConnectionsTotal);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.InboundMessageBytesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Message Bytes Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter4
				});
				list.Add(this.InboundMessageBytesReceivedTotal);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Average bytes/inbound message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average recipients/inbound message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.InboundRecipientsAccepted = new ExPerformanceCounter(base.CategoryName, "Inbound Recipients accepted Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.InboundRecipientsAccepted);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average Recipients/Inbound Message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.InboundMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Inbound Messages Received Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter5,
					exPerformanceCounter7
				});
				list.Add(this.InboundMessagesReceivedTotal);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter8 in list)
					{
						exPerformanceCounter8.Close();
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

		public readonly ExPerformanceCounter InboundMessagesReceivedTotal;

		public readonly ExPerformanceCounter InboundMessageBytesReceivedTotal;

		public readonly ExPerformanceCounter InboundRecipientsAccepted;
	}
}
