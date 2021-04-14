using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class MSExchangeStoreDriverDatabaseInstance : PerformanceCounterInstance
	{
		internal MSExchangeStoreDriverDatabaseInstance(string instanceName, MSExchangeStoreDriverDatabaseInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Delivery Store Driver Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliveryAttempts = new ExPerformanceCounter(base.CategoryName, "Delivery attempts per minute over the last 5 minutes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliveryAttempts, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryAttempts);
				this.DeliveryFailures = new ExPerformanceCounter(base.CategoryName, "Delivery failures per minute over the last 5 minutes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliveryFailures, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryFailures);
				this.CurrentDeliveryThreadsPerMdb = new ExPerformanceCounter(base.CategoryName, "Inbound: Number of delivery threads for a given MDB", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CurrentDeliveryThreadsPerMdb, new ExPerformanceCounter[0]);
				list.Add(this.CurrentDeliveryThreadsPerMdb);
				long num = this.DeliveryAttempts.RawValue;
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

		internal MSExchangeStoreDriverDatabaseInstance(string instanceName) : base(instanceName, "MSExchange Delivery Store Driver Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliveryAttempts = new ExPerformanceCounter(base.CategoryName, "Delivery attempts per minute over the last 5 minutes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryAttempts);
				this.DeliveryFailures = new ExPerformanceCounter(base.CategoryName, "Delivery failures per minute over the last 5 minutes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliveryFailures);
				this.CurrentDeliveryThreadsPerMdb = new ExPerformanceCounter(base.CategoryName, "Inbound: Number of delivery threads for a given MDB", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentDeliveryThreadsPerMdb);
				long num = this.DeliveryAttempts.RawValue;
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

		public readonly ExPerformanceCounter DeliveryAttempts;

		public readonly ExPerformanceCounter DeliveryFailures;

		public readonly ExPerformanceCounter CurrentDeliveryThreadsPerMdb;
	}
}
