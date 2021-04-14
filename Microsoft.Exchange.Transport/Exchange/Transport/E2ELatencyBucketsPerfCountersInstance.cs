using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class E2ELatencyBucketsPerfCountersInstance : PerformanceCounterInstance
	{
		internal E2ELatencyBucketsPerfCountersInstance(string instanceName, E2ELatencyBucketsPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport E2E Latency Buckets")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliverHighPriority = new ExPerformanceCounter(base.CategoryName, "Deliver High Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliverHighPriority, new ExPerformanceCounter[0]);
				list.Add(this.DeliverHighPriority);
				this.DeliverNormalPriority = new ExPerformanceCounter(base.CategoryName, "Deliver Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliverNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.DeliverNormalPriority);
				this.DeliverLowPriority = new ExPerformanceCounter(base.CategoryName, "Deliver Low Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliverLowPriority, new ExPerformanceCounter[0]);
				list.Add(this.DeliverLowPriority);
				this.DeliverNonePriority = new ExPerformanceCounter(base.CategoryName, "Deliver None Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliverNonePriority, new ExPerformanceCounter[0]);
				list.Add(this.DeliverNonePriority);
				this.SendToExternalHighPriority = new ExPerformanceCounter(base.CategoryName, "Send To External High Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SendToExternalHighPriority, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalHighPriority);
				this.SendToExternalNormalPriority = new ExPerformanceCounter(base.CategoryName, "Send To External Normal Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SendToExternalNormalPriority, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalNormalPriority);
				this.SendToExternalLowPriority = new ExPerformanceCounter(base.CategoryName, "Send To External Low Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SendToExternalLowPriority, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalLowPriority);
				this.SendToExternalNonePriority = new ExPerformanceCounter(base.CategoryName, "Send To External None Priority", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SendToExternalNonePriority, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalNonePriority);
				long num = this.DeliverHighPriority.RawValue;
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

		internal E2ELatencyBucketsPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport E2E Latency Buckets")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DeliverHighPriority = new ExPerformanceCounter(base.CategoryName, "Deliver High Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverHighPriority);
				this.DeliverNormalPriority = new ExPerformanceCounter(base.CategoryName, "Deliver Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverNormalPriority);
				this.DeliverLowPriority = new ExPerformanceCounter(base.CategoryName, "Deliver Low Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverLowPriority);
				this.DeliverNonePriority = new ExPerformanceCounter(base.CategoryName, "Deliver None Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliverNonePriority);
				this.SendToExternalHighPriority = new ExPerformanceCounter(base.CategoryName, "Send To External High Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalHighPriority);
				this.SendToExternalNormalPriority = new ExPerformanceCounter(base.CategoryName, "Send To External Normal Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalNormalPriority);
				this.SendToExternalLowPriority = new ExPerformanceCounter(base.CategoryName, "Send To External Low Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalLowPriority);
				this.SendToExternalNonePriority = new ExPerformanceCounter(base.CategoryName, "Send To External None Priority", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SendToExternalNonePriority);
				long num = this.DeliverHighPriority.RawValue;
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

		public readonly ExPerformanceCounter DeliverHighPriority;

		public readonly ExPerformanceCounter DeliverNormalPriority;

		public readonly ExPerformanceCounter DeliverLowPriority;

		public readonly ExPerformanceCounter DeliverNonePriority;

		public readonly ExPerformanceCounter SendToExternalHighPriority;

		public readonly ExPerformanceCounter SendToExternalNormalPriority;

		public readonly ExPerformanceCounter SendToExternalLowPriority;

		public readonly ExPerformanceCounter SendToExternalNonePriority;
	}
}
