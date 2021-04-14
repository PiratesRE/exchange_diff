using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct MultiValueBuilder
	{
		internal MultiValueBuilder(FormatConverter converter, int handle)
		{
			this.converter = converter;
			this.handle = handle;
		}

		public int Count
		{
			get
			{
				return this.converter.MultiValueBuildHelper.Count;
			}
		}

		public PropertyValue this[int i]
		{
			get
			{
				return this.converter.MultiValueBuildHelper[i];
			}
		}

		public void AddValue(PropertyValue value)
		{
			this.converter.MultiValueBuildHelper.AddValue(value);
		}

		public void AddStringValue(StringValue value)
		{
			this.converter.MultiValueBuildHelper.AddValue(value.PropertyValue);
		}

		public void AddStringValue(string value)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, value);
			this.converter.MultiValueBuildHelper.AddValue(stringValue.PropertyValue);
		}

		public void AddStringValue(char[] buffer, int offset, int count)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));
			this.converter.MultiValueBuildHelper.AddValue(stringValue.PropertyValue);
		}

		public void Flush()
		{
			this.converter.Store.MultiValues.Plane(this.handle)[this.converter.Store.MultiValues.Index(this.handle)].Values = this.converter.MultiValueBuildHelper.GetValues();
		}

		public void Cancel()
		{
			this.converter.MultiValueBuildHelper.Cancel();
		}

		private FormatConverter converter;

		private int handle;
	}
}
