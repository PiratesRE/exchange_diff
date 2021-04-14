using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class NspiRpcClientConnectionPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal NspiRpcClientConnectionPerformanceCountersInstance(string instanceName, NspiRpcClientConnectionPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange NSPI RPC Client Connections")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfOpenConnections = new ExPerformanceCounter(base.CategoryName, "Number of Open NSPI RPC Client Connections", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOpenConnections);
				long num = this.NumberOfOpenConnections.RawValue;
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

		internal NspiRpcClientConnectionPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange NSPI RPC Client Connections")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.NumberOfOpenConnections = new ExPerformanceCounter(base.CategoryName, "Number of Open NSPI RPC Client Connections", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfOpenConnections);
				long num = this.NumberOfOpenConnections.RawValue;
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

		public readonly ExPerformanceCounter NumberOfOpenConnections;
	}
}
