using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum RestrictionCheckResult : uint
	{
		AcceptedNoPermissionList = 1U,
		AcceptedInRecipientList,
		AcceptedInGroupList,
		AcceptedAcceptanceListEmpty,
		AcceptedSizeOK,
		AcceptedPrivilegedSender,
		AcceptedInBypassModerationRecipientList,
		AcceptedInBypassModerationGroupList,
		AcceptedInModeratorsList,
		AcceptedInOwnersList,
		AcceptedJournalReport,
		Moderated = 1073741824U,
		Failed = 2147483648U,
		MessageTooLargeForReceiver,
		MessageTooLargeForSender,
		MessageTooLargeForOrganization,
		RejectedInRecipientList,
		RejectedInGroupList,
		RejectedAcceptanceListNonEmpty,
		SenderNotAuthenticated,
		InvalidDirectoryObject,
		RejectedAcceptanceGroupListNonEmpty
	}
}
