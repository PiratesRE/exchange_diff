using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct StringValue
	{
		internal StringValue(FormatStore store, int stringHandle)
		{
			this.Strings = store.Strings;
			this.StringHandle = stringHandle;
		}

		internal StringValue(FormatStore.StringValueStore strings, int stringHandle)
		{
			this.Strings = strings;
			this.StringHandle = stringHandle;
		}

		public PropertyValue PropertyValue
		{
			get
			{
				return new PropertyValue(PropertyType.String, this.StringHandle);
			}
		}

		public int Length
		{
			get
			{
				return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str.Length;
			}
		}

		public int RefCount
		{
			get
			{
				return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount;
			}
		}

		internal int Handle
		{
			get
			{
				return this.StringHandle;
			}
		}

		public string GetString()
		{
			return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str;
		}

		public void CopyTo(int sourceOffset, char[] buffer, int offset, int count)
		{
			this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str.CopyTo(sourceOffset, buffer, offset, count);
		}

		public void AddRef()
		{
			if (this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount != 2147483647)
			{
				FormatStore.StringValueEntry[] array = this.Strings.Plane(this.StringHandle);
				int num = this.Strings.Index(this.StringHandle);
				array[num].RefCount = array[num].RefCount + 1;
			}
		}

		public void Release()
		{
			if (this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount != 2147483647)
			{
				FormatStore.StringValueEntry[] array = this.Strings.Plane(this.StringHandle);
				int num = this.Strings.Index(this.StringHandle);
				if ((array[num].RefCount = array[num].RefCount - 1) == 0)
				{
					this.Strings.Free(this.StringHandle);
				}
			}
			this.StringHandle = -1;
		}

		internal void SetString(string str)
		{
			this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str = str;
		}

		internal FormatStore.StringValueStore Strings;

		internal int StringHandle;
	}
}
