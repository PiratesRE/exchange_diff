using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class CounterEntryMisaligned : CounterEntry
	{
		public CounterEntryMisaligned(IntPtr handle, int offset) : this(CounterEntryMisaligned.GetInternalCounterEntry(handle, offset), handle)
		{
			this.offset = offset;
		}

		private unsafe CounterEntryMisaligned(InternalCounterEntryMisaligned* internalCounterEntry, IntPtr handle)
		{
			if (null == internalCounterEntry)
			{
				throw new ArgumentNullException("internalCategoryEntry");
			}
			this.internalCounterEntry = internalCounterEntry;
			this.Initialize(handle);
		}

		public unsafe override long Value
		{
			get
			{
				long num = (long)this.internalCounterEntry->Value_hi << 32;
				return num + (long)this.internalCounterEntry->Value_lo;
			}
		}

		public unsafe override int SpinLock
		{
			get
			{
				return this.internalCounterEntry->SpinLock;
			}
		}

		public override int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public unsafe override int NameOffset
		{
			get
			{
				return this.internalCounterEntry->CounterNameOffset;
			}
			set
			{
				this.internalCounterEntry->CounterNameOffset = value;
			}
		}

		public unsafe override int NextCounterOffset
		{
			get
			{
				return this.internalCounterEntry->NextCounterOffset;
			}
			set
			{
				this.internalCounterEntry->NextCounterOffset = value;
			}
		}

		public unsafe override int LifetimeOffset
		{
			get
			{
				return this.internalCounterEntry->LifetimeOffset;
			}
			set
			{
				this.internalCounterEntry->LifetimeOffset = value;
			}
		}

		private unsafe static InternalCounterEntryMisaligned* GetInternalCounterEntry(IntPtr handle, int offset)
		{
			return (long)handle / (long)sizeof(InternalCounterEntryMisaligned) + offset;
		}

		private unsafe void Initialize(IntPtr handle)
		{
			if (this.internalCounterEntry->LifetimeOffset != 0)
			{
				this.lifetimeEntry = new LifetimeEntry(handle, this.internalCounterEntry->LifetimeOffset);
			}
			this.name = new string((long)handle / 2L + this.internalCounterEntry->CounterNameOffset);
		}

		private unsafe InternalCounterEntryMisaligned* internalCounterEntry;

		private int offset;
	}
}
