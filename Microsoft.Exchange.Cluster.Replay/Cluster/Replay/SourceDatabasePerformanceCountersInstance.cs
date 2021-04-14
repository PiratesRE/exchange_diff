using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class SourceDatabasePerformanceCountersInstance : PerformanceCounterInstance
	{
		internal SourceDatabasePerformanceCountersInstance(string instanceName, SourceDatabasePerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeRepl Source Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesSent = new ExPerformanceCounter(base.CategoryName, "Total Bytes Sent", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesSent, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesSent);
				this.AverageWriteTime = new ExPerformanceCounter(base.CategoryName, "Avg. Network sec/Write", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTime);
				this.AverageWriteTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageWriteTimeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTimeBase);
				this.AverageReadTime = new ExPerformanceCounter(base.CategoryName, "Avg. Disk sec/Read", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageReadTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadTime);
				this.AverageReadTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageReadTimeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageReadTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadTimeBase);
				this.WriteThruput = new ExPerformanceCounter(base.CategoryName, "Network Write Bytes/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriteThruput, new ExPerformanceCounter[0]);
				list.Add(this.WriteThruput);
				long num = this.TotalBytesSent.RawValue;
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

		internal SourceDatabasePerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeRepl Source Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesSent = new ExPerformanceCounter(base.CategoryName, "Total Bytes Sent", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesSent);
				this.AverageWriteTime = new ExPerformanceCounter(base.CategoryName, "Avg. Network sec/Write", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTime);
				this.AverageWriteTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageWriteTimeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTimeBase);
				this.AverageReadTime = new ExPerformanceCounter(base.CategoryName, "Avg. Disk sec/Read", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadTime);
				this.AverageReadTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageReadTimeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageReadTimeBase);
				this.WriteThruput = new ExPerformanceCounter(base.CategoryName, "Network Write Bytes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WriteThruput);
				long num = this.TotalBytesSent.RawValue;
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

		public readonly ExPerformanceCounter TotalBytesSent;

		public readonly ExPerformanceCounter AverageWriteTime;

		public readonly ExPerformanceCounter AverageWriteTimeBase;

		public readonly ExPerformanceCounter AverageReadTime;

		public readonly ExPerformanceCounter AverageReadTimeBase;

		public readonly ExPerformanceCounter WriteThruput;
	}
}
