using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct FormatStyle
	{
		internal FormatStyle(FormatStore store, int styleHandle)
		{
			this.Styles = store.Styles;
			this.StyleHandle = styleHandle;
		}

		internal FormatStyle(FormatStore.StyleStore styles, int styleHandle)
		{
			this.Styles = styles;
			this.StyleHandle = styleHandle;
		}

		public int Handle
		{
			get
			{
				return this.StyleHandle;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.StyleHandle == 0;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask.IsClear && (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList == null || this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList.Length == 0);
			}
		}

		public FlagProperties FlagProperties
		{
			get
			{
				return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].FlagProperties;
			}
			set
			{
				this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].FlagProperties = value;
			}
		}

		public PropertyBitMask PropertyMask
		{
			get
			{
				return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask;
			}
			set
			{
				this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyMask = value;
			}
		}

		public Property[] PropertyList
		{
			get
			{
				return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList;
			}
			set
			{
				this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].PropertyList = value;
			}
		}

		internal int RefCount
		{
			get
			{
				return this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount;
			}
		}

		public void AddRef()
		{
			if (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount != 2147483647)
			{
				FormatStore.StyleEntry[] array = this.Styles.Plane(this.StyleHandle);
				int num = this.Styles.Index(this.StyleHandle);
				array[num].RefCount = array[num].RefCount + 1;
			}
		}

		public void Release()
		{
			if (this.Styles.Plane(this.StyleHandle)[this.Styles.Index(this.StyleHandle)].RefCount != 2147483647)
			{
				FormatStore.StyleEntry[] array = this.Styles.Plane(this.StyleHandle);
				int num = this.Styles.Index(this.StyleHandle);
				if ((array[num].RefCount = array[num].RefCount - 1) == 0)
				{
					this.Styles.Free(this.StyleHandle);
				}
			}
			this.StyleHandle = -1;
		}

		public static readonly FormatStyle Null = default(FormatStyle);

		internal FormatStore.StyleStore Styles;

		internal int StyleHandle;
	}
}
