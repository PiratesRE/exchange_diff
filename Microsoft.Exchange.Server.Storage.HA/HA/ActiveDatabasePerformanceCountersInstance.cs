using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal sealed class ActiveDatabasePerformanceCountersInstance : PerformanceCounterInstance
	{
		internal ActiveDatabasePerformanceCountersInstance(string instanceName, ActiveDatabasePerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeIS HA Active Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesEmitted = new BufferedPerformanceCounter(base.CategoryName, "Total Log Bytes Generated", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesEmitted, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesEmitted);
				this.EmitDataRate = new BufferedPerformanceCounter(base.CategoryName, "Log Generation Rate (Bytes/sec)", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EmitDataRate, new ExPerformanceCounter[0]);
				list.Add(this.EmitDataRate);
				this.TotalEmitCalls = new BufferedPerformanceCounter(base.CategoryName, "Total Emit Calls", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalEmitCalls, new ExPerformanceCounter[0]);
				list.Add(this.TotalEmitCalls);
				this.EmitCallRate = new BufferedPerformanceCounter(base.CategoryName, "Emit Calls/sec", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.EmitCallRate, new ExPerformanceCounter[0]);
				list.Add(this.EmitCallRate);
				this.AverageEmitTime = new BufferedPerformanceCounter(base.CategoryName, "Average Emit Latency", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEmitTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitTime);
				this.AverageEmitTimeBase = new BufferedPerformanceCounter(base.CategoryName, "AverageEmitTimeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEmitTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitTimeBase);
				this.AverageEmitAndSendTime = new BufferedPerformanceCounter(base.CategoryName, "Average Emit And Send Latency", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEmitAndSendTime, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitAndSendTime);
				this.AverageEmitAndSendTimeBase = new BufferedPerformanceCounter(base.CategoryName, "AverageEmitAndSendTimeBase", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageEmitAndSendTimeBase, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitAndSendTimeBase);
				this.CurrentLogGenerationNumber = new BufferedPerformanceCounter(base.CategoryName, "Current Log Generation Number", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CurrentLogGenerationNumber, new ExPerformanceCounter[0]);
				list.Add(this.CurrentLogGenerationNumber);
				this.BlockModeOverflows = new BufferedPerformanceCounter(base.CategoryName, "Block Mode Replication Overflows", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BlockModeOverflows, new ExPerformanceCounter[0]);
				list.Add(this.BlockModeOverflows);
				this.CompressionEnabled = new BufferedPerformanceCounter(base.CategoryName, "CompressionEnabled", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CompressionEnabled, new ExPerformanceCounter[0]);
				list.Add(this.CompressionEnabled);
				this.TotalBytesInputToCompression = new BufferedPerformanceCounter(base.CategoryName, "Total Bytes Input To Compression", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesInputToCompression, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesInputToCompression);
				this.TotalBytesOutputFromCompression = new BufferedPerformanceCounter(base.CategoryName, "Total Bytes Output From Compression", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalBytesOutputFromCompression, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesOutputFromCompression);
				long num = this.TotalBytesEmitted.RawValue;
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

		internal ActiveDatabasePerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchangeIS HA Active Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalBytesEmitted = new ExPerformanceCounter(base.CategoryName, "Total Log Bytes Generated", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesEmitted);
				this.EmitDataRate = new ExPerformanceCounter(base.CategoryName, "Log Generation Rate (Bytes/sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EmitDataRate);
				this.TotalEmitCalls = new ExPerformanceCounter(base.CategoryName, "Total Emit Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalEmitCalls);
				this.EmitCallRate = new ExPerformanceCounter(base.CategoryName, "Emit Calls/sec", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.EmitCallRate);
				this.AverageEmitTime = new ExPerformanceCounter(base.CategoryName, "Average Emit Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitTime);
				this.AverageEmitTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageEmitTimeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitTimeBase);
				this.AverageEmitAndSendTime = new ExPerformanceCounter(base.CategoryName, "Average Emit And Send Latency", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitAndSendTime);
				this.AverageEmitAndSendTimeBase = new ExPerformanceCounter(base.CategoryName, "AverageEmitAndSendTimeBase", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageEmitAndSendTimeBase);
				this.CurrentLogGenerationNumber = new ExPerformanceCounter(base.CategoryName, "Current Log Generation Number", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentLogGenerationNumber);
				this.BlockModeOverflows = new ExPerformanceCounter(base.CategoryName, "Block Mode Replication Overflows", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BlockModeOverflows);
				this.CompressionEnabled = new ExPerformanceCounter(base.CategoryName, "CompressionEnabled", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CompressionEnabled);
				this.TotalBytesInputToCompression = new ExPerformanceCounter(base.CategoryName, "Total Bytes Input To Compression", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesInputToCompression);
				this.TotalBytesOutputFromCompression = new ExPerformanceCounter(base.CategoryName, "Total Bytes Output From Compression", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalBytesOutputFromCompression);
				long num = this.TotalBytesEmitted.RawValue;
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

		public readonly ExPerformanceCounter TotalBytesEmitted;

		public readonly ExPerformanceCounter EmitDataRate;

		public readonly ExPerformanceCounter TotalEmitCalls;

		public readonly ExPerformanceCounter EmitCallRate;

		public readonly ExPerformanceCounter AverageEmitTime;

		public readonly ExPerformanceCounter AverageEmitTimeBase;

		public readonly ExPerformanceCounter AverageEmitAndSendTime;

		public readonly ExPerformanceCounter AverageEmitAndSendTimeBase;

		public readonly ExPerformanceCounter CurrentLogGenerationNumber;

		public readonly ExPerformanceCounter BlockModeOverflows;

		public readonly ExPerformanceCounter CompressionEnabled;

		public readonly ExPerformanceCounter TotalBytesInputToCompression;

		public readonly ExPerformanceCounter TotalBytesOutputFromCompression;
	}
}
