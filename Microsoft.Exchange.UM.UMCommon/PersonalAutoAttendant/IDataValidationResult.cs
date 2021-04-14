using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IDataValidationResult
	{
		PAAValidationResult PAAValidationResult { get; }

		ADRecipient ADRecipient { get; }

		PhoneNumber PhoneNumber { get; }

		PersonalContactInfo PersonalContactInfo { get; }

		PersonaType PersonaContactInfo { get; }
	}
}
