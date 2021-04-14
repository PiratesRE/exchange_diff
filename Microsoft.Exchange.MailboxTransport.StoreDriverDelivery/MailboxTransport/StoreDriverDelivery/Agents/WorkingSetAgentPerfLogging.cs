using System;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class WorkingSetAgentPerfLogging : IPerformanceDataLogger
	{
		public TimeSpan StopwatchTime
		{
			get
			{
				return this.stopwatchTime;
			}
		}

		public TimeSpan CpuTime
		{
			get
			{
				return this.cpuTime;
			}
		}

		public uint StoreRPCs
		{
			get
			{
				return this.storeRPCs;
			}
		}

		public void Log(string marker, string counter, TimeSpan dataPoint)
		{
			if (counter.Equals("ElapsedTime"))
			{
				this.stopwatchTime = dataPoint;
				return;
			}
			if (counter.Equals("CpuTime"))
			{
				this.cpuTime = dataPoint;
			}
		}

		public void Log(string marker, string counter, uint dataPoint)
		{
			if (counter.Equals("StoreRpcCount"))
			{
				this.storeRPCs = dataPoint;
			}
		}

		public void Log(string marker, string counter, string dataPoint)
		{
			throw new NotImplementedException();
		}

		private TimeSpan stopwatchTime;

		private TimeSpan cpuTime;

		private uint storeRPCs;
	}
}
