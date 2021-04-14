using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal sealed class PswshttpRequestPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal PswshttpRequestPerformanceCountersInstance(string instanceName, PswshttpRequestPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangePowershellWebServiceHttpRequest")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "PowerShell Web Service Average Response Time", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageResponseTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				long num = this.AverageResponseTime.RawValue;
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

		internal PswshttpRequestPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangePowershellWebServiceHttpRequest")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageResponseTime = new ExPerformanceCounter(base.CategoryName, "PowerShell Web Service Average Response Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageResponseTime);
				long num = this.AverageResponseTime.RawValue;
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

		public readonly ExPerformanceCounter AverageResponseTime;
	}
}
