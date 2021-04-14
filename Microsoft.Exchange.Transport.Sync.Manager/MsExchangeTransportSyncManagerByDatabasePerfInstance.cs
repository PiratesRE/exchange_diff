using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MsExchangeTransportSyncManagerByDatabasePerfInstance : PerformanceCounterInstance
	{
		internal MsExchangeTransportSyncManagerByDatabasePerfInstance(string instanceName, MsExchangeTransportSyncManagerByDatabasePerfInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Transport Sync Manager By Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalSubscriptionsQueuedInDatabaseQueueManager = new ExPerformanceCounter(base.CategoryName, "Database Queue Manager - Total subscriptions queued", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSubscriptionsQueuedInDatabaseQueueManager);
				this.TotalSubscriptionInstancesQueuedInDatabaseQueueManager = new ExPerformanceCounter(base.CategoryName, "Database Queue Manager - Total subscription instances queued", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSubscriptionInstancesQueuedInDatabaseQueueManager);
				long num = this.TotalSubscriptionsQueuedInDatabaseQueueManager.RawValue;
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

		internal MsExchangeTransportSyncManagerByDatabasePerfInstance(string instanceName) : base(instanceName, "MSExchange Transport Sync Manager By Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalSubscriptionsQueuedInDatabaseQueueManager = new ExPerformanceCounter(base.CategoryName, "Database Queue Manager - Total subscriptions queued", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSubscriptionsQueuedInDatabaseQueueManager);
				this.TotalSubscriptionInstancesQueuedInDatabaseQueueManager = new ExPerformanceCounter(base.CategoryName, "Database Queue Manager - Total subscription instances queued", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSubscriptionInstancesQueuedInDatabaseQueueManager);
				long num = this.TotalSubscriptionsQueuedInDatabaseQueueManager.RawValue;
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

		public readonly ExPerformanceCounter TotalSubscriptionsQueuedInDatabaseQueueManager;

		public readonly ExPerformanceCounter TotalSubscriptionInstancesQueuedInDatabaseQueueManager;
	}
}
