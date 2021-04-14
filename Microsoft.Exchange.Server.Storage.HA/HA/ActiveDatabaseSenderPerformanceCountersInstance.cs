using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal sealed class ActiveDatabaseSenderPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal ActiveDatabaseSenderPerformanceCountersInstance(string instanceName, ActiveDatabaseSenderPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeIS HA Active Database Sender")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesSent = new BufferedPerformanceCounter(base.CategoryName, "Total Bytes Sent", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesSent, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesSent);
				this.TotalNetworkWrites = new BufferedPerformanceCounter(base.CategoryName, "Total Network Writes", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalNetworkWrites, new ExPerformanceCounter[0]);
				list.Add(this.TotalNetworkWrites);
				this.WritesPerSec = new BufferedPerformanceCounter(base.CategoryName, "Network Writes/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WritesPerSec, new ExPerformanceCounter[0]);
				list.Add(this.WritesPerSec);
				this.AverageWriteTime = new BufferedPerformanceCounter(base.CategoryName, "Avg. Network sec/Write", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTime);
				this.AverageWriteTimeBase = new BufferedPerformanceCounter(base.CategoryName, "AverageWriteTimeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTimeBase);
				this.AverageWriteSize = new BufferedPerformanceCounter(base.CategoryName, "Avg. Network Bytes/Write", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteSize, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteSize);
				this.AverageWriteSizeBase = new BufferedPerformanceCounter(base.CategoryName, "AverageWriteSizeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteSizeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteSizeBase);
				this.WriteThruput = new BufferedPerformanceCounter(base.CategoryName, "Network Write Bytes/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriteThruput, new ExPerformanceCounter[0]);
				list.Add(this.WriteThruput);
				this.AverageWriteAckLatency = new BufferedPerformanceCounter(base.CategoryName, "Avg. mSec/Write Ack", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteAckLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteAckLatency);
				this.AverageWriteAckLatencyBase = new BufferedPerformanceCounter(base.CategoryName, "AverageWriteAckLatencyBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageWriteAckLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteAckLatencyBase);
				this.AverageSocketWriteLatency = new BufferedPerformanceCounter(base.CategoryName, "Avg. Microsecond/SocketWrite", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageSocketWriteLatency, new ExPerformanceCounter[0]);
				list.Add(this.AverageSocketWriteLatency);
				this.AverageSocketWriteLatencyBase = new BufferedPerformanceCounter(base.CategoryName, "AverageSocketWriteLatencyBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageSocketWriteLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageSocketWriteLatencyBase);
				this.AcknowledgedGenerationNumber = new BufferedPerformanceCounter(base.CategoryName, "Acknowledged Generation Number", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AcknowledgedGenerationNumber, new ExPerformanceCounter[0]);
				list.Add(this.AcknowledgedGenerationNumber);
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

		internal ActiveDatabaseSenderPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeIS HA Active Database Sender")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesSent = new ExPerformanceCounter(base.CategoryName, "Total Bytes Sent", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesSent);
				this.TotalNetworkWrites = new ExPerformanceCounter(base.CategoryName, "Total Network Writes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalNetworkWrites);
				this.WritesPerSec = new ExPerformanceCounter(base.CategoryName, "Network Writes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WritesPerSec);
				this.AverageWriteTime = new ExPerformanceCounter(base.CategoryName, "Avg. Network sec/Write", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTime);
				this.AverageWriteTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageWriteTimeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteTimeBase);
				this.AverageWriteSize = new ExPerformanceCounter(base.CategoryName, "Avg. Network Bytes/Write", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteSize);
				this.AverageWriteSizeBase = new ExPerformanceCounter(base.CategoryName, "AverageWriteSizeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteSizeBase);
				this.WriteThruput = new ExPerformanceCounter(base.CategoryName, "Network Write Bytes/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.WriteThruput);
				this.AverageWriteAckLatency = new ExPerformanceCounter(base.CategoryName, "Avg. mSec/Write Ack", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteAckLatency);
				this.AverageWriteAckLatencyBase = new ExPerformanceCounter(base.CategoryName, "AverageWriteAckLatencyBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageWriteAckLatencyBase);
				this.AverageSocketWriteLatency = new ExPerformanceCounter(base.CategoryName, "Avg. Microsecond/SocketWrite", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSocketWriteLatency);
				this.AverageSocketWriteLatencyBase = new ExPerformanceCounter(base.CategoryName, "AverageSocketWriteLatencyBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageSocketWriteLatencyBase);
				this.AcknowledgedGenerationNumber = new ExPerformanceCounter(base.CategoryName, "Acknowledged Generation Number", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AcknowledgedGenerationNumber);
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

		public readonly ExPerformanceCounter TotalNetworkWrites;

		public readonly ExPerformanceCounter WritesPerSec;

		public readonly ExPerformanceCounter AverageWriteTime;

		public readonly ExPerformanceCounter AverageWriteTimeBase;

		public readonly ExPerformanceCounter AverageWriteSize;

		public readonly ExPerformanceCounter AverageWriteSizeBase;

		public readonly ExPerformanceCounter WriteThruput;

		public readonly ExPerformanceCounter AverageWriteAckLatency;

		public readonly ExPerformanceCounter AverageWriteAckLatencyBase;

		public readonly ExPerformanceCounter AverageSocketWriteLatency;

		public readonly ExPerformanceCounter AverageSocketWriteLatencyBase;

		public readonly ExPerformanceCounter AcknowledgedGenerationNumber;
	}
}
