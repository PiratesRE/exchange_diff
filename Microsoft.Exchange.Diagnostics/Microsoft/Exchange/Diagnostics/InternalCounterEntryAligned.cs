using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InternalCounterEntryAligned
	{
		public int SpinLock;

		public int CounterNameHashCode;

		public int CounterNameOffset;

		public int LifetimeOffset;

		public long Value;

		public int NextCounterOffset;

		public int Padding2;
	}
}
