using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal sealed class PickupPerfCountersInstance : PerformanceCounterInstance
	{
		internal PickupPerfCountersInstance(string instanceName, PickupPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Pickup")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.Submitted = new ExPerformanceCounter(base.CategoryName, "Messages Submitted", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Submitted);
				this.NDRed = new ExPerformanceCounter(base.CategoryName, "Messages NDRed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NDRed);
				this.Badmailed = new ExPerformanceCounter(base.CategoryName, "Messages Badmailed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Badmailed);
				long num = this.Submitted.RawValue;
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

		internal PickupPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Pickup")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.Submitted = new ExPerformanceCounter(base.CategoryName, "Messages Submitted", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Submitted);
				this.NDRed = new ExPerformanceCounter(base.CategoryName, "Messages NDRed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NDRed);
				this.Badmailed = new ExPerformanceCounter(base.CategoryName, "Messages Badmailed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.Badmailed);
				long num = this.Submitted.RawValue;
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

		public readonly ExPerformanceCounter Submitted;

		public readonly ExPerformanceCounter NDRed;

		public readonly ExPerformanceCounter Badmailed;
	}
}
