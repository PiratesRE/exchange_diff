using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PhoneNumberAttributedValueConverter : BaseConverter
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
			AttributedValue<PhoneNumber> attributedValue = (AttributedValue<PhoneNumber>)propertyValue;
			return new PhoneNumberAttributedValue
			{
				Attributions = attributedValue.Attributions,
				Value = new PhoneNumber
				{
					Number = attributedValue.Value.Number,
					Type = attributedValue.Value.Type
				}
			};
		}
	}
}
