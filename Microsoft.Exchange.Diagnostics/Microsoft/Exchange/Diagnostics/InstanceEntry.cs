using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class InstanceEntry
	{
		public InstanceEntry(IntPtr handle, int offset) : this(InstanceEntry.GetInternalInstanceEntry(handle, offset), handle)
		{
			this.offset = offset;
			this.InitializeNextInstance(handle);
		}

		private unsafe InstanceEntry(InternalInstanceEntry* internalIntancesEntry, IntPtr handle)
		{
			if (null == internalIntancesEntry)
			{
				throw new ArgumentNullException("internalCategoryEntry");
			}
			this.internalInstanceEntry = internalIntancesEntry;
			this.Initialize(handle);
		}

		public InstanceEntry Next
		{
			get
			{
				return this.nextInstanceEntry;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public CounterEntry FirstCounter
		{
			get
			{
				return this.counterEntry;
			}
		}

		public unsafe int RefCount
		{
			get
			{
				return this.internalInstanceEntry->RefCount;
			}
			set
			{
				this.internalInstanceEntry->RefCount = value;
			}
		}

		public unsafe int SpinLock
		{
			get
			{
				return this.internalInstanceEntry->SpinLock;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public unsafe int NameOffset
		{
			get
			{
				return this.internalInstanceEntry->InstanceNameOffset;
			}
			set
			{
				this.internalInstanceEntry->InstanceNameOffset = value;
			}
		}

		public unsafe int FirstCounterOffset
		{
			get
			{
				return this.internalInstanceEntry->FirstCounterOffset;
			}
			set
			{
				this.internalInstanceEntry->FirstCounterOffset = value;
			}
		}

		public unsafe int NextInstanceOffset
		{
			get
			{
				return this.internalInstanceEntry->NextInstanceOffset;
			}
			set
			{
				this.internalInstanceEntry->NextInstanceOffset = value;
			}
		}

		public unsafe override string ToString()
		{
			return string.Format("{0}({1:X}) RefCount={2} SpinLock={3} Offset={4}", new object[]
			{
				this.Name,
				this.internalInstanceEntry->InstanceNameHashCode,
				this.RefCount,
				this.SpinLock,
				this.Offset
			});
		}

		private unsafe static InternalInstanceEntry* GetInternalInstanceEntry(IntPtr handle, int offset)
		{
			return (long)handle / (long)sizeof(InternalInstanceEntry) + offset;
		}

		private unsafe void Initialize(IntPtr handle)
		{
			if (this.internalInstanceEntry->FirstCounterOffset != 0)
			{
				this.counterEntry = CounterEntry.GetCounterEntry(handle, this.internalInstanceEntry->FirstCounterOffset);
			}
			this.name = new string((long)handle / 2L + this.internalInstanceEntry->InstanceNameOffset);
		}

		private unsafe void InitializeNextInstance(IntPtr handle)
		{
			InstanceEntry instanceEntry = this;
			while (instanceEntry.internalInstanceEntry->NextInstanceOffset != 0)
			{
				InstanceEntry instanceEntry2 = new InstanceEntry(InstanceEntry.GetInternalInstanceEntry(handle, instanceEntry.internalInstanceEntry->NextInstanceOffset), handle);
				instanceEntry2.offset = instanceEntry.internalInstanceEntry->NextInstanceOffset;
				instanceEntry.nextInstanceEntry = instanceEntry2;
				instanceEntry = instanceEntry2;
			}
		}

		private unsafe InternalInstanceEntry* internalInstanceEntry;

		private InstanceEntry nextInstanceEntry;

		private string name;

		private CounterEntry counterEntry;

		private int offset;
	}
}
