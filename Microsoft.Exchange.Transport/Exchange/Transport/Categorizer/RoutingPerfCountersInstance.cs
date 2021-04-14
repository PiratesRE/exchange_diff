using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class RoutingPerfCountersInstance : PerformanceCounterInstance
	{
		internal RoutingPerfCountersInstance(string instanceName, RoutingPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, RoutingPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RoutingNdrsTotal = new ExPerformanceCounter(base.CategoryName, "Routing NDRs Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingNdrsTotal);
				this.RoutingTablesCalculatedTotal = new ExPerformanceCounter(base.CategoryName, "Routing Tables Calculated Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingTablesCalculatedTotal);
				this.RoutingTablesChangedTotal = new ExPerformanceCounter(base.CategoryName, "Routing Tables Changed Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingTablesChangedTotal);
				long num = this.RoutingNdrsTotal.RawValue;
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

		internal RoutingPerfCountersInstance(string instanceName) : base(instanceName, RoutingPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.RoutingNdrsTotal = new ExPerformanceCounter(base.CategoryName, "Routing NDRs Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingNdrsTotal);
				this.RoutingTablesCalculatedTotal = new ExPerformanceCounter(base.CategoryName, "Routing Tables Calculated Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingTablesCalculatedTotal);
				this.RoutingTablesChangedTotal = new ExPerformanceCounter(base.CategoryName, "Routing Tables Changed Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RoutingTablesChangedTotal);
				long num = this.RoutingNdrsTotal.RawValue;
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

		public readonly ExPerformanceCounter RoutingNdrsTotal;

		public readonly ExPerformanceCounter RoutingTablesCalculatedTotal;

		public readonly ExPerformanceCounter RoutingTablesChangedTotal;
	}
}
