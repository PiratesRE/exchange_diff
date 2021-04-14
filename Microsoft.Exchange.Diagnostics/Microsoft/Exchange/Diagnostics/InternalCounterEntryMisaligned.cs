using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InternalCounterEntryMisaligned
	{
		public int SpinLock;

		public int CounterNameHashCode;

		public int CounterNameOffset;

		public int LifetimeOffset;

		public int Value_lo;

		public int Value_hi;

		public int NextCounterOffset;

		public int Padding2;
	}
}
