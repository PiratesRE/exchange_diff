using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DsnGeneratorPerfCountersInstance : PerformanceCounterInstance
	{
		internal DsnGeneratorPerfCountersInstance(string instanceName, DsnGeneratorPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, DsnGeneratorPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.FailedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Failure DSNs Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailedDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.FailedDsnTotal);
				this.DelayedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Delay DSNs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DelayedDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.DelayedDsnTotal);
				this.RelayedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Relay DSNs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RelayedDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.RelayedDsnTotal);
				this.DeliveredDsnTotal = new ExPerformanceCounter(base.CategoryName, "Delivered DSNs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeliveredDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.DeliveredDsnTotal);
				this.ExpandedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Expanded DSNs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExpandedDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedDsnTotal);
				this.FailedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Failure DSNs within the last hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailedDsnInLastHour, new ExPerformanceCounter[0]);
				list.Add(this.FailedDsnInLastHour);
				this.AlertableFailedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Alertable failure DSNs within the last hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AlertableFailedDsnInLastHour, new ExPerformanceCounter[0]);
				list.Add(this.AlertableFailedDsnInLastHour);
				this.DelayedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Delayed DSNs within the last hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DelayedDsnInLastHour, new ExPerformanceCounter[0]);
				list.Add(this.DelayedDsnInLastHour);
				this.CatchAllRecipientFailedDsnTotal = new ExPerformanceCounter(base.CategoryName, "NDRs generated for catch-all recipients.", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CatchAllRecipientFailedDsnTotal, new ExPerformanceCounter[0]);
				list.Add(this.CatchAllRecipientFailedDsnTotal);
				long num = this.FailedDsnTotal.RawValue;
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

		internal DsnGeneratorPerfCountersInstance(string instanceName) : base(instanceName, DsnGeneratorPerfCounters.CategoryName)
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.FailedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Failure DSNs Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedDsnTotal);
				this.DelayedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Delay DSNs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayedDsnTotal);
				this.RelayedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Relay DSNs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RelayedDsnTotal);
				this.DeliveredDsnTotal = new ExPerformanceCounter(base.CategoryName, "Delivered DSNs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DeliveredDsnTotal);
				this.ExpandedDsnTotal = new ExPerformanceCounter(base.CategoryName, "Expanded DSNs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExpandedDsnTotal);
				this.FailedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Failure DSNs within the last hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedDsnInLastHour);
				this.AlertableFailedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Alertable failure DSNs within the last hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AlertableFailedDsnInLastHour);
				this.DelayedDsnInLastHour = new ExPerformanceCounter(base.CategoryName, "Delayed DSNs within the last hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayedDsnInLastHour);
				this.CatchAllRecipientFailedDsnTotal = new ExPerformanceCounter(base.CategoryName, "NDRs generated for catch-all recipients.", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CatchAllRecipientFailedDsnTotal);
				long num = this.FailedDsnTotal.RawValue;
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

		public readonly ExPerformanceCounter FailedDsnTotal;

		public readonly ExPerformanceCounter DelayedDsnTotal;

		public readonly ExPerformanceCounter RelayedDsnTotal;

		public readonly ExPerformanceCounter DeliveredDsnTotal;

		public readonly ExPerformanceCounter ExpandedDsnTotal;

		public readonly ExPerformanceCounter FailedDsnInLastHour;

		public readonly ExPerformanceCounter AlertableFailedDsnInLastHour;

		public readonly ExPerformanceCounter DelayedDsnInLastHour;

		public readonly ExPerformanceCounter CatchAllRecipientFailedDsnTotal;
	}
}
