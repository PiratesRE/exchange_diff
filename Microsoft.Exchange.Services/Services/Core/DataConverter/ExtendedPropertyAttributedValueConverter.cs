using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ExtendedPropertyAttributedValueConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			if (propertyValue == null)
			{
				return null;
			}
			AttributedValue<ContactExtendedPropertyData> attributedValue = (AttributedValue<ContactExtendedPropertyData>)propertyValue;
			ExtendedPropertyUri propertyUri = new ExtendedPropertyUri((NativeStorePropertyDefinition)attributedValue.Value.PropertyDefinition);
			ExtendedPropertyType extendedPropertyForValues = ExtendedPropertyProperty.GetExtendedPropertyForValues(propertyUri, attributedValue.Value.PropertyDefinition, attributedValue.Value.RawValue);
			return new ExtendedPropertyAttributedValue(extendedPropertyForValues, attributedValue.Attributions);
		}
	}
}
