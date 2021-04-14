using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IDataValidator : IDisposeTrackable, IDisposable
	{
		bool ValidateADContactForOutdialing(string legacyExchangeDN, out IDataValidationResult result);

		bool ValidateADContactForTransferToMailbox(string legacyExchangeDN, out IDataValidationResult result);

		bool ValidatePhoneNumberForOutdialing(string phoneNumber, out IDataValidationResult result);

		bool ValidateContactItemCallerId(StoreObjectId storeId, out IDataValidationResult result);

		bool ValidateADContactCallerId(string exchangeLegacyDN, out IDataValidationResult result);

		bool ValidatePersonaContactCallerId(string emailAddress, out IDataValidationResult result);

		bool ValidatePhoneNumberCallerId(string number, out IDataValidationResult result);

		bool ValidateContactFolderCallerId(out IDataValidationResult result);

		bool ValidateExtensions(IList<string> extensions, out PAAValidationResult result, out string extensionInError);
	}
}
