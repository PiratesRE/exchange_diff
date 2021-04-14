using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal sealed class HttpProxyPerArrayCountersInstance : PerformanceCounterInstance
	{
		internal HttpProxyPerArrayCountersInstance(string instanceName, HttpProxyPerArrayCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange HttpProxy Per Array")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalServersInArray = new ExPerformanceCounter(base.CategoryName, "Total Servers In Array", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalServersInArray);
				long num = this.TotalServersInArray.RawValue;
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

		internal HttpProxyPerArrayCountersInstance(string instanceName) : base(instanceName, "MSExchange HttpProxy Per Array")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalServersInArray = new ExPerformanceCounter(base.CategoryName, "Total Servers In Array", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalServersInArray);
				long num = this.TotalServersInArray.RawValue;
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

		public readonly ExPerformanceCounter TotalServersInArray;
	}
}
