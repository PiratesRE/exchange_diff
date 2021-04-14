using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ContactLinkingOperation
	{
		None,
		AutoLinkViaEmailAddress,
		AutoLinkViaIMAddress,
		AutoLinkSkippedConflictPersonSets,
		AutoLinkSkippedInLinkRejectHistory,
		ManualLinking,
		Unlinking,
		RejectSuggestion,
		AutoLinkViaGalLinkId,
		AutoLinkViaEmailOrImAddressInDirectoryPerson,
		AutoLinkSkippedDirectoryPersonAlreadyLinked,
		AutoLinkSkippedDirectoryPersonUnlinked,
		AutoLinkSkippedConflictingGALLinkState
	}
}
