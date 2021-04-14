using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MemoryDataProvider : PerformanceDataProvider
	{
		private MemoryDataProvider() : base("KBytesAllocated", false)
		{
		}

		internal static MemoryDataProvider Instance
		{
			get
			{
				return MemoryDataProvider.singletonInstance;
			}
		}

		public override PerformanceData TakeSnapshot(bool begin)
		{
			base.Latency = TimeSpan.Zero;
			base.RequestCount = (uint)(GC.GetTotalMemory(false) >> 10);
			return base.TakeSnapshot(begin);
		}

		private const int DivByOneKShiftBits = 10;

		private static readonly MemoryDataProvider singletonInstance = new MemoryDataProvider();
	}
}
