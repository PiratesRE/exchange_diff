using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class CategoryEntry
	{
		public CategoryEntry(IntPtr handle, int offset) : this(CategoryEntry.GetInternalCategoryEntry(handle, offset), handle)
		{
			this.offset = offset;
			this.InitializeNextCategory(handle);
		}

		private unsafe CategoryEntry(InternalCategoryEntry* internalCategoryEntry, IntPtr handle)
		{
			if (null == internalCategoryEntry)
			{
				throw new ArgumentNullException("internalCategoryEntry");
			}
			this.internalCategoryEntry = internalCategoryEntry;
			this.Initialize(handle);
		}

		public CategoryEntry Next
		{
			get
			{
				return this.nextCategoryEntry;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public InstanceEntry FirstInstance
		{
			get
			{
				return this.instanceEntry;
			}
		}

		public unsafe int SpinLock
		{
			get
			{
				return this.internalCategoryEntry->SpinLock;
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
				return this.internalCategoryEntry->CategoryNameOffset;
			}
			set
			{
				this.internalCategoryEntry->CategoryNameOffset = value;
			}
		}

		public unsafe int FirstInstanceOffset
		{
			get
			{
				return this.internalCategoryEntry->FirstInstanceOffset;
			}
			set
			{
				this.internalCategoryEntry->FirstInstanceOffset = value;
			}
		}

		public unsafe int NextCategoryOffset
		{
			get
			{
				return this.internalCategoryEntry->NextCategoryOffset;
			}
			set
			{
				this.internalCategoryEntry->NextCategoryOffset = value;
			}
		}

		public unsafe void Refresh()
		{
			this.Initialize((IntPtr)((void*)(this.internalCategoryEntry - this.offset / sizeof(InternalCategoryEntry))));
		}

		public unsafe void RefreshInstances(IntPtr handle)
		{
			if (this.internalCategoryEntry->FirstInstanceOffset != 0)
			{
				this.instanceEntry = new InstanceEntry(handle, this.internalCategoryEntry->FirstInstanceOffset);
			}
		}

		public unsafe override string ToString()
		{
			return string.Format("{0}({1:X}) SpinLock={2}", this.Name, this.internalCategoryEntry->CategoryNameHashCode, this.SpinLock);
		}

		private unsafe static InternalCategoryEntry* GetInternalCategoryEntry(IntPtr handle, int offset)
		{
			return (long)handle / (long)sizeof(InternalCategoryEntry) + offset;
		}

		private unsafe void Initialize(IntPtr handle)
		{
			if (this.internalCategoryEntry->FirstInstanceOffset != 0)
			{
				this.instanceEntry = new InstanceEntry(handle, this.internalCategoryEntry->FirstInstanceOffset);
			}
			this.name = new string((long)handle / 2L + this.internalCategoryEntry->CategoryNameOffset);
		}

		private unsafe void InitializeNextCategory(IntPtr handle)
		{
			CategoryEntry categoryEntry = this;
			while (categoryEntry.internalCategoryEntry->NextCategoryOffset != 0)
			{
				CategoryEntry categoryEntry2 = new CategoryEntry(CategoryEntry.GetInternalCategoryEntry(handle, categoryEntry.internalCategoryEntry->NextCategoryOffset), handle);
				categoryEntry.nextCategoryEntry = categoryEntry2;
				categoryEntry = categoryEntry2;
			}
		}

		private unsafe InternalCategoryEntry* internalCategoryEntry;

		private CategoryEntry nextCategoryEntry;

		private string name;

		private InstanceEntry instanceEntry;

		private int offset;
	}
}
