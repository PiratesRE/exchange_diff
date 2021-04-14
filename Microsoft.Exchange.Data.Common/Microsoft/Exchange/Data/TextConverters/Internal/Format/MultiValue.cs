using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct MultiValue
	{
		internal MultiValue(FormatStore store, int multiValueHandle)
		{
			this.MultiValues = store.MultiValues;
			this.MultiValueHandle = multiValueHandle;
		}

		internal MultiValue(FormatStore.MultiValueStore multiValues, int multiValueHandle)
		{
			this.MultiValues = multiValues;
			this.MultiValueHandle = multiValueHandle;
		}

		public PropertyValue PropertyValue
		{
			get
			{
				return new PropertyValue(PropertyType.MultiValue, this.MultiValueHandle);
			}
		}

		public int Length
		{
			get
			{
				return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values.Length;
			}
		}

		internal int Handle
		{
			get
			{
				return this.MultiValueHandle;
			}
		}

		internal int RefCount
		{
			get
			{
				return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount;
			}
		}

		public PropertyValue this[int index]
		{
			get
			{
				return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values[index];
			}
		}

		public StringValue GetStringValue(int index)
		{
			return this.MultiValues.Store.GetStringValue(this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values[index]);
		}

		public void AddRef()
		{
			if (this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount != 2147483647)
			{
				FormatStore.MultiValueEntry[] array = this.MultiValues.Plane(this.MultiValueHandle);
				int num = this.MultiValues.Index(this.MultiValueHandle);
				array[num].RefCount = array[num].RefCount + 1;
			}
		}

		public void Release()
		{
			if (this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount != 2147483647)
			{
				FormatStore.MultiValueEntry[] array = this.MultiValues.Plane(this.MultiValueHandle);
				int num = this.MultiValues.Index(this.MultiValueHandle);
				if ((array[num].RefCount = array[num].RefCount - 1) == 0)
				{
					this.MultiValues.Free(this.MultiValueHandle);
				}
			}
			this.MultiValueHandle = -1;
		}

		internal FormatStore.MultiValueStore MultiValues;

		internal int MultiValueHandle;
	}
}
