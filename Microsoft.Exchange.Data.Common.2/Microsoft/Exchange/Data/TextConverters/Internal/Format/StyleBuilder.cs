using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct StyleBuilder
	{
		internal StyleBuilder(FormatConverter converter, int handle)
		{
			this.converter = converter;
			this.handle = handle;
		}

		public void SetProperty(PropertyId propertyId, PropertyValue value)
		{
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, value);
		}

		public void SetProperties(Property[] properties, int propertyCount)
		{
			for (int i = 0; i < propertyCount; i++)
			{
				this.SetProperty(properties[i].Id, properties[i].Value);
			}
		}

		public void SetStringProperty(PropertyId propertyId, StringValue value)
		{
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
		}

		public void SetStringProperty(PropertyId propertyId, string value)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, value);
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
		}

		public void SetStringProperty(PropertyId propertyId, BufferString value)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, value);
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
		}

		public void SetStringProperty(PropertyId propertyId, char[] buffer, int offset, int count)
		{
			StringValue stringValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
		}

		public void SetMultiValueProperty(PropertyId propertyId, MultiValue value)
		{
			value.AddRef();
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
		}

		public void SetMultiValueProperty(PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
		{
			MultiValue multiValue = this.converter.RegisterMultiValue(false, out multiValueBuilder);
			this.converter.StyleBuildHelper.SetProperty(0, propertyId, multiValue.PropertyValue);
		}

		public void Flush()
		{
			this.converter.StyleBuildHelper.GetPropertyList(out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].PropertyList, out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].FlagProperties, out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].PropertyMask);
		}

		private FormatConverter converter;

		private int handle;
	}
}
