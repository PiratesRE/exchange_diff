using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpSendPerfCountersInstance : PerformanceCounterInstance
	{
		internal SmtpSendPerfCountersInstance(string instanceName, SmtpSendPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, SmtpSendPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsCurrent, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Messages Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MessagesSuppressedDueToBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Suppressed Due to Bare Linefeeds", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesSuppressedDueToBareLinefeeds, new ExPerformanceCounter[0]);
				list.Add(this.MessagesSuppressedDueToBareLinefeeds);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Bytes Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.TotalRecipientsSent = new ExPerformanceCounter(base.CategoryName, "Recipients sent", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalRecipientsSent, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TotalRecipientsSent);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average recipients (message base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average message bytes/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.MessageBytesSentTotal = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessageBytesSentTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter7
				});
				list.Add(this.MessageBytesSentTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average message bytes/message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.MessagesSentTotal = new ExPerformanceCounter(base.CategoryName, "Messages Sent Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesSentTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter6,
					exPerformanceCounter8,
					exPerformanceCounter9
				});
				list.Add(this.MessagesSentTotal);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average messages (connection Base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.TotalBytesSent = new ExPerformanceCounter(base.CategoryName, "Bytes Sent Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesSent, new ExPerformanceCounter[]
				{
					exPerformanceCounter4,
					exPerformanceCounter11
				});
				list.Add(this.TotalBytesSent);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average bytes (connection Base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionsTotal, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter10,
					exPerformanceCounter12
				});
				list.Add(this.ConnectionsTotal);
				this.DnsErrors = new ExPerformanceCounter(base.CategoryName, "DNS Errors", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DnsErrors, new ExPerformanceCounter[0]);
				list.Add(this.DnsErrors);
				this.ConnectionFailures = new ExPerformanceCounter(base.CategoryName, "Connection Failures", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ConnectionFailures, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionFailures);
				this.SocketErrors = new ExPerformanceCounter(base.CategoryName, "Socket Errors", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SocketErrors, new ExPerformanceCounter[0]);
				list.Add(this.SocketErrors);
				this.ProtocolErrors = new ExPerformanceCounter(base.CategoryName, "Protocol Errors", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ProtocolErrors, new ExPerformanceCounter[0]);
				list.Add(this.ProtocolErrors);
				this.TlsNegotiationsFailed = new ExPerformanceCounter(base.CategoryName, "TLS Negotiations Failed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TlsNegotiationsFailed, new ExPerformanceCounter[0]);
				list.Add(this.TlsNegotiationsFailed);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter13 in list)
					{
						exPerformanceCounter13.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal SmtpSendPerfCountersInstance(string instanceName) : base(instanceName, SmtpSendPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Messages Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MessagesSuppressedDueToBareLinefeeds = new ExPerformanceCounter(base.CategoryName, "Messages Suppressed Due to Bare Linefeeds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesSuppressedDueToBareLinefeeds);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Bytes Sent/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Average recipients/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.TotalRecipientsSent = new ExPerformanceCounter(base.CategoryName, "Recipients sent", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TotalRecipientsSent);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Average recipients (message base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Average message bytes/message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.MessageBytesSentTotal = new ExPerformanceCounter(base.CategoryName, "Message Bytes Sent Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3,
					exPerformanceCounter7
				});
				list.Add(this.MessageBytesSentTotal);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Average message bytes/message Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "Average messages/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.MessagesSentTotal = new ExPerformanceCounter(base.CategoryName, "Messages Sent Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2,
					exPerformanceCounter6,
					exPerformanceCounter8,
					exPerformanceCounter9
				});
				list.Add(this.MessagesSentTotal);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "Average messages (connection Base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "Average bytes/connection", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.TotalBytesSent = new ExPerformanceCounter(base.CategoryName, "Bytes Sent Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4,
					exPerformanceCounter11
				});
				list.Add(this.TotalBytesSent);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "Average bytes (connection Base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.ConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter,
					exPerformanceCounter10,
					exPerformanceCounter12
				});
				list.Add(this.ConnectionsTotal);
				this.DnsErrors = new ExPerformanceCounter(base.CategoryName, "DNS Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DnsErrors);
				this.ConnectionFailures = new ExPerformanceCounter(base.CategoryName, "Connection Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ConnectionFailures);
				this.SocketErrors = new ExPerformanceCounter(base.CategoryName, "Socket Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SocketErrors);
				this.ProtocolErrors = new ExPerformanceCounter(base.CategoryName, "Protocol Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ProtocolErrors);
				this.TlsNegotiationsFailed = new ExPerformanceCounter(base.CategoryName, "TLS Negotiations Failed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TlsNegotiationsFailed);
				long num = this.ConnectionsCurrent.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter13 in list)
					{
						exPerformanceCounter13.Close();
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

		public readonly ExPerformanceCounter MessagesSentTotal;

		public readonly ExPerformanceCounter MessageBytesSentTotal;

		public readonly ExPerformanceCounter MessagesSuppressedDueToBareLinefeeds;

		public readonly ExPerformanceCounter TotalBytesSent;

		public readonly ExPerformanceCounter TotalRecipientsSent;

		public readonly ExPerformanceCounter DnsErrors;

		public readonly ExPerformanceCounter ConnectionFailures;

		public readonly ExPerformanceCounter SocketErrors;

		public readonly ExPerformanceCounter ProtocolErrors;

		public readonly ExPerformanceCounter TlsNegotiationsFailed;
	}
}
