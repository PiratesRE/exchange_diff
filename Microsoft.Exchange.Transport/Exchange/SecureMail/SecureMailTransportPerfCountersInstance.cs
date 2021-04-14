using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SecureMail
{
	internal sealed class SecureMailTransportPerfCountersInstance : PerformanceCounterInstance
	{
		internal SecureMailTransportPerfCountersInstance(string instanceName, SecureMailTransportPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Secure Mail Transport")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DomainSecureMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Messages Received", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureMessagesReceivedTotal);
				this.DomainSecureMessagesSentTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Messages Sent", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureMessagesSentTotal);
				this.DomainSecureOutboundSessionFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Outbound Session Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureOutboundSessionFailuresTotal);
				long num = this.DomainSecureMessagesReceivedTotal.RawValue;
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

		internal SecureMailTransportPerfCountersInstance(string instanceName) : base(instanceName, "MSExchange Secure Mail Transport")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DomainSecureMessagesReceivedTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Messages Received", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureMessagesReceivedTotal);
				this.DomainSecureMessagesSentTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Messages Sent", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureMessagesSentTotal);
				this.DomainSecureOutboundSessionFailuresTotal = new ExPerformanceCounter(base.CategoryName, "Domain Secure Outbound Session Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DomainSecureOutboundSessionFailuresTotal);
				long num = this.DomainSecureMessagesReceivedTotal.RawValue;
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

		public readonly ExPerformanceCounter DomainSecureMessagesReceivedTotal;

		public readonly ExPerformanceCounter DomainSecureMessagesSentTotal;

		public readonly ExPerformanceCounter DomainSecureOutboundSessionFailuresTotal;
	}
}
