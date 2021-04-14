using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct MultiValueBuildHelper
	{
		internal MultiValueBuildHelper(FormatStore store)
		{
			this.Store = store;
			this.Values = null;
			this.ValuesCount = 0;
		}

		public int Count
		{
			get
			{
				return this.ValuesCount;
			}
		}

		public PropertyValue this[int i]
		{
			get
			{
				return this.Values[i];
			}
		}

		public void AddValue(PropertyValue value)
		{
			if (this.Values == null)
			{
				this.Values = new PropertyValue[4];
			}
			else if (this.ValuesCount == this.Values.Length)
			{
				if (this.ValuesCount == MultiValueBuildHelper.MaxValues)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				PropertyValue[] array = new PropertyValue[this.ValuesCount * 2];
				Array.Copy(this.Values, 0, array, 0, this.ValuesCount);
				this.Values = array;
			}
			this.Values[this.ValuesCount++] = value;
		}

		public PropertyValue[] GetValues()
		{
			if (this.ValuesCount == 0)
			{
				return null;
			}
			PropertyValue[] array = new PropertyValue[this.ValuesCount];
			Array.Copy(this.Values, 0, array, 0, this.ValuesCount);
			this.ValuesCount = 0;
			return array;
		}

		public void Cancel()
		{
			for (int i = 0; i < this.ValuesCount; i++)
			{
				if (this.Values[i].IsRefCountedHandle)
				{
					this.Store.ReleaseValue(this.Values[i]);
				}
			}
			this.ValuesCount = 0;
		}

		internal static readonly int MaxValues = 32;

		internal FormatStore Store;

		internal PropertyValue[] Values;

		internal int ValuesCount;
	}
}
