using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IDataLoader : IDisposeTrackable, IDisposable
	{
		PhoneNumber GetCallerId();

		string GetDiversionForCall();

		void GetUserOofSettings(out UserOofSettings owaOof, out bool telOof);

		void GetFreeBusyInformation(out FreeBusyStatusEnum freeBusy);

		WorkingHours GetWorkingHours();

		PersonalContactInfo[] GetMatchingPersonalContacts(PhoneNumber callerId);

		ADContactInfo GetMatchingADContact(PhoneNumber callerId);

		List<string> GetMatchingPersonaEmails();

		ExTimeZone GetUserTimeZone();

		UMSubscriber GetUMSubscriber();

		bool TryIsWithinCompanyWorkingHours(out bool withinWorkingHours);
	}
}
