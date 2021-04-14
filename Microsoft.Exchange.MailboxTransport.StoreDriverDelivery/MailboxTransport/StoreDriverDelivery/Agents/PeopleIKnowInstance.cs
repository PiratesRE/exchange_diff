using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class PeopleIKnowInstance : PerformanceCounterInstance
	{
		internal PeopleIKnowInstance(string instanceName, PeopleIKnowInstance autoUpdateTotalInstance) : base(instanceName, "People-I-Know Delivery Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageStopWatchTime = new ExPerformanceCounter(base.CategoryName, "Average Time to Process Message", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageStopWatchTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageStopWatchTime);
				this.AverageStopWatchTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Time to Process Message", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageStopWatchTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageStopWatchTimeBase);
				this.AverageCpuTime = new ExPerformanceCounter(base.CategoryName, "Average CPU Time to Process Message", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageCpuTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageCpuTime);
				this.AverageCpuTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average CPU Time to Process Message", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageCpuTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageCpuTimeBase);
				this.AverageStoreRPCs = new ExPerformanceCounter(base.CategoryName, "Average Store RPCs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageStoreRPCs, new ExPerformanceCounter[0]);
				list.Add(this.AverageStoreRPCs);
				this.AverageStoreRPCsBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Store RPCs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageStoreRPCsBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageStoreRPCsBase);
				long num = this.AverageStopWatchTime.RawValue;
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

		internal PeopleIKnowInstance(string instanceName) : base(instanceName, "People-I-Know Delivery Agent")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageStopWatchTime = new ExPerformanceCounter(base.CategoryName, "Average Time to Process Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageStopWatchTime);
				this.AverageStopWatchTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Time to Process Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageStopWatchTimeBase);
				this.AverageCpuTime = new ExPerformanceCounter(base.CategoryName, "Average CPU Time to Process Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageCpuTime);
				this.AverageCpuTimeBase = new ExPerformanceCounter(base.CategoryName, "Base for Average CPU Time to Process Message", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageCpuTimeBase);
				this.AverageStoreRPCs = new ExPerformanceCounter(base.CategoryName, "Average Store RPCs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageStoreRPCs);
				this.AverageStoreRPCsBase = new ExPerformanceCounter(base.CategoryName, "Base for Average Store RPCs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageStoreRPCsBase);
				long num = this.AverageStopWatchTime.RawValue;
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

		public readonly ExPerformanceCounter AverageStopWatchTime;

		public readonly ExPerformanceCounter AverageStopWatchTimeBase;

		public readonly ExPerformanceCounter AverageCpuTime;

		public readonly ExPerformanceCounter AverageCpuTimeBase;

		public readonly ExPerformanceCounter AverageStoreRPCs;

		public readonly ExPerformanceCounter AverageStoreRPCsBase;
	}
}
