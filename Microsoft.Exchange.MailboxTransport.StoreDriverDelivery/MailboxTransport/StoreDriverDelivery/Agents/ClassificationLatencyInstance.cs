using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class ClassificationLatencyInstance : PerformanceCounterInstance
	{
		internal ClassificationLatencyInstance(string instanceName, ClassificationLatencyInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeInference Classification Latency")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfItemsProcessedIn0to100Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 0-100 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn0to100Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn0to100Milliseconds);
				this.NumberOfItemsProcessedIn100to200Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 100-200 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn100to200Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn100to200Milliseconds);
				this.NumberOfItemsProcessedIn200to500Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 200-500 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn200to500Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn200to500Milliseconds);
				this.NumberOfItemsProcessedIn500to750Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 500-750 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn500to750Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn500to750Milliseconds);
				this.NumberOfItemsProcessedIn750to1000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 750-1000 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn750to1000Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn750to1000Milliseconds);
				this.NumberOfItemsProcessedIn1000to5000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 1000-5000 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedIn1000to5000Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn1000to5000Milliseconds);
				this.NumberOfItemsProcessedInGreaterThan5000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed in greater than 5000 ms", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsProcessedInGreaterThan5000Milliseconds, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedInGreaterThan5000Milliseconds);
				long num = this.NumberOfItemsProcessedIn0to100Milliseconds.RawValue;
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

		internal ClassificationLatencyInstance(string instanceName) : base(instanceName, "MSExchangeInference Classification Latency")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfItemsProcessedIn0to100Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 0-100 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn0to100Milliseconds);
				this.NumberOfItemsProcessedIn100to200Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 100-200 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn100to200Milliseconds);
				this.NumberOfItemsProcessedIn200to500Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 200-500 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn200to500Milliseconds);
				this.NumberOfItemsProcessedIn500to750Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 500-750 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn500to750Milliseconds);
				this.NumberOfItemsProcessedIn750to1000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 750-1000 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn750to1000Milliseconds);
				this.NumberOfItemsProcessedIn1000to5000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed 1000-5000 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedIn1000to5000Milliseconds);
				this.NumberOfItemsProcessedInGreaterThan5000Milliseconds = new ExPerformanceCounter(base.CategoryName, "Items processed in greater than 5000 ms", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsProcessedInGreaterThan5000Milliseconds);
				long num = this.NumberOfItemsProcessedIn0to100Milliseconds.RawValue;
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

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn0to100Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn100to200Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn200to500Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn500to750Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn750to1000Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedIn1000to5000Milliseconds;

		public readonly ExPerformanceCounter NumberOfItemsProcessedInGreaterThan5000Milliseconds;
	}
}
