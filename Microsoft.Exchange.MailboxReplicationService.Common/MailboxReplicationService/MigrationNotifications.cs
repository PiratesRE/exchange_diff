using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class MigrationNotifications
	{
		public const string MailboxLockedNotification = "MailboxLocked";

		public const string MailboxCannotBeUnlockedNotification = "MailboxCannotBeUnlocked";

		public const string RequestIsPoisonedNotification = "RequestIsPoisoned";

		public const string CloudMailboxNotConvertedToMailUser = "SourceMailboxNotMorphedToMeu";

		public const string MRSConfigSettingsErrorNotification = "MRSConfigSettingsCorrupted";

		public static readonly string CorruptJobNotification = "CorruptJobError";

		public static readonly string CorruptJobItemNotification = "CorruptJobItemError";

		public static readonly string CannotMoveMailboxDueToExistingMoveNotInProgress = "CannotMoveMailboxDueToExistingMoveNotInProgress";
	}
}
