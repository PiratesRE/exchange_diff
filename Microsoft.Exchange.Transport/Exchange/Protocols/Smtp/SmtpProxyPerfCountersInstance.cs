using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class SmtpProxyPerfCountersInstance : PerformanceCounterInstance
	{
		internal SmtpProxyPerfCountersInstance(string instanceName, SmtpProxyPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeFrontEndTransport Smtp Blind Proxy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalProxyAttempts = new ExPerformanceCounter(base.CategoryName, "Total Proxy Attempts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyAttempts);
				this.TotalSuccessfulProxySessions = new ExPerformanceCounter(base.CategoryName, "Total Successful Proxy Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSuccessfulProxySessions);
				this.PercentageProxySetupFailures = new ExPerformanceCounter(base.CategoryName, "Percentage Proxy Setup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageProxySetupFailures);
				this.TotalProxyUserLookupFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy User Lookup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyUserLookupFailures);
				this.TotalProxyBackEndLocatorFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy BackEndLocator Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyBackEndLocatorFailures);
				this.TotalProxyDnsLookupFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy Dns Lookup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyDnsLookupFailures);
				this.TotalProxyConnectionFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy Connection Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyConnectionFailures);
				this.TotalProxyProtocolErrors = new ExPerformanceCounter(base.CategoryName, "Total Proxy Protocol Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyProtocolErrors);
				this.TotalProxySocketErrors = new ExPerformanceCounter(base.CategoryName, "Total Proxy Socket Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxySocketErrors);
				this.TotalBytesProxied = new ExPerformanceCounter(base.CategoryName, "Total Bytes Proxied", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesProxied);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Messages Proxied/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.MessagesProxiedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Proxied Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.MessagesProxiedTotal);
				this.OutboundConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OutboundConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.OutboundConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.OutboundConnectionsTotal);
				this.InboundConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Inbound Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InboundConnectionsCurrent);
				long num = this.TotalProxyAttempts.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal SmtpProxyPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeFrontEndTransport Smtp Blind Proxy")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalProxyAttempts = new ExPerformanceCounter(base.CategoryName, "Total Proxy Attempts", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyAttempts);
				this.TotalSuccessfulProxySessions = new ExPerformanceCounter(base.CategoryName, "Total Successful Proxy Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSuccessfulProxySessions);
				this.PercentageProxySetupFailures = new ExPerformanceCounter(base.CategoryName, "Percentage Proxy Setup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageProxySetupFailures);
				this.TotalProxyUserLookupFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy User Lookup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyUserLookupFailures);
				this.TotalProxyBackEndLocatorFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy BackEndLocator Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyBackEndLocatorFailures);
				this.TotalProxyDnsLookupFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy Dns Lookup Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyDnsLookupFailures);
				this.TotalProxyConnectionFailures = new ExPerformanceCounter(base.CategoryName, "Total Proxy Connection Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyConnectionFailures);
				this.TotalProxyProtocolErrors = new ExPerformanceCounter(base.CategoryName, "Total Proxy Protocol Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxyProtocolErrors);
				this.TotalProxySocketErrors = new ExPerformanceCounter(base.CategoryName, "Total Proxy Socket Errors", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalProxySocketErrors);
				this.TotalBytesProxied = new ExPerformanceCounter(base.CategoryName, "Total Bytes Proxied", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesProxied);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Messages Proxied/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.MessagesProxiedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Proxied Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.MessagesProxiedTotal);
				this.OutboundConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OutboundConnectionsCurrent);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Created/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.OutboundConnectionsTotal = new ExPerformanceCounter(base.CategoryName, "Outbound Connections Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.OutboundConnectionsTotal);
				this.InboundConnectionsCurrent = new ExPerformanceCounter(base.CategoryName, "Inbound Connections Current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InboundConnectionsCurrent);
				long num = this.TotalProxyAttempts.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter3 in list)
					{
						exPerformanceCounter3.Close();
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

		public readonly ExPerformanceCounter TotalProxyAttempts;

		public readonly ExPerformanceCounter TotalSuccessfulProxySessions;

		public readonly ExPerformanceCounter PercentageProxySetupFailures;

		public readonly ExPerformanceCounter TotalProxyUserLookupFailures;

		public readonly ExPerformanceCounter TotalProxyBackEndLocatorFailures;

		public readonly ExPerformanceCounter TotalProxyDnsLookupFailures;

		public readonly ExPerformanceCounter TotalProxyConnectionFailures;

		public readonly ExPerformanceCounter TotalProxyProtocolErrors;

		public readonly ExPerformanceCounter TotalProxySocketErrors;

		public readonly ExPerformanceCounter TotalBytesProxied;

		public readonly ExPerformanceCounter MessagesProxiedTotal;

		public readonly ExPerformanceCounter OutboundConnectionsCurrent;

		public readonly ExPerformanceCounter OutboundConnectionsTotal;

		public readonly ExPerformanceCounter InboundConnectionsCurrent;
	}
}
