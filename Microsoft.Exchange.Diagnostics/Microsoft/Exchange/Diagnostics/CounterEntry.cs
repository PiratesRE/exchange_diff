using System;

namespace Microsoft.Exchange.Diagnostics
{
	public abstract class CounterEntry
	{
		public CounterEntry Next
		{
			get
			{
				return this.nextCounterEntry;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public LifetimeEntry Lifetime
		{
			get
			{
				return this.lifetimeEntry;
			}
		}

		public abstract long Value { get; }

		public abstract int SpinLock { get; }

		public abstract int Offset { get; }

		public abstract int NameOffset { get; set; }

		public abstract int NextCounterOffset { get; set; }

		public abstract int LifetimeOffset { get; set; }

		public static CounterEntry GetCounterEntry(IntPtr handle, int offset)
		{
			CounterEntry counterEntry = CounterEntry.InternalGetCounterEntry(handle, offset);
			counterEntry.InitializeNextCounter(handle);
			return counterEntry;
		}

		public override string ToString()
		{
			if (this.Lifetime == null)
			{
				return string.Format("{0} Value={1} SpinLock={2}", this.Name, this.Value, this.SpinLock);
			}
			return string.Format("{0} Value={1} SpinLock={2} Lifetime={3}", new object[]
			{
				this.Name,
				this.Value,
				this.SpinLock,
				this.Lifetime
			});
		}

		private static CounterEntry InternalGetCounterEntry(IntPtr handle, int offset)
		{
			if (offset % 8 == 0)
			{
				return new CounterEntryAligned(handle, offset);
			}
			return new CounterEntryMisaligned(handle, offset);
		}

		private void InitializeNextCounter(IntPtr handle)
		{
			CounterEntry counterEntry = this;
			while (counterEntry.NextCounterOffset != 0)
			{
				CounterEntry counterEntry2 = CounterEntry.InternalGetCounterEntry(handle, counterEntry.NextCounterOffset);
				counterEntry.nextCounterEntry = counterEntry2;
				counterEntry = counterEntry2;
			}
		}

		[CLSCompliant(false)]
		protected CounterEntry nextCounterEntry;

		[CLSCompliant(false)]
		protected string name;

		[CLSCompliant(false)]
		protected LifetimeEntry lifetimeEntry;
	}
}
