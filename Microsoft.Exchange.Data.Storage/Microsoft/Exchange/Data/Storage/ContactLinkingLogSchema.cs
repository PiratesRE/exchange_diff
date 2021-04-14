using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ContactLinkingLogSchema
	{
		internal enum ContactUpdate
		{
			ItemId,
			PersonId,
			Linked,
			LinkRejectHistory,
			GALLinkState,
			GALLinkID,
			AddressBookEntryId,
			SmtpAddressCache,
			UserApprovedLink
		}

		internal enum ContactLinking
		{
			LinkOperation,
			LinkingPersonId,
			LinkingItemId,
			LinkToPersonId,
			LinkToItemId
		}

		internal enum ContactUnlinking
		{
			PersonId,
			ItemId,
			ADObjectIdGuid
		}

		internal enum GALLinkFixup
		{
			PersonId,
			ADObjectIdGuid
		}

		internal enum RejectSuggestion
		{
			PersonId,
			SuggestionPersonId
		}

		internal enum Error
		{
			Exception,
			Context
		}

		internal enum Warning
		{
			Context
		}

		internal enum SkippedContactLink
		{
			LinkingPersonId,
			LinkingItemId,
			LinkToPersonId,
			LinkToItemId,
			LinkToPersonContactCount,
			CurrentCountOfContactsAdded,
			MaximumContactsAllowedToAdd
		}

		internal enum PerformanceData
		{
			Elapsed,
			CPU,
			RPCCount,
			RPCLatency,
			DirectoryCount,
			DirectoryLatency,
			StoreTimeInServer,
			StoreTimeInCPU,
			StorePagesRead,
			StorePagesPreRead,
			StoreLogRecords,
			StoreLogBytes,
			ContactsCreated,
			ContactsUpdated,
			ContactsRead,
			ContactsProcessed
		}

		internal enum MigrationStart
		{
			DueTime
		}

		internal enum MigrationEnd
		{
			Success
		}
	}
}
