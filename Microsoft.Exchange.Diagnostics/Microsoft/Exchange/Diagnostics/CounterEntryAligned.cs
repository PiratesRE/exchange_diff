using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class CounterEntryAligned : CounterEntry
	{
		public CounterEntryAligned(IntPtr handle, int offset) : this(CounterEntryAligned.GetInternalCounterEntry(handle, offset), handle)
		{
			this.offset = offset;
		}

		[CLSCompliant(false)]
		public unsafe CounterEntryAligned(InternalCounterEntryAligned* internalCounterEntry, IntPtr handle)
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
				return this.internalCounterEntry->Value;
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

		private unsafe static InternalCounterEntryAligned* GetInternalCounterEntry(IntPtr handle, int offset)
		{
			return (long)handle / (long)sizeof(InternalCounterEntryAligned) + offset;
		}

		private unsafe void Initialize(IntPtr handle)
		{
			if (this.internalCounterEntry->LifetimeOffset != 0)
			{
				this.lifetimeEntry = new LifetimeEntry(handle, this.internalCounterEntry->LifetimeOffset);
			}
			this.name = new string((long)handle / 2L + this.internalCounterEntry->CounterNameOffset);
		}

		private unsafe InternalCounterEntryAligned* internalCounterEntry;

		private int offset;
	}
}
