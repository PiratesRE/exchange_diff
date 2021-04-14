using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum CalendarInconsistencyFlag
	{
		None,
		StoreObjectValidation,
		StorageException,
		UserNotFound,
		LegacyUser,
		LargeDL,
		OrphanedMeeting,
		VersionInfo,
		TimeOverlap,
		StartTime,
		EndTime,
		StartTimeZone,
		RecurringTimeZone,
		Location,
		Cancellation,
		RecurrenceBlob,
		RecurrenceAnomaly,
		RecurringException,
		ModifiedOccurrenceMatch,
		MissingOccurrenceDeletion,
		ExtraOccurrenceDeletion,
		MissingItem,
		DuplicatedItem,
		Response,
		Organizer,
		MissingCvs
	}
}
