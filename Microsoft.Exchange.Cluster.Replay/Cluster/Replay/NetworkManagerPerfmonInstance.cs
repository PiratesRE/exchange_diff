using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class NetworkManagerPerfmonInstance : PerformanceCounterInstance
	{
		internal NetworkManagerPerfmonInstance(string instanceName, NetworkManagerPerfmonInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Network Manager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.LogCopyThruputReceived = new ExPerformanceCounter(base.CategoryName, "Log Copy KB Received/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LogCopyThruputReceived, new ExPerformanceCounter[0]);
				list.Add(this.LogCopyThruputReceived);
				this.SeederThruputReceived = new ExPerformanceCounter(base.CategoryName, "Seeder KB Received/Sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SeederThruputReceived, new ExPerformanceCounter[0]);
				list.Add(this.SeederThruputReceived);
				this.TotalCompressedLogBytesReceived = new ExPerformanceCounter(base.CategoryName, "Total Compressed Log Bytes Received", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalCompressedLogBytesReceived, new ExPerformanceCounter[0]);
				list.Add(this.TotalCompressedLogBytesReceived);
				this.TotalLogBytesDecompressed = new ExPerformanceCounter(base.CategoryName, "Total Log Bytes Decompressed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalLogBytesDecompressed, new ExPerformanceCounter[0]);
				list.Add(this.TotalLogBytesDecompressed);
				this.TotalCompressedSeedingBytesReceived = new ExPerformanceCounter(base.CategoryName, "Total Compressed Seeding Bytes Received", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalCompressedSeedingBytesReceived, new ExPerformanceCounter[0]);
				list.Add(this.TotalCompressedSeedingBytesReceived);
				this.TotalSeedingBytesDecompressed = new ExPerformanceCounter(base.CategoryName, "Total Seeding Bytes Decompressed", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalSeedingBytesDecompressed, new ExPerformanceCounter[0]);
				list.Add(this.TotalSeedingBytesDecompressed);
				long num = this.LogCopyThruputReceived.RawValue;
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

		internal NetworkManagerPerfmonInstance(string instanceName) : base(instanceName, "MSExchange Network Manager")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.LogCopyThruputReceived = new ExPerformanceCounter(base.CategoryName, "Log Copy KB Received/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LogCopyThruputReceived);
				this.SeederThruputReceived = new ExPerformanceCounter(base.CategoryName, "Seeder KB Received/Sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SeederThruputReceived);
				this.TotalCompressedLogBytesReceived = new ExPerformanceCounter(base.CategoryName, "Total Compressed Log Bytes Received", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCompressedLogBytesReceived);
				this.TotalLogBytesDecompressed = new ExPerformanceCounter(base.CategoryName, "Total Log Bytes Decompressed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalLogBytesDecompressed);
				this.TotalCompressedSeedingBytesReceived = new ExPerformanceCounter(base.CategoryName, "Total Compressed Seeding Bytes Received", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCompressedSeedingBytesReceived);
				this.TotalSeedingBytesDecompressed = new ExPerformanceCounter(base.CategoryName, "Total Seeding Bytes Decompressed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalSeedingBytesDecompressed);
				long num = this.LogCopyThruputReceived.RawValue;
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

		public readonly ExPerformanceCounter LogCopyThruputReceived;

		public readonly ExPerformanceCounter SeederThruputReceived;

		public readonly ExPerformanceCounter TotalCompressedLogBytesReceived;

		public readonly ExPerformanceCounter TotalLogBytesDecompressed;

		public readonly ExPerformanceCounter TotalCompressedSeedingBytesReceived;

		public readonly ExPerformanceCounter TotalSeedingBytesDecompressed;
	}
}
