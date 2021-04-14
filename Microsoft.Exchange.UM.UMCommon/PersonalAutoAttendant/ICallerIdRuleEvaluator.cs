using System;
using System.Collections.Generic;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface ICallerIdRuleEvaluator
	{
		PersonalContactInfo[] MatchedPersonalContacts { get; }

		ADContactInfo MatchedADContact { get; }

		List<string> MatchedPersonaEmails { get; }

		PhoneNumber GetCallerId();
	}
}
