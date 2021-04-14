using System;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal enum LinkType
	{
		Member,
		AuthOrig,
		DLMemRejectPerms,
		DLMemSubmitPerms,
		ManagedBy,
		Manager,
		MSExchBypassModerationFromDLMembersLink,
		MSExchBypassModerationLink,
		MSExchCoManagedByLink,
		MSExchDelegateListLink,
		MSExchModeratedByLink,
		PublicDelegates,
		UnauthOrig
	}
}
