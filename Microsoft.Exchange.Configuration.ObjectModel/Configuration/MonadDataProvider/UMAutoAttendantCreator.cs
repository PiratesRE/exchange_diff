using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class UMAutoAttendantCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Identity",
				"Name",
				"SpeechEnabled",
				"DTMFFallbackAutoAttendant",
				"PilotIdentifierList",
				"NameLookupEnabled",
				"BusinessName",
				"BusinessLocation",
				"UMDialPlan",
				"Status",
				"BusinessHoursWelcomeGreetingFilename",
				"BusinessHoursWelcomeGreetingEnabled",
				"AfterHoursWelcomeGreetingFilename",
				"AfterHoursWelcomeGreetingEnabled",
				"InfoAnnouncementFilename",
				"InfoAnnouncementEnabled",
				"BusinessHoursMainMenuCustomPromptFilename",
				"BusinessHoursMainMenuCustomPromptEnabled",
				"AfterHoursMainMenuCustomPromptFilename",
				"AfterHoursMainMenuCustomPromptEnabled",
				"BusinessHoursSchedule",
				"HolidaySchedule",
				"TimeZone",
				"OperatorExtension",
				"BusinessHoursTransferToOperatorEnabled",
				"AfterHoursTransferToOperatorEnabled",
				"CallSomeoneEnabled",
				"SendVoiceMsgEnabled",
				"ContactScope",
				"ContactAddressList",
				"MatchedNameSelectionMethod",
				"Language",
				"BusinessHoursKeyMappingEnabled",
				"BusinessHoursKeyMapping",
				"AfterHoursKeyMappingEnabled",
				"AfterHoursKeyMapping",
				"AllowDialPlanSubscribers",
				"AllowExtensions",
				"AllowedInCountryOrRegionGroups",
				"AllowedInternationalGroups",
				"WhenChanged"
			};
		}

		protected override void FillProperties(Type type, PSObject psObject, object dummyObject, IList<string> properties)
		{
			this.FillProperty(type, psObject, dummyObject as ConfigurableObject, "TimeZoneName");
			base.FillProperties(type, psObject, dummyObject, properties);
		}
	}
}
