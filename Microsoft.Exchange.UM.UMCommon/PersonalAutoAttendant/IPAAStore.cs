using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IPAAStore : IDisposeTrackable, IDisposable
	{
		IList<PersonalAutoAttendant> GetAutoAttendants(PAAValidationMode validationMode);

		bool TryGetAutoAttendants(PAAValidationMode validationMode, out IList<PersonalAutoAttendant> autoAttendants);

		IList<PersonalAutoAttendant> GetAutoAttendants(PAAValidationMode validationMode, out PAAStoreStatus storeStatus);

		PersonalAutoAttendant GetAutoAttendant(Guid identity, PAAValidationMode validationMode);

		bool TryGetAutoAttendant(Guid identity, PAAValidationMode validationMode, out PersonalAutoAttendant autoAttendant);

		void DeleteAutoAttendant(Guid identity);

		void DeleteGreeting(PersonalAutoAttendant paa);

		void Save(IList<PersonalAutoAttendant> autoAttendants);

		GreetingBase OpenGreeting(PersonalAutoAttendant paa);

		void DeletePAAConfiguration();

		void GetUserPermissions(out bool enabledForPersonalAutoAttendant, out bool enabledForOutdialing);

		bool Validate(PersonalAutoAttendant paa, PAAValidationMode validationMode);

		IList<string> GetExtensionsInPrimaryDialPlan();

		bool ValidatePhoneNumberForOutdialing(string number, out IDataValidationResult result);

		bool ValidateADContactForOutdialing(string legacyExchangeDN, out IDataValidationResult result);

		bool ValidateADContactForTransferToMailbox(string legacyExchangeDN, out IDataValidationResult result);

		bool ValidateContactItemCallerId(StoreObjectId storeId, out IDataValidationResult result);

		bool ValidateADContactCallerId(string exchangeLegacyDN, out IDataValidationResult result);

		bool ValidatePhoneNumberCallerId(string number, out IDataValidationResult result);

		bool ValidateContactFolderCallerId(out IDataValidationResult result);

		bool ValidatePersonaContactCallerId(string emailAddress, out IDataValidationResult result);

		bool ValidateExtensions(IList<string> extensions, out PAAValidationResult result, out string extensionInError);
	}
}
