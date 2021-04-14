using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class MSExchangeStoreDriverDeliveryAgentInstance : PerformanceCounterInstance
	{
		internal MSExchangeStoreDriverDeliveryAgentInstance(string instanceName, MSExchangeStoreDriverDeliveryAgentInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Delivery Store Driver Agents")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliveryAgentFailures = new ExPerformanceCounter(base.CategoryName, "StoreDriverDelivery Agent Failure", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliveryAgentFailures, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryAgentFailures);
				long num = this.DeliveryAgentFailures.RawValue;
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

		internal MSExchangeStoreDriverDeliveryAgentInstance(string instanceName) : base(instanceName, "MSExchange Delivery Store Driver Agents")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliveryAgentFailures = new ExPerformanceCounter(base.CategoryName, "StoreDriverDelivery Agent Failure", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryAgentFailures);
				long num = this.DeliveryAgentFailures.RawValue;
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

		public readonly ExPerformanceCounter DeliveryAgentFailures;
	}
}
