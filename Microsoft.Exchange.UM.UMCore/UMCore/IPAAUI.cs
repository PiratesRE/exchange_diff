using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.PersonalAutoAttendant;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPAAUI
	{
		void SetADTransferTargetMenuItem(int key, string type, string context, string legacyExchangeDN, ADRecipient transferTarget);

		void SetPhoneNumberTransferMenuItem(int key, string type, string context, string phoneNumberString);

		void SetFindMeMenuItem(int key, string type, string context);

		void SetMenuItemTransferToVoiceMail();

		void SetADTransferTarget(ADRecipient transferTarget);

		void SetTransferToMailboxEnabled();

		void SetInvalidADContact();

		void SetTransferToVoiceMessageEnabled();

		void SetBlindTransferEnabled(bool enabled, PhoneNumber target);

		void SetPermissionCheckFailure();

		void SetFindMeNumbers(FindMe[] numbers);
	}
}
