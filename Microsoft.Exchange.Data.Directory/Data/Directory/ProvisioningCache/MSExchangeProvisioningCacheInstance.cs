using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ProvisioningCache
{
	internal sealed class MSExchangeProvisioningCacheInstance : PerformanceCounterInstance
	{
		internal MSExchangeProvisioningCacheInstance(string instanceName, MSExchangeProvisioningCacheInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Provisioning Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.GlobalAggregateHits = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache global data aggregate hits.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GlobalAggregateHits);
				this.GlobalAggregateMisses = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache global data aggregate misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GlobalAggregateMisses);
				this.OrganizationAggregateHits = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache organization data aggregate hits.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationAggregateHits);
				this.OrganizationAggregateMisses = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache organization data aggregate misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationAggregateMisses);
				this.ReceivedInvalidationMsgNum = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache Received Invalidation Msg Counter.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReceivedInvalidationMsgNum);
				this.TotalCachedObjectNum = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache Total Cached Object Counter.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCachedObjectNum);
				long num = this.GlobalAggregateHits.RawValue;
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

		internal MSExchangeProvisioningCacheInstance(string instanceName) : base(instanceName, "MSExchange Provisioning Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.GlobalAggregateHits = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache global data aggregate hits.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GlobalAggregateHits);
				this.GlobalAggregateMisses = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache global data aggregate misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GlobalAggregateMisses);
				this.OrganizationAggregateHits = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache organization data aggregate hits.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationAggregateHits);
				this.OrganizationAggregateMisses = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache organization data aggregate misses.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OrganizationAggregateMisses);
				this.ReceivedInvalidationMsgNum = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache Received Invalidation Msg Counter.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReceivedInvalidationMsgNum);
				this.TotalCachedObjectNum = new ExPerformanceCounter(base.CategoryName, "Provisioning Cache Total Cached Object Counter.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCachedObjectNum);
				long num = this.GlobalAggregateHits.RawValue;
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

		public readonly ExPerformanceCounter GlobalAggregateHits;

		public readonly ExPerformanceCounter GlobalAggregateMisses;

		public readonly ExPerformanceCounter OrganizationAggregateHits;

		public readonly ExPerformanceCounter OrganizationAggregateMisses;

		public readonly ExPerformanceCounter ReceivedInvalidationMsgNum;

		public readonly ExPerformanceCounter TotalCachedObjectNum;
	}
}
