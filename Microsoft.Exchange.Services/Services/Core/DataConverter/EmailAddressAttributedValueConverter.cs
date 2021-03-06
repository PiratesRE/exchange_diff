using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class EmailAddressAttributedValueConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		public override object ConvertToServiceObjectValue(object propertyValue, IdConverterWithCommandSettings idConverterWithCommandSettings)
		{
			AttributedValue<Participant> attributedValue = (AttributedValue<Participant>)propertyValue;
			if (attributedValue == null)
			{
				return null;
			}
			EmailAddressWrapper emailAddressWrapper = ParticipantConverter.ConvertParticipantToEmailAddressWrapper(attributedValue.Value, idConverterWithCommandSettings);
			if (emailAddressWrapper == null)
			{
				return null;
			}
			return new EmailAddressAttributedValue
			{
				Attributions = attributedValue.Attributions,
				Value = emailAddressWrapper
			};
		}
	}
}
