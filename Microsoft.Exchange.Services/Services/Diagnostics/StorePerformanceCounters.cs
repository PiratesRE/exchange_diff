using System;
using System.Text;

namespace Microsoft.Exchange.Services.Diagnostics
{
	public struct StorePerformanceCounters
	{
		public long ElapsedMilliseconds { get; set; }

		public double Cpu { get; set; }

		public double RpcLatency { get; set; }

		public int RpcCount { get; set; }

		public double RpcLatencyOnStore { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.AppendFormat("elapsed={0}ms\n", this.ElapsedMilliseconds);
			stringBuilder.AppendFormat("cpu={0}ms\n", this.Cpu);
			stringBuilder.AppendFormat("rpcLatency={0}ms\n", this.RpcLatency);
			stringBuilder.AppendFormat("rpcCount={0}\n", this.RpcCount);
			stringBuilder.AppendFormat("rpcLatencyOnStore={0}ms\n", this.RpcLatencyOnStore);
			return stringBuilder.ToString();
		}
	}
}
