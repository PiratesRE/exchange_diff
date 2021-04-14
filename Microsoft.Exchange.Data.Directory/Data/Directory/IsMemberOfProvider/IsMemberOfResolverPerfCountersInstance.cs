using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.IsMemberOfProvider
{
	internal sealed class IsMemberOfResolverPerfCountersInstance : PerformanceCounterInstance
	{
		internal IsMemberOfResolverPerfCountersInstance(string instanceName, IsMemberOfResolverPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "Expanded Groups Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ResolvedGroupsHitCount = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsHitCount);
				this.ResolvedGroupsMissCount = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsMissCount);
				this.ResolvedGroupsCacheSize = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsCacheSize);
				this.ResolvedGroupsCacheSizePercentage = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Size Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsCacheSizePercentage);
				this.ExpandedGroupsHitCount = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsHitCount);
				this.ExpandedGroupsMissCount = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsMissCount);
				this.ExpandedGroupsCacheSize = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsCacheSize);
				this.ExpandedGroupsCacheSizePercentage = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Size Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsCacheSizePercentage);
				this.LdapQueries = new ExPerformanceCounter(base.CategoryName, "LDAP Queries Issued by Expanded Groups.", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LdapQueries);
				long num = this.ResolvedGroupsHitCount.RawValue;
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

		internal IsMemberOfResolverPerfCountersInstance(string instanceName) : base(instanceName, "Expanded Groups Cache")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ResolvedGroupsHitCount = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsHitCount);
				this.ResolvedGroupsMissCount = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsMissCount);
				this.ResolvedGroupsCacheSize = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsCacheSize);
				this.ResolvedGroupsCacheSizePercentage = new ExPerformanceCounter(base.CategoryName, "Resolved Groups Cache Size Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ResolvedGroupsCacheSizePercentage);
				this.ExpandedGroupsHitCount = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Hit Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsHitCount);
				this.ExpandedGroupsMissCount = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Miss Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsMissCount);
				this.ExpandedGroupsCacheSize = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Size", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsCacheSize);
				this.ExpandedGroupsCacheSizePercentage = new ExPerformanceCounter(base.CategoryName, "Expanded Groups Cache Size Percentage", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedGroupsCacheSizePercentage);
				this.LdapQueries = new ExPerformanceCounter(base.CategoryName, "LDAP Queries Issued by Expanded Groups.", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LdapQueries);
				long num = this.ResolvedGroupsHitCount.RawValue;
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

		public readonly ExPerformanceCounter ResolvedGroupsHitCount;

		public readonly ExPerformanceCounter ResolvedGroupsMissCount;

		public readonly ExPerformanceCounter ResolvedGroupsCacheSize;

		public readonly ExPerformanceCounter ResolvedGroupsCacheSizePercentage;

		public readonly ExPerformanceCounter ExpandedGroupsHitCount;

		public readonly ExPerformanceCounter ExpandedGroupsMissCount;

		public readonly ExPerformanceCounter ExpandedGroupsCacheSize;

		public readonly ExPerformanceCounter ExpandedGroupsCacheSizePercentage;

		public readonly ExPerformanceCounter LdapQueries;
	}
}
