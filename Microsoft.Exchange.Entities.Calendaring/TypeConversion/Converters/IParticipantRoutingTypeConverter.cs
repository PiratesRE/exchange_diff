using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal interface IParticipantRoutingTypeConverter
	{
		ConvertValue<Participant[], Participant[]> ConvertToEntity { get; }

		ConvertValue<Participant[], Participant[]> ConvertToStorage { get; }
	}
}
