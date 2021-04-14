using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal interface IParticipantWrapper
	{
		Participant Participant { get; }
	}
}
