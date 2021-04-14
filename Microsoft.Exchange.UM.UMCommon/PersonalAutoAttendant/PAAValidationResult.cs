using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal enum PAAValidationResult
	{
		Valid,
		ParseError,
		SipUriInNonSipDialPlan,
		PermissionCheckFailure,
		NonExistentContact,
		NoValidPhones,
		NonExistentDefaultContactsFolder,
		NonExistentDirectoryUser,
		NonMailboxDirectoryUser,
		NonExistentPersona,
		InvalidExtension
	}
}
