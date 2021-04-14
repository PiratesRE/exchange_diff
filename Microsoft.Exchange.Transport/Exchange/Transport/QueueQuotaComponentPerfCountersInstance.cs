using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class QueueQuotaComponentPerfCountersInstance : PerformanceCounterInstance
	{
		internal QueueQuotaComponentPerfCountersInstance(string instanceName, QueueQuotaComponentPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Queue Quota Component")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.EntitiesInThrottledState = new ExPerformanceCounter(base.CategoryName, "Entities in throttled state", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EntitiesInThrottledState, new ExPerformanceCounter[0]);
				list.Add(this.EntitiesInThrottledState);
				this.MessagesTempRejectedTotal = new ExPerformanceCounter(base.CategoryName, "Messages temp rejected total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesTempRejectedTotal, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTempRejectedTotal);
				this.MessagesTempRejectedRecently = new ExPerformanceCounter(base.CategoryName, "Messages temp rejected recently", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesTempRejectedRecently, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTempRejectedRecently);
				this.OldestThrottledEntityIntervalInSeconds = new ExPerformanceCounter(base.CategoryName, "Oldest Throttled Entity Interval In Seconds", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.OldestThrottledEntityIntervalInSeconds, new ExPerformanceCounter[0]);
				list.Add(this.OldestThrottledEntityIntervalInSeconds);
				long num = this.EntitiesInThrottledState.RawValue;
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

		internal QueueQuotaComponentPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Queue Quota Component")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.EntitiesInThrottledState = new ExPerformanceCounter(base.CategoryName, "Entities in throttled state", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.EntitiesInThrottledState);
				this.MessagesTempRejectedTotal = new ExPerformanceCounter(base.CategoryName, "Messages temp rejected total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTempRejectedTotal);
				this.MessagesTempRejectedRecently = new ExPerformanceCounter(base.CategoryName, "Messages temp rejected recently", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesTempRejectedRecently);
				this.OldestThrottledEntityIntervalInSeconds = new ExPerformanceCounter(base.CategoryName, "Oldest Throttled Entity Interval In Seconds", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.OldestThrottledEntityIntervalInSeconds);
				long num = this.EntitiesInThrottledState.RawValue;
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

		public readonly ExPerformanceCounter EntitiesInThrottledState;

		public readonly ExPerformanceCounter MessagesTempRejectedTotal;

		public readonly ExPerformanceCounter MessagesTempRejectedRecently;

		public readonly ExPerformanceCounter OldestThrottledEntityIntervalInSeconds;
	}
}
