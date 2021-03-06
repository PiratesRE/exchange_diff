using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ExDateTimeAttributedValueConverter : BaseConverter
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
			AttributedValue<ExDateTime> attributedValue = (AttributedValue<ExDateTime>)propertyValue;
			return new StringAttributedValue
			{
				Attributions = attributedValue.Attributions,
				Value = ExDateTimeConverter.ToUtcXsdDateTime(attributedValue.Value)
			};
		}
	}
}
