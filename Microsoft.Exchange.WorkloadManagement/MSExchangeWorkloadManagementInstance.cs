using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal sealed class MSExchangeWorkloadManagementInstance : PerformanceCounterInstance
	{
		internal MSExchangeWorkloadManagementInstance(string instanceName, MSExchangeWorkloadManagementInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange WorkloadManagement")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.WorkloadCount = new ExPerformanceCounter(base.CategoryName, "Workload Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkloadCount);
				this.ActiveClassifications = new ExPerformanceCounter(base.CategoryName, "Active Classifications", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveClassifications);
				long num = this.WorkloadCount.RawValue;
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

		internal MSExchangeWorkloadManagementInstance(string instanceName) : base(instanceName, "MSExchange WorkloadManagement")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.WorkloadCount = new ExPerformanceCounter(base.CategoryName, "Workload Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.WorkloadCount);
				this.ActiveClassifications = new ExPerformanceCounter(base.CategoryName, "Active Classifications", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveClassifications);
				long num = this.WorkloadCount.RawValue;
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

		public readonly ExPerformanceCounter WorkloadCount;

		public readonly ExPerformanceCounter ActiveClassifications;
	}
}
